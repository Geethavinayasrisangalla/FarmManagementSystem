using FarmManagement.Web.Models.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FarmManagement.Web.Controllers;

[Authorize(Roles = "Admin,Manager,Supervisor,Viewer")]
public class ReportController : Controller
{
    private readonly IReportService _reportService;

    public ReportController(IReportService reportService)
    {
        _reportService = reportService;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> YieldAnalytics()
    {
        var vm = await _reportService.GetYieldAnalyticsAsync();
        return View(vm);
    }
}
