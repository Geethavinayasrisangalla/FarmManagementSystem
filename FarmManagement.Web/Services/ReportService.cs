using FarmManagement.Web.Data;
using FarmManagement.Web.Models.Enums;
using FarmManagement.Web.Models.Interfaces;
using FarmManagement.Web.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FarmManagement.Web.Services;

public class ReportService : IReportService
{
    private readonly FarmDbContext _db;
    public ReportService(FarmDbContext db) => _db = db;

    public async Task<DashboardViewModel> GetDashboardDataAsync() =>
        new DashboardViewModel
        {
            TotalFields = await _db.Fields.CountAsync(),
            TotalCrops = await _db.Crops.CountAsync(),

            ActivePestIncidents = await _db.PestIncidents
                                           .CountAsync(p => p.Status == IncidentStatus.Active),

            LowStockResources = await _db.Resources
                                           .CountAsync(r => r.Quantity <= 10),

            UpcomingHarvests = await _db.PlantingSchedules
                                           .CountAsync(ps => ps.ScheduledDate >= DateTime.Today
                                                          && ps.ScheduledDate <= DateTime.Today.AddDays(30)
                                                          && ps.Status == "Scheduled"),

            TotalYieldThisSeason = await _db.Harvests
                                            .Where(h => h.HarvestedDate.Year == DateTime.Today.Year)
                                            .SumAsync(h => (decimal?)h.ActualYieldKg) ?? 0,

            RecentSchedules = await _db.PlantingSchedules
                                           .Include(ps => ps.Crop)
                                           .Where(ps => ps.Status == "Scheduled")
                                           .OrderBy(ps => ps.ScheduledDate)
                                           .Take(5)
                                           .ToListAsync(),

            RecentPestAlerts = await _db.PestIncidents
                                           .Include(p => p.Crop)
                                           .Where(p => p.Status == IncidentStatus.Active)
                                           .OrderByDescending(p => p.ReportedDate)
                                           .Take(5)
                                           .ToListAsync()
        };

    public async Task<YieldAnalyticsViewModel> GetYieldAnalyticsAsync()
    {
        var records = await _db.Harvests
                               .Include(h => h.PlantingSchedule)
                                   .ThenInclude(ps => ps.Crop)
                               .Include(h => h.PlantingSchedule)
                                   .ThenInclude(ps => ps.Field)
                               .OrderByDescending(h => h.HarvestedDate)
                               .ToListAsync();

        return new YieldAnalyticsViewModel
        {
            Records = records,
            CropNames = records.Select(r => r.PlantingSchedule.Crop.CropName).ToList(),
            YieldValues = records.Select(r => r.ActualYieldKg).ToList(),
            TotalYield = records.Sum(r => r.ActualYieldKg),
            AverageYield = records.Count > 0 ? records.Average(r => r.ActualYieldKg) : 0
        };
    }
}