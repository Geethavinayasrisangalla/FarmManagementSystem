namespace FarmManagement.Web.Models.Entities;

public class Harvest
{
    public int HarvestId { get; set; }
    public int ScheduleId { get; set; }            // fixed: was PlantingScheduleId
    public DateTime HarvestedDate { get; set; } = DateTime.Now;
    public decimal ActualYieldKg { get; set; }
    public string? Notes { get; set; }             // fixed: was QualityNotes

    // ── Navigation Property ───────────────────────────────────────
    public PlantingSchedule PlantingSchedule { get; set; } = null!;
}