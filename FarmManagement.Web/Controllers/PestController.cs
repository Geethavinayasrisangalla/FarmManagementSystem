using FarmManagement.Web.Events;
using FarmManagement.Web.Models.Entities;
using FarmManagement.Web.Models.Enums;
using FarmManagement.Web.Models.Interfaces;
using FarmManagement.Web.States;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FarmManagement.Web.Controllers;

// Patterns used:
//   State    — PestStateMachine enforces valid status transitions (Active→Monitoring→Resolved)
//   Observer — IEventDispatcher replaces direct IActivityService calls
//   Facade   — IFarmFacade handles Delete (service + event in one call)
[Authorize(Roles = "Admin,Farmer,FieldSupervisor,Storekeeper")]
public class PestController : Controller
{
    private readonly IPestService      _pestService;
    private readonly ICropService      _cropService;
    private readonly IEventDispatcher  _dispatcher;
    private readonly IFarmFacade       _facade;

    public PestController(IPestService pestService, ICropService cropService,
                          IEventDispatcher dispatcher, IFarmFacade facade)
    {
        _pestService = pestService;
        _cropService = cropService;
        _dispatcher  = dispatcher;
        _facade      = facade;
    }

    private int    CurrentUserId   => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
    private string CurrentUserName => User.FindFirstValue(ClaimTypes.Name) ?? "Unknown";
    private string CurrentUserRole => User.FindFirstValue(ClaimTypes.Role) ?? "Unknown";

    public async Task<IActionResult> Index(string? search)
    {
        var incidents = await _pestService.GetAllAsync();
        if (!string.IsNullOrWhiteSpace(search))
        {
            incidents = incidents.Where(p => p.PestName.Contains(search, StringComparison.OrdinalIgnoreCase)
                                          || (p.Crop?.CropName ?? "").Contains(search, StringComparison.OrdinalIgnoreCase));
        }
        ViewBag.Search = search;
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

        // Observer Pattern
        await _dispatcher.DispatchAsync(new PestReportedEvent(
            CurrentUserId, CurrentUserName, CurrentUserRole, pest.PestName));

        TempData["Success"] = $"Pest incident '{pest.PestName}' reported successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, string status, string? treatmentNotes)
    {
        var incident = await _pestService.GetByIdAsync(id);
        if (incident == null) return NotFound();

        try
        {
            // State Pattern — PestService uses PestStateMachine to validate the transition
            await _pestService.UpdateStatusAsync(id, status, treatmentNotes);

            // Observer Pattern
            await _dispatcher.DispatchAsync(new PestStatusUpdatedEvent(
                CurrentUserId, CurrentUserName, CurrentUserRole,
                incident.PestName, status));
            TempData["Success"] = $"Status updated to '{status}' for '{incident.PestName}'.";
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
        var incident = await _pestService.GetByIdAsync(id);
        if (incident == null) return NotFound();

        // Facade Pattern — single call coordinates PestService + event dispatch
        await _facade.DeletePestAsync(
            id, incident.PestName,
            CurrentUserId, CurrentUserName, CurrentUserRole);

        TempData["Success"] = $"Incident '{incident.PestName}' deleted.";
        return RedirectToAction(nameof(Index));
    }
}
