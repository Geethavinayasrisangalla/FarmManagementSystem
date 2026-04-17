using FarmManagement.Web.Models.ViewModels;
using FarmManagement.Web.Models.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FarmManagement.Web.Controllers;

[Authorize(Roles = "Admin,Manager,Supervisor,Viewer")]
public class FieldController : Controller
{
    private readonly IFieldService    _fieldService;
    private readonly IActivityService _activityService;

    public FieldController(IFieldService fieldService, IActivityService activityService)
    {
        _fieldService     = fieldService;
        _activityService  = activityService;
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

    [Authorize(Roles = "Admin,Manager,Supervisor")]
    public IActionResult Create() => View(new FieldViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Manager,Supervisor")]
    public async Task<IActionResult> Create(FieldViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        await _fieldService.CreateAsync(vm);

        await _activityService.LogAsync(CurrentUserId, CurrentUserName, CurrentUserRole,
            "Created", "Field", $"Added field '{vm.FieldName}' ({vm.AreaHectares} ha, {vm.SoilType})");

        TempData["Success"] = $"Field '{vm.FieldName}' added successfully.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin,Manager,Supervisor")]
    public async Task<IActionResult> Edit(int id)
    {
        var field = await _fieldService.GetByIdAsync(id);
        if (field == null) return NotFound();

        var vm = new FieldViewModel
        {
            FieldId      = field.FieldId,
            FieldName    = field.FieldName,
            AreaHectares = field.AreaHectares,
            SoilType     = field.SoilType,
            Location     = field.Location
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Manager,Supervisor")]
    public async Task<IActionResult> Edit(int id, FieldViewModel vm)
    {
        if (id != vm.FieldId) return BadRequest();
        if (!ModelState.IsValid) return View(vm);

        await _fieldService.UpdateAsync(vm);

        await _activityService.LogAsync(CurrentUserId, CurrentUserName, CurrentUserRole,
            "Updated", "Field", $"Updated field '{vm.FieldName}' — {vm.AreaHectares} ha, {vm.SoilType}");

        TempData["Success"] = $"Field '{vm.FieldName}' updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Manager,Supervisor")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var field = await _fieldService.GetByIdAsync(id);
        if (field == null) return NotFound();

        await _fieldService.DeleteAsync(id);

        await _activityService.LogAsync(CurrentUserId, CurrentUserName, CurrentUserRole,
            "Deleted", "Field", $"Deleted field '{field.FieldName}'");

        TempData["Success"] = $"Field '{field.FieldName}' deleted.";
        return RedirectToAction(nameof(Index));
    }
}
