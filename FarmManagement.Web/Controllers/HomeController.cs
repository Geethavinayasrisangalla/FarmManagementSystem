using FarmManagement.Models.ViewModels;
using FarmManagement.Services.Interfaces;
using FarmManagement.Web.Models;
using FarmManagement.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FarmManagement.Controllers;

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