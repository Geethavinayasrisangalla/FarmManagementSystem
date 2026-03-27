// ── IPestService.cs ──────────────────────────────────────────
using FarmManagement.Web.Models.Entities;

public interface IPestService
{
    Task<IEnumerable<PestIncident>> GetAllAsync();
    Task<PestIncident?> GetByIdAsync(int id);
    Task<IEnumerable<PestIncident>> GetActivesAsync();
    Task CreateAsync(PestIncident incident);
    Task UpdateStatusAsync(int id, string status, string? treatmentNotes);
    Task DeleteAsync(int id);
}
