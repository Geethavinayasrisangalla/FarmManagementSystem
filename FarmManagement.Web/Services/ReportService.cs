using FarmManagement.Web.Data;
using FarmManagement.Web.Models.Entities;
using FarmManagement.Web.Models.Enums;
using FarmManagement.Web.Models.Interfaces;
using FarmManagement.Web.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FarmManagement.Web.Services;

public class ReportService : IReportService
{
    private readonly FarmDbContext _db;
    public ReportService(FarmDbContext db) => _db = db;

    public async Task<DashboardViewModel> GetDashboardDataAsync()
    {
        var builder = new DashboardBuilder(_db);

        await builder.WithFieldCountAsync();
        await builder.WithCropCountAsync();
        await builder.WithActivePestCountAsync();
        await builder.WithLowStockCountAsync();
        await builder.WithUpcomingHarvestsAsync();
        await builder.WithTotalYieldAsync();
        await builder.WithRecentSchedulesAsync();
        await builder.WithRecentPestAlertsAsync();

        return builder.Build();
    }

    public async Task<YieldAnalyticsViewModel> GetYieldAnalyticsAsync()
    {
        var records = await _db.Harvests.AsNoTracking()
                               .Include(h => h.PlantingSchedule)
                                   .ThenInclude(ps => ps.Crop)
                               .Include(h => h.PlantingSchedule)
                                   .ThenInclude(ps => ps.Field)
                               .OrderByDescending(h => h.HarvestedDate)
                               .ToListAsync();

        return new YieldAnalyticsViewModel
        {
            Records      = records,
            CropNames    = records.Select(r => r.PlantingSchedule.Crop.CropName).ToList(),
            YieldValues  = records.Select(r => r.ActualYieldKg).ToList(),
            TotalYield   = records.Sum(r => r.ActualYieldKg),
            AverageYield = records.Count > 0 ? records.Average(r => r.ActualYieldKg) : 0
        };
    }

    public async Task GenerateYieldReportAsync()
    {
        var harvests = await _db.Harvests.AsNoTracking()
            .Include(h => h.PlantingSchedule).ThenInclude(ps => ps.Crop)
            .Include(h => h.PlantingSchedule).ThenInclude(ps => ps.Field)
            .Where(h => h.HarvestedDate.Year == DateTime.Today.Year)
            .ToListAsync();

        var grouped = harvests.GroupBy(h => new
        {
            h.PlantingSchedule.CropId,
            h.PlantingSchedule.Crop.Season
        });

        foreach (var group in grouped)
        {
            var totalYield = group.Sum(h => h.ActualYieldKg);
            var totalArea = group.Select(h => h.PlantingSchedule.Field)
                                 .DistinctBy(f => f.FieldId)
                                 .Sum(f => f.AreaHectares);
            var avgPerAcre = totalArea > 0 ? totalYield / totalArea : 0;

            var existing = await _db.YieldReports.FirstOrDefaultAsync(y =>
                y.CropId == group.Key.CropId &&
                y.Season == group.Key.Season &&
                y.Year == DateTime.Today.Year);

            if (existing != null)
            {
                existing.TotalYieldKg = totalYield;
                existing.AverageYieldPerAcre = avgPerAcre;
                existing.GeneratedAt = DateTime.Now;
                existing.Remarks = $"Auto-generated from {group.Count()} harvest records";
            }
            else
            {
                _db.YieldReports.Add(new YieldReport
                {
                    CropId = group.Key.CropId,
                    TotalYieldKg = totalYield,
                    AverageYieldPerAcre = avgPerAcre,
                    Season = group.Key.Season,
                    Year = DateTime.Today.Year,
                    Remarks = $"Auto-generated from {group.Count()} harvest records"
                });
            }
        }

        await _db.SaveChangesAsync();
    }

    public async Task<IEnumerable<YieldReport>> GetYieldReportsAsync() =>
        await _db.YieldReports.AsNoTracking()
            .Include(y => y.Crop)
            .OrderByDescending(y => y.GeneratedAt)
            .ToListAsync();

    public async Task<PestSummaryViewModel> GetPestSummaryAsync()
    {
        var incidents = await _db.PestIncidents.AsNoTracking()
            .Include(p => p.Crop)
            .ToListAsync();

        return new PestSummaryViewModel
        {
            TotalIncidents = incidents.Count,
            ActiveCount = incidents.Count(p => p.Status == IncidentStatus.Active),
            MonitoringCount = incidents.Count(p => p.Status == IncidentStatus.Monitoring),
            ResolvedCount = incidents.Count(p => p.Status == IncidentStatus.Resolved),
            Incidents = incidents.OrderByDescending(p => p.ReportedDate)
        };
    }

    public async Task<ResourceReportViewModel> GetResourceReportAsync()
    {
        var resources = await _db.Resources.AsNoTracking()
            .Include(r => r.ResourceUsages)
            .OrderBy(r => r.Name)
            .ToListAsync();

        return new ResourceReportViewModel
        {
            TotalResources = resources.Count,
            LowStockCount = resources.Count(r => r.Quantity <= 10),
            TotalAllocations = resources.Sum(r => r.ResourceUsages.Count),
            Resources = resources
        };
    }

    public async Task<FarmAnalyticsViewModel> GetFarmAnalyticsAsync()
    {
        return new FarmAnalyticsViewModel
        {
            TotalFields = await _db.Fields.CountAsync(),
            TotalCrops = await _db.Crops.CountAsync(),
            TotalResources = await _db.Resources.CountAsync(),
            TotalHarvests = await _db.Harvests.CountAsync(),
            TotalPestIncidents = await _db.PestIncidents.CountAsync(),
            ActivePests = await _db.PestIncidents.CountAsync(p => p.Status == IncidentStatus.Active),
            LowStockItems = await _db.Resources.CountAsync(r => r.Quantity <= 10),
            TotalYieldKg = await _db.Harvests.SumAsync(h => (decimal?)h.ActualYieldKg) ?? 0,
            TotalFieldArea = await _db.Fields.SumAsync(f => (decimal?)f.AreaHectares) ?? 0,
            CropsByStatus = await _db.Crops.AsNoTracking()
                .GroupBy(c => c.Status)
                .Select(g => new StatusCount { Status = g.Key, Count = g.Count() })
                .ToListAsync(),
            ResourcesByType = await _db.Resources.AsNoTracking()
                .GroupBy(r => r.Type)
                .Select(g => new TypeCount { Type = g.Key.ToString(), Count = g.Count(), TotalQty = g.Sum(r => r.Quantity) })
                .ToListAsync()
        };
    }
}
