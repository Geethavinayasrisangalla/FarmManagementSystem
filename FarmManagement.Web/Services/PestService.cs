using FarmManagement.Web.Data;
using FarmManagement.Web.Models.Entities;
using FarmManagement.Web.Models.Enums;
using FarmManagement.Web.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FarmManagement.Web.Services;

public class PestService : IPestService
{
    private readonly FarmDbContext _db;
    public PestService(FarmDbContext db) => _db = db;

    public async Task<IEnumerable<PestIncident>> GetAllAsync() =>
        await _db.PestIncidents.Include(p => p.Crop)
                               .OrderByDescending(p => p.ReportedDate)
                               .ToListAsync();

    public async Task<PestIncident?> GetByIdAsync(int id) =>
        await _db.PestIncidents.Include(p => p.Crop)
                               .FirstOrDefaultAsync(p => p.PestIncidentId == id);

    public async Task<IEnumerable<PestIncident>> GetActivesAsync() =>
        await _db.PestIncidents.Include(p => p.Crop)
                               .Where(p => p.Status == IncidentStatus.Active)
                               .ToListAsync();

    public async Task CreateAsync(PestIncident incident)
    {
        _db.PestIncidents.Add(incident);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateStatusAsync(int id, string status, string? treatmentNotes)
    {
        var pest = await _db.PestIncidents.FindAsync(id)
                   ?? throw new KeyNotFoundException("Incident not found.");

        pest.Status = Enum.Parse<IncidentStatus>(status);

        if (!string.IsNullOrWhiteSpace(treatmentNotes))
            pest.TreatmentNotes = treatmentNotes;

        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var pest = await _db.PestIncidents.FindAsync(id);
        if (pest != null) { _db.PestIncidents.Remove(pest); await _db.SaveChangesAsync(); }
    }
}
