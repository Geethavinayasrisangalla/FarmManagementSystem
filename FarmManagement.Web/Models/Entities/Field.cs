namespace FarmManagement.Web.Models.Entities;

public class Field
{
    public int FieldId { get; set; }
    public string FieldName { get; set; } = string.Empty;
    public decimal AreaHectares { get; set; }      // fixed: double → decimal
    public string SoilType { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // ── Navigation Properties ─────────────────────────────────────
    public ICollection<Crop> Crops { get; set; } = new List<Crop>();
    public ICollection<PlantingSchedule> PlantingSchedules { get; set; } = new List<PlantingSchedule>(); // added
    public ICollection<ResourceUsage> ResourceUsages { get; set; } = new List<ResourceUsage>();
}