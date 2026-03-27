using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using FarmManagement.Web.Models.Entities;
using FarmManagement.Web.Models.ViewModels;
using FarmManagement.Web.Services;
using FarmManagement.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace FarmManagement.Web.Controllers
{
    public class ResourceController : Controller
    {
        private readonly IResourceService _resourceService;
        private readonly FarmDbContext _db; // used for Field dropdown in usage form

        public ResourceController(IResourceService resourceService, FarmDbContext db)
        {
            _resourceService = resourceService;
            _db = db;
        }

        // ─── INDEX: List all resources ──────────────────────────────────────
        public async Task<IActionResult> Index()
        {
            var resources = await _resourceService.GetAllAsync();
            var vm = new InventoryViewModel { Resources = resources };
            return View(vm);
        }

        // ─── DETAILS: View one resource + usage history ─────────────────────
        public async Task<IActionResult> Details(int id)
        {
            var resource = await _resourceService.GetByIdAsync(id);
            if (resource == null) return NotFound();
            return View(resource);
        }

        // ─── CREATE GET ──────────────────────────────────────────────────────
        public IActionResult Create()
        {
            PopulateTypeDropdown();
            return View(new Resource());
        }

        // ─── CREATE POST ─────────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Resource resource)
        {
            if (!ModelState.IsValid)
            {
                PopulateTypeDropdown(resource.Type);
                return View(resource);
            }

            await _resourceService.CreateAsync(resource);
            TempData["Success"] = $"Resource '{resource.Name}' added to inventory.";
            return RedirectToAction(nameof(Index));
        }

        // ─── EDIT GET ────────────────────────────────────────────────────────
        public async Task<IActionResult> Edit(int id)
        {
            var resource = await _resourceService.GetByIdAsync(id);
            if (resource == null) return NotFound();
            PopulateTypeDropdown(resource.Type);
            return View(resource);
        }

        // ─── EDIT POST ───────────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Resource resource)
        {
            if (id != resource.ResourceId) return BadRequest();

            if (!ModelState.IsValid)
            {
                PopulateTypeDropdown(resource.Type);
                return View(resource);
            }

            var updated = await _resourceService.UpdateAsync(resource);
            if (updated == null) return NotFound();

            TempData["Success"] = $"Resource '{resource.Name}' updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        // ─── DELETE POST ─────────────────────────────────────────────────────
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var deleted = await _resourceService.DeleteAsync(id);
            if (!deleted) return NotFound();

            TempData["Success"] = "Resource deleted.";
            return RedirectToAction(nameof(Index));
        }

        // ─── USAGE LOG: View all usages for a resource ───────────────────────
        public async Task<IActionResult> UsageLog(int id)
        {
            var resource = await _resourceService.GetByIdAsync(id);
            if (resource == null) return NotFound();
            ViewBag.Resource = resource;

            var usages = await _resourceService.GetUsagesByResourceAsync(id);
            return View(usages);
        }

        // ─── LOG USAGE GET (form to record a usage) ─────────────────────────
        public async Task<IActionResult> LogUsage(int? resourceId)
        {
            var vm = await BuildUsageViewModel(resourceId);
            return View(vm);
        }

        // ─── LOG USAGE POST ──────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogUsage(ResourceUsageViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                var refreshed = await BuildUsageViewModel(vm.ResourceId);
                refreshed.QuantityUsed = vm.QuantityUsed;
                refreshed.Remarks = vm.Remarks;
                refreshed.DateApplied = vm.DateApplied;
                return View(refreshed);
            }

            var usage = new ResourceUsage
            {
                ResourceId = vm.ResourceId,
                FieldId = vm.FieldId,
                QuantityUsed = vm.QuantityUsed,
                DateApplied = vm.DateApplied,
                Remarks = vm.Remarks
            };

            var (success, error) = await _resourceService.CreateUsageAsync(usage);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, error ?? "Could not log usage.");
                var refreshed = await BuildUsageViewModel(vm.ResourceId);
                return View(refreshed);
            }

            TempData["Success"] = "Usage logged and inventory updated.";
            return RedirectToAction(nameof(UsageLog), new { id = vm.ResourceId });
        }

        // ─── Helpers ─────────────────────────────────────────────────────────
        private void PopulateTypeDropdown(string? selected = null)
        {
            var types = new List<string> { "Seed", "Fertilizer", "Pesticide", "Water", "Other" };
            ViewBag.Types = new SelectList(types, selected);
            ViewBag.Units = new SelectList(new[] { "kg", "Liters", "bags", "units", "tonnes" }, selected);
        }

        private async Task<ResourceUsageViewModel> BuildUsageViewModel(int? resourceId)
        {
            var vm = new ResourceUsageViewModel
            {
                ResourceId = resourceId ?? 0,
                DateApplied = DateTime.Today,
                AvailableResources = await _db.Resources.AsNoTracking().ToListAsync(),
                AvailableFields = await _db.Fields.AsNoTracking().ToListAsync()
            };
            return vm;
        }
    }
}