using FarmManagement.Web.Models.Entities;
using FarmManagement.Web.Models.Enums;
using FarmManagement.Web.Models.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FarmManagement.Web.Controllers;

[Authorize(Roles = "Admin,Manager,Supervisor,Worker")]
public class PestController : Controller
{
    private readonly IPestService     _pestService;
    private readonly ICropService     _cropService;
    private readonly IActivityService _activityService;

    public PestController(IPestService pestService, ICropService cropService, IActivityService activityService)
    {
        _pestService     = pestService;
        _cropService     = cropService;
        _activityService = activityService;
    }

    private int    CurrentUserId   => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
    private string CurrentUserName => User.FindFirstValue(ClaimTypes.Name) ?? "Unknown";
    private string CurrentUserRole => User.FindFirstValue(ClaimTypes.Role) ?? "Unknown";

    public async Task<IActionResult> Index()
    {
        var incidents = await _pestService.GetAllAsync();
        return View(incidents);
    }

    public async Task<IActionResult> Details(int id)
    {
        var incident = await _pestService.GetByIdAsync(id);
        if (incident == null) return NotFound();
        return View(incident);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Crops = await _cropService.GetAllAsync();
        return View(new PestIncident());
    }

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
        pest.Status       = IncidentStatus.Active;

        await _pestService.CreateAsync(pest);

        await _activityService.LogAsync(CurrentUserId, CurrentUserName, CurrentUserRole,
            "Reported", "Pest", $"Logged pest incident: '{pest.PestName}'");

        TempData["Success"] = $"Pest incident '{pest.PestName}' reported successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, string status, string? treatmentNotes)
    {
        var incident = await _pestService.GetByIdAsync(id);
        if (incident == null) return NotFound();

        await _pestService.UpdateStatusAsync(id, status, treatmentNotes);

        await _activityService.LogAsync(CurrentUserId, CurrentUserName, CurrentUserRole,
            "Updated", "Pest", $"Updated '{incident.PestName}' status to {status}");

        TempData["Success"] = $"Status updated to '{status}' for '{incident.PestName}'.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var incident = await _pestService.GetByIdAsync(id);
        if (incident == null) return NotFound();

        await _pestService.DeleteAsync(id);

        await _activityService.LogAsync(CurrentUserId, CurrentUserName, CurrentUserRole,
            "Deleted", "Pest", $"Deleted pest incident: '{incident.PestName}'");

        TempData["Success"] = $"Incident '{incident.PestName}' deleted.";
        return RedirectToAction(nameof(Index));
    }
}
