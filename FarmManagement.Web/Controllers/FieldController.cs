using FarmManagement.Models.ViewModels;
using FarmManagement.Services.Interfaces;
using FarmManagement.Web.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FarmManagement.Controllers;

public class FieldController : Controller
{
    private readonly IFieldService _fieldService;

    public FieldController(IFieldService fieldService)
    {
        _fieldService = fieldService;
    }

    // GET: /Field
    public async Task<IActionResult> Index()
    {
        var fields = await _fieldService.GetAllAsync();
        return View(fields);
    }

    // GET: /Field/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var field = await _fieldService.GetByIdAsync(id);
        if (field == null) return NotFound();
        return View(field);
    }

    // GET: /Field/Create
    public IActionResult Create()
    {
        return View(new FieldViewModel());
    }

    // POST: /Field/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(FieldViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        await _fieldService.CreateAsync(vm);
        TempData["Success"] = $"Field '{vm.FieldName}' added successfully.";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Field/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var field = await _fieldService.GetByIdAsync(id);
        if (field == null) return NotFound();

        var vm = new FieldViewModel
        {
            FieldId = field.FieldId,
            FieldName = field.FieldName,
            AreaHectares = field.AreaHectares,
            SoilType = field.SoilType,
            Location = field.Location
        };

        return View(vm);
    }

    // POST: /Field/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, FieldViewModel vm)
    {
        if (id != vm.FieldId) return BadRequest();
        if (!ModelState.IsValid) return View(vm);

        await _fieldService.UpdateAsync(vm);
        TempData["Success"] = $"Field '{vm.FieldName}' updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    // POST: /Field/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var field = await _fieldService.GetByIdAsync(id);
        if (field == null) return NotFound();

        await _fieldService.DeleteAsync(id);
        TempData["Success"] = $"Field '{field.FieldName}' deleted.";
        return RedirectToAction(nameof(Index));
    }
}