using FarmManagement.Web.Models.ViewModels;

public interface IReportService
{
    Task<DashboardViewModel> GetDashboardDataAsync();
    Task<YieldAnalyticsViewModel> GetYieldAnalyticsAsync();
}