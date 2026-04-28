using FarmManagement.Web.Data;
using FarmManagement.Web.Models.Entities;
using FarmManagement.Web.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FarmManagement.PestResourceService.Controllers;

[ApiController]
[Route("api/resources")]
[Authorize]
public class ResourcesController : ControllerBase
{
    private readonly FarmDbContext _db;
    public ResourcesController(FarmDbContext db) => _db = db;


    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool includePesticides = false)
    {
        var query = _db.Resources.AsNoTracking();
        if (!includePesticides)
            query = query.Where(r => r.Type != ResourceType.Pesticide);
        return Ok(await query.OrderBy(r => r.Name).ToListAsync());
    }


    [HttpGet("low-stock")]
    public async Task<IActionResult> GetLowStock([FromQuery] decimal threshold = 10)
        => Ok(await _db.Resources.Where(r => r.Quantity <= threshold).AsNoTracking().ToListAsync());


    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var resource = await _db.Resources.Include(r => r.ResourceUsages)
                                          .ThenInclude(ru => ru.PlantingSchedule)
                                          .AsNoTracking()
                                          .FirstOrDefaultAsync(r => r.ResourceId == id);
        return resource is null ? NotFound(new { message = $"Resource {id} not found." }) : Ok(resource);
    }


    [HttpPost]
    [Authorize(Roles = "Admin,Storekeeper")]
    public async Task<IActionResult> Create([FromBody] ResourceRequest req)
    {
        var resource = new Resource
        {
            Name        = req.Name,
            Type        = req.Type,
            Quantity    = req.Quantity,
            Unit        = req.Unit,
            LastUpdated = DateTime.Now
        };
        _db.Resources.Add(resource);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = resource.ResourceId }, resource);
    }


    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Storekeeper")]
    public async Task<IActionResult> Update(int id, [FromBody] ResourceRequest req)
    {
        var resource = await _db.Resources.FindAsync(id);
        if (resource is null) return NotFound(new { message = $"Resource {id} not found." });

        resource.Name        = req.Name;
        resource.Type        = req.Type;
        resource.Quantity    = req.Quantity;
        resource.Unit        = req.Unit;
        resource.LastUpdated = DateTime.Now;
        await _db.SaveChangesAsync();
        return Ok(resource);
    }


    [HttpPost("{id:int}/allocate")]
    [Authorize(Roles = "Admin,Farmer,Storekeeper")]
    public async Task<IActionResult> Allocate(int id, [FromBody] AllocateRequest req)
    {
        var resource = await _db.Resources.FindAsync(id);
        if (resource is null) return NotFound(new { message = $"Resource {id} not found." });
        if (resource.Quantity < req.Quantity)
            return BadRequest(new { message = $"Insufficient stock. Available: {resource.Quantity} {resource.Unit}" });

        resource.Quantity   -= req.Quantity;
        resource.LastUpdated = DateTime.Now;

        _db.ResourceUsages.Add(new ResourceUsage
        {
            ResourceId   = id,
            ScheduleId   = req.ScheduleId,
            QuantityUsed = req.Quantity,
            Notes        = req.Notes,
            UsedDate     = DateTime.Now
        });
        await _db.SaveChangesAsync();
        return Ok(new { message = "Allocated successfully.", remainingQuantity = resource.Quantity });
    }


    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin,Storekeeper")]
    public async Task<IActionResult> Delete(int id)
    {
        var resource = await _db.Resources.Include(r => r.ResourceUsages).FirstOrDefaultAsync(r => r.ResourceId == id);
        if (resource is null) return NotFound(new { message = $"Resource {id} not found." });
        _db.ResourceUsages.RemoveRange(resource.ResourceUsages);
        _db.Resources.Remove(resource);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}

public record ResourceRequest(string Name, ResourceType Type, decimal Quantity, string Unit);
public record AllocateRequest(int ScheduleId, decimal Quantity, string? Notes);
