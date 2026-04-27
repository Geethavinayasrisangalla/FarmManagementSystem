using FarmManagement.Web.Data;
using FarmManagement.Web.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FarmManagement.CropFieldService.Controllers;

[ApiController]
[Route("api/fields")]
[Authorize]
public class FieldsController : ControllerBase
{
    private readonly FarmDbContext _db;
    public FieldsController(FarmDbContext db) => _db = db;

    /// <summary>Get all fields.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _db.Fields.Include(f => f.Crops).AsNoTracking().OrderBy(f => f.FieldName).ToListAsync());

    /// <summary>Get a field by ID.</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var field = await _db.Fields.Include(f => f.Crops).AsNoTracking().FirstOrDefaultAsync(f => f.FieldId == id);
        return field is null ? NotFound(new { message = $"Field {id} not found." }) : Ok(field);
    }

    /// <summary>Create a new field.</summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Farmer,FieldSupervisor")]
    public async Task<IActionResult> Create([FromBody] FieldRequest req)
    {
        var field = new Field
        {
            FieldName    = req.FieldName,
            AreaHectares = req.AreaHectares,
            SoilType     = req.SoilType,
            Location     = req.Location,
            CreatedAt    = DateTime.Now
        };
        _db.Fields.Add(field);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = field.FieldId }, field);
    }

    /// <summary>Update a field.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Farmer,FieldSupervisor")]
    public async Task<IActionResult> Update(int id, [FromBody] FieldRequest req)
    {
        var field = await _db.Fields.FindAsync(id);
        if (field is null) return NotFound(new { message = $"Field {id} not found." });

        field.FieldName    = req.FieldName;
        field.AreaHectares = req.AreaHectares;
        field.SoilType     = req.SoilType;
        field.Location     = req.Location;
        await _db.SaveChangesAsync();
        return Ok(field);
    }

    /// <summary>Delete a field and its crops.</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin,Farmer")]
    public async Task<IActionResult> Delete(int id)
    {
        var field = await _db.Fields.Include(f => f.Crops).FirstOrDefaultAsync(f => f.FieldId == id);
        if (field is null) return NotFound(new { message = $"Field {id} not found." });

        _db.Crops.RemoveRange(field.Crops);
        _db.Fields.Remove(field);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}

public record FieldRequest(string FieldName, decimal AreaHectares, string SoilType, string Location);
