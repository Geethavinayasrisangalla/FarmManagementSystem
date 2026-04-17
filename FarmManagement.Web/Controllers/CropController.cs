using FarmManagement.Web.Models.ViewModels;
using FarmManagement.Web.Models.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FarmManagement.Web.Controllers;

[Authorize(Roles = "Admin,Manager,Supervisor,Viewer")]
public class CropController : Controller
{
    private readonly ICropService     _cropService;
    private readonly IActivityService _activityService;

    public CropController(ICropService cropService, IActivityService activityService)
    {
        _cropService      = cropService;
        _activityService  = activityService;
    }

    private int    CurrentUserId   => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
    private string CurrentUserName => User.FindFirstValue(ClaimTypes.Name) ?? "Unknown";
    private string CurrentUserRole => User.FindFirstValue(ClaimTypes.Role) ?? "Unknown";

    public async Task<IActionResult> Index()
    {
        var crops = await _cropService.GetAllAsync();
        return View(crops);
    }

    public async Task<IActionResult> Details(int id)
    {
        var crop = await _cropService.GetByIdAsync(id);
        if (crop == null) return NotFound();
        return View(crop);
    }

    [Authorize(Roles = "Admin,Manager,Supervisor")]
    public async Task<IActionResult> Create()
    {
        var vm = await _cropService.PrepareViewModelAsync();
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Manager,Supervisor")]
    public async Task<IActionResult> Create(CropViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            var prepared = await _cropService.PrepareViewModelAsync(vm);
            return View(prepared);
        }

        await _cropService.CreateAsync(vm);

        await _activityService.LogAsync(CurrentUserId, CurrentUserName, CurrentUserRole,
            "Created", "Crop", $"Added crop '{vm.CropName}' ({vm.CropType}) for {vm.Season} season");

        TempData["Success"] = $"Crop '{vm.CropName}' added successfully.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin,Manager,Supervisor")]
    public async Task<IActionResult> Edit(int id)
    {
        var crop = await _cropService.GetByIdAsync(id);
        if (crop == null) return NotFound();

        var vm = new CropViewModel
        {
            CropId               = crop.CropId,
            CropName             = crop.CropName,
            CropType             = crop.CropType,
            Season               = crop.Season,
            PlantingDate         = crop.PlantingDate,
            ExpectedHarvestDate  = crop.ExpectedHarvestDate,
            FieldId              = crop.FieldId,
            Status               = crop.Status
        };

        var prepared = await _cropService.PrepareViewModelAsync(vm);
        return View(prepared);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Manager,Supervisor")]
    public async Task<IActionResult> Edit(int id, CropViewModel vm)
    {
        if (id != vm.CropId) return BadRequest();

        if (!ModelState.IsValid)
        {
            var prepared = await _cropService.PrepareViewModelAsync(vm);
            return View(prepared);
        }

        await _cropService.UpdateAsync(vm);

        await _activityService.LogAsync(CurrentUserId, CurrentUserName, CurrentUserRole,
            "Updated", "Crop", $"Updated crop '{vm.CropName}' — status: {vm.Status}");

        TempData["Success"] = $"Crop '{vm.CropName}' updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Manager,Supervisor")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var crop = await _cropService.GetByIdAsync(id);
        if (crop == null) return NotFound();

        await _cropService.DeleteAsync(id);

        await _activityService.LogAsync(CurrentUserId, CurrentUserName, CurrentUserRole,
            "Deleted", "Crop", $"Deleted crop '{crop.CropName}'");

        TempData["Success"] = $"Crop '{crop.CropName}' deleted.";
        return RedirectToAction(nameof(Index));
    }
}
