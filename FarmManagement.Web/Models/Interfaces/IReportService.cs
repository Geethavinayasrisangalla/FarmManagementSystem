using FarmManagement.Web.Models.ViewModels;

namespace FarmManagement.Web.Models.Interfaces;

public interface IReportService
{
    Task<DashboardViewModel> GetDashboardDataAsync();
    Task<YieldAnalyticsViewModel> GetYieldAnalyticsAsync();
}