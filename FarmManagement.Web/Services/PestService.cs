using FarmManagement.Web.Data;
using FarmManagement.Web.Models.Entities;
using FarmManagement.Web.Models.Enums;
using FarmManagement.Web.Models.Interfaces;
using FarmManagement.Web.States;
using Microsoft.EntityFrameworkCore;

namespace FarmManagement.Web.Services;

// State Pattern — UpdateStatusAsync now delegates all transition logic to PestStateMachine.
// Invalid transitions (e.g. Active → Resolved) throw InvalidOperationException automatically.
public class PestService : IPestService
{
    private readonly FarmDbContext _db;
    public PestService(FarmDbContext db) => _db = db;

    public async Task<IEnumerable<PestIncident>> GetAllAsync() =>
        await _db.PestIncidents.AsNoTracking()
                               .Include(p => p.Crop)
                               .OrderByDescending(p => p.ReportedDate)
                               .ToListAsync();

    public async Task<PestIncident?> GetByIdAsync(int id) =>
        await _db.PestIncidents.AsNoTracking()
                               .Include(p => p.Crop)
                               .FirstOrDefaultAsync(p => p.PestIncidentId == id);

    public async Task<IEnumerable<PestIncident>> GetActivesAsync() =>
        await _db.PestIncidents.AsNoTracking()
                               .Include(p => p.Crop)
                               .Where(p => p.Status == IncidentStatus.Active)
                               .ToListAsync();

    public async Task CreateAsync(PestIncident incident)
    {
        _db.PestIncidents.Add(incident);
        await _db.SaveChangesAsync();
    }

    // State Pattern — PestStateMachine enforces valid transitions and throws on invalid ones
    public async Task UpdateStatusAsync(int id, string status, string? treatmentNotes)
    {
        var pest = await _db.PestIncidents.FindAsync(id)
                   ?? throw new KeyNotFoundException("Incident not found.");

        // Delegates transition logic to the State Machine
        PestStateMachine.Transition(pest, status);

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
