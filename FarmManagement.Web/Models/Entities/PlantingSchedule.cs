namespace FarmManagement.Web.Models.Entities;

public class PlantingSchedule
{
    public int ScheduleId { get; set; }            // fixed: was PlantingScheduleId
    public int CropId { get; set; }
    public int FieldId { get; set; }               // added: FK to Field
    public DateTime ScheduledDate { get; set; }
    public decimal ExpectedYieldKg { get; set; }
    public string Status { get; set; } = "Scheduled";
    public string? Notes { get; set; }

    // ── Navigation Properties ─────────────────────────────────────
    public Crop Crop { get; set; } = null!;
    public Field Field { get; set; } = null!;                              // added
    public ICollection<Harvest> Harvests { get; set; } = new List<Harvest>();          // fixed: was singular Harvest?
    public ICollection<ResourceUsage> ResourceUsages { get; set; } = new List<ResourceUsage>(); // added
}