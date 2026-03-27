namespace FarmManagement.Web.Models.Entities;

public class PlantingSchedule
{
    public int PlantingScheduleId { get; set; }
    public int CropId { get; set; }
    public DateTime ScheduledDate { get; set; }
    public decimal ExpectedYieldKg { get; set; }
    public string Status { get; set; } = "Scheduled";
    public string? Notes { get; set; }

    public Crop Crop { get; set; } = null!;
    public Harvest? Harvest { get; set; }
}
