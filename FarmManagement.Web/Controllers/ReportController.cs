using FarmManagement.Services.Interfaces;
using FarmManagement.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FarmManagement.Controllers;

public class ReportController : Controller
{
    private readonly IReportService _reportService;

    public ReportController(IReportService reportService)
    {
        _reportService = reportService;
    }

    // GET: /Report/YieldAnalytics
    public async Task<IActionResult> YieldAnalytics()
    {
        var vm = await _reportService.GetYieldAnalyticsAsync();
        return View(vm);
    }
}