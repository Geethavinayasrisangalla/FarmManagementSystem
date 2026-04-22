using FarmManagement.Web.Events;
using FarmManagement.Web.Factories;
using FarmManagement.Web.Models.Interfaces;
using FarmManagement.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FarmManagement.Web.Controllers;

// Patterns used:
//   Factory  — IFieldFactory maps entity ↔ ViewModel (no manual mapping in controller)
//   Observer — IEventDispatcher replaces direct IActivityService calls
[Authorize(Roles = "Admin,Farmer,FieldSupervisor,Agronomist")]
public class FieldController : Controller
{
    private readonly IFieldService     _fieldService;
    private readonly IFieldFactory     _fieldFactory;
    private readonly IEventDispatcher  _dispatcher;

    public FieldController(IFieldService fieldService, IFieldFactory fieldFactory,
                           IEventDispatcher dispatcher)
    {
        _fieldService = fieldService;
        _fieldFactory = fieldFactory;
        _dispatcher   = dispatcher;
    }

    private int    CurrentUserId   => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
    private string CurrentUserName => User.FindFirstValue(ClaimTypes.Name) ?? "Unknown";
    private string CurrentUserRole => User.FindFirstValue(ClaimTypes.Role) ?? "Unknown";

    public async Task<IActionResult> Index()
    {
        var fields = await _fieldService.GetAllAsync();
        return View(fields);
    }

    public async Task<IActionResult> Details(int id)
    {
        var field = await _fieldService.GetByIdAsync(id);
        if (field == null) return NotFound();
        return View(field);
    }

    [Authorize(Roles = "Admin,Farmer,FieldSupervisor")]
    public IActionResult Create() => View(new FieldViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Farmer,FieldSupervisor")]
    public async Task<IActionResult> Create(FieldViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        await _fieldService.CreateAsync(vm);

        // Observer Pattern — dispatch event; ActivityLogHandler writes the log
        await _dispatcher.DispatchAsync(new FieldCreatedEvent(
            CurrentUserId, CurrentUserName, CurrentUserRole,
            vm.FieldName, vm.AreaHectares, vm.SoilType));

        TempData["Success"] = $"Field '{vm.FieldName}' added successfully.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin,Farmer,FieldSupervisor")]
    public async Task<IActionResult> Edit(int id)
    {
        var field = await _fieldService.GetByIdAsync(id);
        if (field == null) return NotFound();

        // Factory Pattern — one line instead of 5-line manual mapping
        var vm = _fieldFactory.ToViewModel(field);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Farmer,FieldSupervisor")]
    public async Task<IActionResult> Edit(int id, FieldViewModel vm)
    {
        if (id != vm.FieldId) return BadRequest();
        if (!ModelState.IsValid) return View(vm);

        await _fieldService.UpdateAsync(vm);

        // Observer Pattern
        await _dispatcher.DispatchAsync(new FieldUpdatedEvent(
            CurrentUserId, CurrentUserName, CurrentUserRole,
            vm.FieldName, vm.AreaHectares, vm.SoilType));

        TempData["Success"] = $"Field '{vm.FieldName}' updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Farmer,FieldSupervisor")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var field = await _fieldService.GetByIdAsync(id);
        if (field == null) return NotFound();

        try
        {
            await _fieldService.DeleteAsync(id);

            // Observer Pattern
            await _dispatcher.DispatchAsync(new FieldDeletedEvent(
                CurrentUserId, CurrentUserName, CurrentUserRole, field.FieldName));

            TempData["Success"] = $"Field '{field.FieldName}' deleted.";
        }
        catch (Exception)
        {
            TempData["Error"] = $"Could not delete '{field.FieldName}'. Please try again.";
        }

        return RedirectToAction(nameof(Index));
    }
}
