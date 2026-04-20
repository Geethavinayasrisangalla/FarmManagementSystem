using FarmManagement.Web.Models.ViewModels;
using FarmManagement.Web.Models.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FarmManagement.Web.Controllers;

[Authorize(Roles = "Admin,Manager,Worker")]
public class ResourceController : Controller
{
    private readonly IResourceService _resourceService;
    private readonly IScheduleService _scheduleService;
    private readonly IActivityService _activityService;

    public ResourceController(IResourceService resourceService,
                               IScheduleService scheduleService,
                               IActivityService activityService)
    {
        _resourceService = resourceService;
        _scheduleService = scheduleService;
        _activityService = activityService;
    }

    private int    CurrentUserId   => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
    private string CurrentUserName => User.FindFirstValue(ClaimTypes.Name) ?? "Unknown";
    private string CurrentUserRole => User.FindFirstValue(ClaimTypes.Role) ?? "Unknown";

    public async Task<IActionResult> Index()
    {
        var resources = await _resourceService.GetAllAsync();
        return View(resources);
    }

    public async Task<IActionResult> Details(int id)
    {
        var resource = await _resourceService.GetByIdAsync(id);
        if (resource == null) return NotFound();
        return View(resource);
    }

    [Authorize(Roles = "Admin,Manager")]
    public IActionResult Create() => View(new InventoryViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Create(InventoryViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        await _resourceService.CreateAsync(vm);

        await _activityService.LogAsync(CurrentUserId, CurrentUserName, CurrentUserRole,
            "Created", "Resource", $"Added resource '{vm.Name}' — {vm.Quantity} {vm.Unit}");

        TempData["Success"] = $"Resource '{vm.Name}' added successfully.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Edit(int id)
    {
        var resource = await _resourceService.GetByIdAsync(id);
        if (resource == null) return NotFound();

        var vm = new InventoryViewModel
        {
            ResourceId = resource.ResourceId,
            Name       = resource.Name,
            Type       = resource.Type,
            Quantity   = resource.Quantity,
            Unit       = resource.Unit
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Edit(int id, InventoryViewModel vm)
    {
        if (id != vm.ResourceId) return BadRequest();
        if (!ModelState.IsValid) return View(vm);

        await _resourceService.UpdateAsync(vm);

        await _activityService.LogAsync(CurrentUserId, CurrentUserName, CurrentUserRole,
            "Updated", "Resource", $"Updated resource '{vm.Name}' — {vm.Quantity} {vm.Unit}");

        TempData["Success"] = $"Resource '{vm.Name}' updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Allocate(int id)
    {
        var resource = await _resourceService.GetByIdAsync(id);
        if (resource == null) return NotFound();

        ViewBag.Resource  = resource;
        ViewBag.Schedules = await _scheduleService.GetAllAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Allocate(int resourceId, int scheduleId,
                                               decimal qty, string? notes)
    {
        try
        {
            await _resourceService.AllocateAsync(resourceId, scheduleId, qty, notes);

            var resource = await _resourceService.GetByIdAsync(resourceId);
            await _activityService.LogAsync(CurrentUserId, CurrentUserName, CurrentUserRole,
                "Allocated", "Resource",
                $"Used {qty} {resource?.Unit} of '{resource?.Name}' for schedule #{scheduleId}");

            TempData["Success"] = "Resource allocated successfully.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var resource = await _resourceService.GetByIdAsync(id);
        if (resource == null) return NotFound();

        try
        {
            await _resourceService.DeleteAsync(id);

            await _activityService.LogAsync(CurrentUserId, CurrentUserName, CurrentUserRole,
                "Deleted", "Resource", $"Deleted resource '{resource.Name}'");

            TempData["Success"] = $"Resource '{resource.Name}' deleted.";
        }
        catch (Exception)
        {
            TempData["Error"] = $"Could not delete '{resource.Name}'. Please try again.";
        }

        return RedirectToAction(nameof(Index));
    }
}
