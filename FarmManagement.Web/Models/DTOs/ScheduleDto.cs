namespace FarmManagement.Web.Models.DTOs;

public class ScheduleDto
{
    public int ScheduleId { get; set; }
    public DateTime ScheduledDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }


    public int CropId { get; set; }
    public string? CropName { get; set; }
    public string? CropType { get; set; }


    public int FieldId { get; set; }
    public string? FieldName { get; set; }


    public bool IsUpcoming => ScheduledDate >= DateTime.Today
                           && ScheduledDate <= DateTime.Today.AddDays(30)
                           && Status == "Scheduled";
}