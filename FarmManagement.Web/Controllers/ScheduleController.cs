using FarmManagement.Web.Models.Interfaces;
using FarmManagement.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FarmManagement.Web.Controllers;

[Authorize(Roles = "Admin,Manager,Supervisor,Worker,Viewer")]
public class ScheduleController : Controller
{
    private readonly IScheduleService _scheduleService;
    private readonly IActivityService _activityService;

    public ScheduleController(IScheduleService scheduleService, IActivityService activityService)
    {
        _scheduleService = scheduleService;
        _activityService = activityService;
    }

    private int    CurrentUserId   => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
    private string CurrentUserName => User.FindFirstValue(ClaimTypes.Name) ?? "Unknown";
    private string CurrentUserRole => User.FindFirstValue(ClaimTypes.Role) ?? "Unknown";

    public async Task<IActionResult> Index()
    {
        var schedules = await _scheduleService.GetAllAsync();
        return View(schedules);
    }

    public async Task<IActionResult> HarvestList()
    {
        var upcoming = await _scheduleService.GetUpcomingAsync();
        return View(upcoming);
    }

    public async Task<IActionResult> Details(int id)
    {
        var schedule = await _scheduleService.GetByIdAsync(id);
        if (schedule == null) return NotFound();
        return View(schedule);
    }

    [Authorize(Roles = "Admin,Supervisor")]
    public async Task<IActionResult> Create()
    {
        var vm = await _scheduleService.PrepareViewModelAsync();
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Supervisor")]
    public async Task<IActionResult> Create(ScheduleViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            var prepared = await _scheduleService.PrepareViewModelAsync(vm);
            return View(prepared);
        }

        await _scheduleService.CreateAsync(vm);

        await _activityService.LogAsync(CurrentUserId, CurrentUserName, CurrentUserRole,
            "Created", "Schedule", $"Scheduled harvest for {vm.ScheduledDate:dd MMM yyyy} — expected {vm.ExpectedYieldKg} kg");

        TempData["Success"] = "Harvest scheduled successfully.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin,Manager,Supervisor,Worker")]
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

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Manager,Supervisor,Worker")]
    public async Task<IActionResult> RecordHarvest(int id, decimal actualYield, string? notes)
    {
        if (actualYield <= 0)
        {
            TempData["Error"] = "Actual yield must be greater than 0.";
            return RedirectToAction(nameof(RecordHarvest), new { id });
        }

        await _scheduleService.RecordHarvestAsync(id, actualYield, notes);

        await _activityService.LogAsync(CurrentUserId, CurrentUserName, CurrentUserRole,
            "Harvested", "Harvest", $"Recorded harvest #{id} — actual yield: {actualYield:N0} kg");

        TempData["Success"] = "Harvest recorded successfully.";
        return RedirectToAction(nameof(HarvestList));
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Supervisor")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _scheduleService.DeleteAsync(id);

        await _activityService.LogAsync(CurrentUserId, CurrentUserName, CurrentUserRole,
            "Deleted", "Schedule", $"Deleted schedule #{id}");

        TempData["Success"] = "Schedule deleted.";
        return RedirectToAction(nameof(Index));
    }
}
