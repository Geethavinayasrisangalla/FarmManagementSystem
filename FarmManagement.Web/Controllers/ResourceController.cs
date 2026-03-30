using FarmManagement.Web.Models.ViewModels;
using FarmManagement.Web.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FarmManagement.Web.Controllers;

public class ResourceController : Controller
{
    private readonly IResourceService _resourceService;

    public ResourceController(IResourceService resourceService)
    {
        _resourceService = resourceService;
    }

    // GET: /Resource
    public async Task<IActionResult> Index()
    {
        var resources = await _resourceService.GetAllAsync();
        return View(resources);
    }

    // GET: /Resource/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var resource = await _resourceService.GetByIdAsync(id);
        if (resource == null) return NotFound();
        return View(resource);
    }

    // GET: /Resource/Create
    public IActionResult Create()
    {
        return View(new InventoryViewModel());
    }

    // POST: /Resource/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(InventoryViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        await _resourceService.CreateAsync(vm);
        TempData["Success"] = $"Resource '{vm.Name}' added successfully.";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Resource/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var resource = await _resourceService.GetByIdAsync(id);
        if (resource == null) return NotFound();

        var vm = new InventoryViewModel
        {
            ResourceId = resource.ResourceId,
            Name = resource.Name,
            Type = resource.Type,
            Quantity = resource.Quantity,
            Unit = resource.Unit
        };

        return View(vm);
    }

    // POST: /Resource/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, InventoryViewModel vm)
    {
        if (id != vm.ResourceId) return BadRequest();
        if (!ModelState.IsValid) return View(vm);

        await _resourceService.UpdateAsync(vm);
        TempData["Success"] = $"Resource '{vm.Name}' updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Resource/Allocate/5
    public async Task<IActionResult> Allocate(int id)
    {
        var resource = await _resourceService.GetByIdAsync(id);
        if (resource == null) return NotFound();

        ViewBag.Resource = resource;
        return View();
    }

    // POST: /Resource/Allocate
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Allocate(int resourceId, int fieldId,
                                               decimal qty, string? notes)
    {
        try
        {
            await _resourceService.AllocateAsync(resourceId, fieldId, qty, notes);
            TempData["Success"] = "Resource allocated successfully.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    // POST: /Resource/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var resource = await _resourceService.GetByIdAsync(id);
        if (resource == null) return NotFound();

        await _resourceService.DeleteAsync(id);
        TempData["Success"] = $"Resource '{resource.Name}' deleted.";
        return RedirectToAction(nameof(Index));
    }
}