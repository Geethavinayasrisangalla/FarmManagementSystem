using FarmManagement.Models;
using FarmManagement.Models.ViewModels;
using FarmManagement.Services.Interfaces;
using FarmManagement.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FarmManagement.Controllers;

public class CropController : Controller
{
    private readonly ICropService _cropService;
    private readonly IFieldService _fieldService;

    public CropController(ICropService cropService, IFieldService fieldService)
    {
        _cropService = cropService;
        _fieldService = fieldService;
    }

    // GET: /Crop
    public async Task<IActionResult> Index()
    {
        var crops = await _cropService.GetAllAsync();
        return View(crops);
    }

    // GET: /Crop/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var crop = await _cropService.GetByIdAsync(id);
        if (crop == null) return NotFound();
        return View(crop);
    }

    // GET: /Crop/Create
    public async Task<IActionResult> Create()
    {
        var vm = await _cropService.PrepareViewModelAsync();
        return View(vm);
    }

    // POST: /Crop/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CropViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            var prepared = await _cropService.PrepareViewModelAsync(vm);
            return View(prepared);
        }

        await _cropService.CreateAsync(vm);
        TempData["Success"] = $"Crop '{vm.CropName}' added successfully.";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Crop/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var crop = await _cropService.GetByIdAsync(id);
        if (crop == null) return NotFound();

        var vm = new CropViewModel
        {
            CropId = crop.CropId,
            CropName = crop.CropName,
            CropType = crop.CropType,
            Season = crop.Season,
            PlantingDate = crop.PlantingDate,
            ExpectedHarvestDate = crop.ExpectedHarvestDate,
            FieldId = crop.FieldId,
            Status = crop.Status
        };

        var prepared = await _cropService.PrepareViewModelAsync(vm);
        return View(prepared);
    }

    // POST: /Crop/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CropViewModel vm)
    {
        if (id != vm.CropId) return BadRequest();

        if (!ModelState.IsValid)
        {
            var prepared = await _cropService.PrepareViewModelAsync(vm);
            return View(prepared);
        }

        await _cropService.UpdateAsync(vm);
        TempData["Success"] = $"Crop '{vm.CropName}' updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    // POST: /Crop/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var crop = await _cropService.GetByIdAsync(id);
        if (crop == null) return NotFound();

        await _cropService.DeleteAsync(id);
        TempData["Success"] = $"Crop '{crop.CropName}' deleted.";
        return RedirectToAction(nameof(Index));
    }
}