using Microsoft.AspNetCore.Mvc;
using FarmManagement.Web.Models.Entities;
using FarmManagement.Web.Data;
using Microsoft.EntityFrameworkCore;
using FarmManagement.Web.Models.Entities;

public class ScheduleController : Controller
{
    private readonly FarmDbContext _context;

    public ScheduleController(FarmDbContext context)
    {
        _context = context;
    }

    // GET: List all active plantings for the Supervisor
    public async Task<IActionResult> Index()
    {
        var schedules = await _context.PlantingSchedules
            .Include(s => s.Crop)
            .Include(s => s.Field)
            .Where(s => s.Status == "Active")
            .ToListAsync();
        return View(schedules);
    }

    // POST: CreatePlanting
    [HttpPost]
    public async Task<IActionResult> CreatePlanting(PlantingSchedule schedule)
    {
        // WHY: Check if the field is already occupied (Prevents double-planting)
        var isOccupied = await _context.PlantingSchedules
            .AnyAsync(s => s.FieldId == schedule.FieldId && s.Status == "Active");

        if (isOccupied)
        {
            ModelState.AddModelError("", "Error: This field already has an active crop!");
            return View(schedule);
        }

        schedule.Status = "Active";
        _context.Add(schedule);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // POST: RecordHarvest
    [HttpPost]
    public async Task<IActionResult> RecordHarvest(Harvest harvest)
    {
        // WHY: Save the production data for Member 5's Analytics
        _context.Harvests.Add(harvest);

        // WHY: Update the Schedule status to "Completed" to free up the field
        var schedule = await _context.PlantingSchedules
            .FirstOrDefaultAsync(s => s.FieldId == harvest.FieldId && s.Status == "Active");

        if (schedule != null)
        {
            schedule.Status = "Completed";
            _context.Update(schedule);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction("Index", "Report"); // Send to Member 5's view
    }
}