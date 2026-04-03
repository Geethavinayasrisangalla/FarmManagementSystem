using FarmManagement.Web.Models.Interfaces;
using FarmManagement.Web.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FarmManagement.Web.Controllers;

public class ScheduleController : Controller
{
    private readonly IScheduleService _scheduleService;

    public ScheduleController(IScheduleService scheduleService)
    {
        _scheduleService = scheduleService;
    }

    // GET: /Schedule
    public async Task<IActionResult> Index()
    {
        var schedules = await _scheduleService.GetAllAsync();
        return View(schedules);
    }

    // GET: /Schedule/HarvestList
    public async Task<IActionResult> HarvestList()
    {
        var upcoming = await _scheduleService.GetUpcomingAsync();
        return View(upcoming);
    }

    // GET: /Schedule/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var schedule = await _scheduleService.GetByIdAsync(id);
        if (schedule == null) return NotFound();
        return View(schedule);
    }

    // GET: /Schedule/Create
    public async Task<IActionResult> Create()
    {
        var vm = await _scheduleService.PrepareViewModelAsync();
        return View(vm);
    }

    // POST: /Schedule/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ScheduleViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            var prepared = await _scheduleService.PrepareViewModelAsync(vm);
            return View(prepared);
        }

        await _scheduleService.CreateAsync(vm);
        TempData["Success"] = "Harvest scheduled successfully.";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Schedule/RecordHarvest/5
    public async Task<IActionResult> RecordHarvest(int id)
    {
        var schedule = await _scheduleService.GetByIdAsync(id);
        if (schedule == null) return NotFound();

        if (schedule.Status == "Completed")
        {
            TempData["Error"] = "This harvest has already been recorded.";
            return RedirectToAction(nameof(HarvestList));
        }

        return View(schedule);
    }

    // POST: /Schedule/RecordHarvest
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RecordHarvest(int id, decimal actualYield, string? notes)
    {
        if (actualYield <= 0)
        {
            TempData["Error"] = "Actual yield must be greater than 0.";
            return RedirectToAction(nameof(RecordHarvest), new { id });
        }

        await _scheduleService.RecordHarvestAsync(id, actualYield, notes);
        TempData["Success"] = "Harvest recorded successfully.";
        return RedirectToAction(nameof(HarvestList));
    }

    // POST: /Schedule/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _scheduleService.DeleteAsync(id);
        TempData["Success"] = "Schedule deleted.";
        return RedirectToAction(nameof(Index));
    }
}