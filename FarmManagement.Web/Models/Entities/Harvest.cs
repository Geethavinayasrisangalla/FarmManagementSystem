namespace FarmManagement.Web.Models.Entities;

public class Harvest
{
    public int HarvestId { get; set; }
    public int PlantingScheduleId { get; set; }
    public DateTime HarvestedDate { get; set; } = DateTime.Now;
    public decimal ActualYieldKg { get; set; }
    public string? QualityNotes { get; set; }

    public PlantingSchedule PlantingSchedule { get; set; } = null!;
}