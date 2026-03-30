using FarmManagement.Web.Models.ViewModels;
using FarmManagement.Web.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FarmManagement.Web.Controllers;

public class HomeController : Controller
{
    private readonly IReportService _reportService;

    public HomeController(IReportService reportService)
    {
        _reportService = reportService;
    }

    // GET: / (Dashboard)
    public async Task<IActionResult> Index()
    {
        var vm = await _reportService.GetDashboardDataAsync();
        return View(vm);
    }

    // GET: /Home/Error
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = HttpContext.TraceIdentifier
        });
    }
}