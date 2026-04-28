using FarmManagement.Web.Data;
using FarmManagement.Web.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FarmManagement.CropFieldService.Controllers;

[ApiController]
[Route("api/crops")]
[Authorize]
public class CropsController : ControllerBase
{
    private readonly FarmDbContext _db;
    public CropsController(FarmDbContext db) => _db = db;


    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? search)
    {
        var query = _db.Crops.Include(c => c.Field).AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(c => c.CropName.Contains(search) || c.CropType.Contains(search));
        return Ok(await query.OrderByDescending(c => c.CropId).ToListAsync());
    }


    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var crop = await _db.Crops.Include(c => c.Field)
                                  .Include(c => c.PestIncidents)
                                  .Include(c => c.PlantingSchedules)
                                  .AsNoTracking()
                                  .FirstOrDefaultAsync(c => c.CropId == id);
        return crop is null ? NotFound(new { message = $"Crop {id} not found." }) : Ok(crop);
    }


    [HttpPost]
    [Authorize(Roles = "Admin,Farmer,FieldSupervisor")]
    public async Task<IActionResult> Create([FromBody] CropRequest req)
    {
        if (req.PlantingDate >= req.ExpectedHarvestDate)
            return BadRequest(new { message = "Planting date must be before expected harvest date." });

        var crop = new Crop
        {
            CropName            = req.CropName,
            CropType            = req.CropType,
            Season              = req.Season,
            PlantingDate        = req.PlantingDate,
            ExpectedHarvestDate = req.ExpectedHarvestDate,
            FieldId             = req.FieldId,
            Status              = "Growing"
        };
        _db.Crops.Add(crop);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = crop.CropId }, crop);
    }


    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Farmer,FieldSupervisor")]
    public async Task<IActionResult> Update(int id, [FromBody] CropRequest req)
    {
        var crop = await _db.Crops.FindAsync(id);
        if (crop is null) return NotFound(new { message = $"Crop {id} not found." });

        crop.CropName            = req.CropName;
        crop.CropType            = req.CropType;
        crop.Season              = req.Season;
        crop.PlantingDate        = req.PlantingDate;
        crop.ExpectedHarvestDate = req.ExpectedHarvestDate;
        crop.FieldId             = req.FieldId;
        await _db.SaveChangesAsync();
        return Ok(crop);
    }


    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin,Farmer")]
    public async Task<IActionResult> Delete(int id)
    {
        var crop = await _db.Crops.FindAsync(id);
        if (crop is null) return NotFound(new { message = $"Crop {id} not found." });

        var scheduleIds = await _db.PlantingSchedules.Where(ps => ps.CropId == id).Select(ps => ps.ScheduleId).ToListAsync();
        if (scheduleIds.Count > 0)
        {
            _db.ResourceUsages.RemoveRange(_db.ResourceUsages.Where(r => scheduleIds.Contains(r.ScheduleId)));
            _db.Harvests.RemoveRange(_db.Harvests.Where(h => scheduleIds.Contains(h.ScheduleId)));
            _db.PlantingSchedules.RemoveRange(_db.PlantingSchedules.Where(ps => scheduleIds.Contains(ps.ScheduleId)));
        }
        _db.PestIncidents.RemoveRange(_db.PestIncidents.Where(p => p.CropId == id));
        _db.YieldReports.RemoveRange(_db.YieldReports.Where(y => y.CropId == id));
        _db.Crops.Remove(crop);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}

public record CropRequest(
    string CropName, string CropType,
    FarmManagement.Web.Models.Enums.SeasonType Season,
    DateTime PlantingDate, DateTime ExpectedHarvestDate, int FieldId);
