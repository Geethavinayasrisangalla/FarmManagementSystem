using FarmManagement.Web.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FarmManagement.Web.Controllers;

public class ReportController : Controller
{
    private readonly IReportService _reportService;

    public ReportController(IReportService reportService)
    {
        _reportService = reportService;
    }

    // GET: /Report
    public IActionResult Index()
    {
        return View();
    }

    // GET: /Report/YieldAnalytics
    public async Task<IActionResult> YieldAnalytics()
    {
        var vm = await _reportService.GetYieldAnalyticsAsync();
        return View(vm);
    }
}