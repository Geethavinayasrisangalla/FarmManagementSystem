using FarmManagement.Web.Models.Enums;

namespace FarmManagement.Web.Models.Entities;

public class PestIncident
{
    public int PestIncidentId { get; set; }
    public string PestName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public IncidentStatus Status { get; set; } = IncidentStatus.Active;
    public DateTime ReportedDate { get; set; } = DateTime.Now;
    public string? TreatmentNotes { get; set; }    // added: used in controller + DbInitializer
    public int CropId { get; set; }

    // ── Navigation Property ───────────────────────────────────────
    public Crop Crop { get; set; } = null!;

    // Removed: ICollection<Treatment> — Treatment entity doesn't exist
    // in your solution. Add it back only if you create Treatment.cs
}