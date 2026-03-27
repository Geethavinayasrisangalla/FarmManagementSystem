using FarmManagement.Web.Data;
using FarmManagement.Web.Models.Entities;
using FarmManagement.Web.Models.Enums;

namespace FarmManagement.Web.Services
{
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
                               .Include(p => p.Treatments)
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
        {
            _db.Treatments.Add(new Treatment
            {
                PestIncidentId = id,
                TreatmentType = "Manual Update",
                Description = treatmentNotes,
                AppliedDate = DateTime.Now
            });
        }
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var p = await _db.PestIncidents.FindAsync(id);
        if (p != null) { _db.PestIncidents.Remove(p); await _db.SaveChangesAsync(); }
    }
    }
}
