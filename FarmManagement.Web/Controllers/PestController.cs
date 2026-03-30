using FarmManagement.Web.Models.Entities;
using FarmManagement.Web.Models.Enums;
using FarmManagement.Web.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FarmManagement.Web.Controllers;

public class PestController : Controller
{
    private readonly IPestService _pestService;
    private readonly ICropService _cropService;

    public PestController(IPestService pestService, ICropService cropService)
    {
        _pestService = pestService;
        _cropService = cropService;
    }

    // GET: /Pest
    public async Task<IActionResult> Index()
    {
        var incidents = await _pestService.GetAllAsync();
        return View(incidents);
    }

    // GET: /Pest/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var incident = await _pestService.GetByIdAsync(id);
        if (incident == null) return NotFound();
        return View(incident);
    }

    // GET: /Pest/Create
    public async Task<IActionResult> Create()
    {
        ViewBag.Crops = await _cropService.GetAllAsync();
        return View(new PestIncident());
    }

    // POST: /Pest/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PestIncident pest)
    {
        ModelState.Remove("Crop");

        if (!ModelState.IsValid)
        {
            ViewBag.Crops = await _cropService.GetAllAsync();
            return View(pest);
        }

        pest.ReportedDate = DateTime.Now;
        pest.Status = IncidentStatus.Active;

        await _pestService.CreateAsync(pest);
        TempData["Success"] = $"Pest incident '{pest.PestName}' reported successfully.";
        return RedirectToAction(nameof(Index));
    }

    // POST: /Pest/UpdateStatus
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, string status, string? treatmentNotes)
    {
        var incident = await _pestService.GetByIdAsync(id);
        if (incident == null) return NotFound();

        await _pestService.UpdateStatusAsync(id, status, treatmentNotes);
        TempData["Success"] = $"Status updated to '{status}' for '{incident.PestName}'.";
        return RedirectToAction(nameof(Index));
    }

    // POST: /Pest/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var incident = await _pestService.GetByIdAsync(id);
        if (incident == null) return NotFound();

        await _pestService.DeleteAsync(id);
        TempData["Success"] = $"Incident '{incident.PestName}' deleted.";
        return RedirectToAction(nameof(Index));
    }
}