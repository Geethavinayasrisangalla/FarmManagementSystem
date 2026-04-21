using FarmManagement.Web.Events;
using FarmManagement.Web.Factories;
using FarmManagement.Web.Models.Interfaces;
using FarmManagement.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FarmManagement.Web.Controllers;

// Patterns used:
//   Factory  — IResourceFactory maps entity ↔ ViewModel
//   Observer — IEventDispatcher replaces direct IActivityService calls
//   Facade   — IFarmFacade coordinates Allocate & Delete (multi-service ops)
[Authorize(Roles = "Admin,Storekeeper")]
public class ResourceController : Controller
{
    private readonly IResourceService _resourceService;
    private readonly IScheduleService _scheduleService;
    private readonly IResourceFactory _resourceFactory;
    private readonly IEventDispatcher _dispatcher;
    private readonly IFarmFacade      _facade;

    public ResourceController(IResourceService resourceService,
                               IScheduleService scheduleService,
                               IResourceFactory resourceFactory,
                               IEventDispatcher dispatcher,
                               IFarmFacade facade)
    {
        _resourceService = resourceService;
        _scheduleService = scheduleService;
        _resourceFactory = resourceFactory;
        _dispatcher      = dispatcher;
        _facade          = facade;
    }

    private int    CurrentUserId   => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
    private string CurrentUserName => User.FindFirstValue(ClaimTypes.Name) ?? "Unknown";
    private string CurrentUserRole => User.FindFirstValue(ClaimTypes.Role) ?? "Unknown";

    public async Task<IActionResult> Index(string? search)
    {
        var resources = await _resourceService.GetAllAsync();
        if (!string.IsNullOrWhiteSpace(search))
        {
            resources = resources.Where(r => r.Name.Contains(search, StringComparison.OrdinalIgnoreCase)
                                          || r.Type.ToString().Contains(search, StringComparison.OrdinalIgnoreCase));
        }
        ViewBag.Search = search;
        return View(resources);
    }

    public async Task<IActionResult> Details(int id)
    {
        var resource = await _resourceService.GetByIdAsync(id);
        if (resource == null) return NotFound();
        return View(resource);
    }

    [Authorize(Roles = "Admin,Farmer")]
    public IActionResult Create() => View(new InventoryViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Farmer")]
    public async Task<IActionResult> Create(InventoryViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        await _resourceService.CreateAsync(vm);

        // Observer Pattern
        await _dispatcher.DispatchAsync(new ResourceCreatedEvent(
            CurrentUserId, CurrentUserName, CurrentUserRole,
            vm.Name, vm.Quantity, vm.Unit));

        TempData["Success"] = $"Resource '{vm.Name}' added successfully.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin,Farmer")]
    public async Task<IActionResult> Edit(int id)
    {
        var resource = await _resourceService.GetByIdAsync(id);
        if (resource == null) return NotFound();

        // Factory Pattern — single line replaces 5-line manual ViewModel construction
        var vm = _resourceFactory.ToViewModel(resource);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Farmer")]
    public async Task<IActionResult> Edit(int id, InventoryViewModel vm)
    {
        if (id != vm.ResourceId) return BadRequest();
        if (!ModelState.IsValid) return View(vm);

        await _resourceService.UpdateAsync(vm);

        // Observer Pattern
        await _dispatcher.DispatchAsync(new ResourceUpdatedEvent(
            CurrentUserId, CurrentUserName, CurrentUserRole,
            vm.Name, vm.Quantity, vm.Unit));

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
            // Facade Pattern — single call coordinates ResourceService + event dispatch
            await _facade.AllocateResourceAsync(
                resourceId, scheduleId, qty, notes,
                CurrentUserId, CurrentUserName, CurrentUserRole);

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
    [Authorize(Roles = "Admin,Farmer")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var resource = await _resourceService.GetByIdAsync(id);
        if (resource == null) return NotFound();

        try
        {
            // Facade Pattern — single call coordinates ResourceService + event dispatch
            await _facade.DeleteResourceAsync(
                id, resource.Name,
                CurrentUserId, CurrentUserName, CurrentUserRole);

            TempData["Success"] = $"Resource '{resource.Name}' deleted.";
        }
        catch (Exception)
        {
            TempData["Error"] = $"Could not delete '{resource.Name}'. Please try again.";
        }

        return RedirectToAction(nameof(Index));
    }
}
