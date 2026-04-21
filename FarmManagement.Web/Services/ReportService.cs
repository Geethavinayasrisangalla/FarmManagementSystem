using FarmManagement.Web.Data;
using FarmManagement.Web.Models.Interfaces;
using FarmManagement.Web.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FarmManagement.Web.Services;

// Builder Pattern — GetDashboardDataAsync now uses DashboardBuilder instead of
// constructing DashboardViewModel inline with 8 fields in one expression.
public class ReportService : IReportService
{
    private readonly FarmDbContext _db;
    public ReportService(FarmDbContext db) => _db = db;

    public async Task<DashboardViewModel> GetDashboardDataAsync()
    {
        // Builder Pattern — each With___() method populates one section independently
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
        var records = await _db.Harvests
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
}
