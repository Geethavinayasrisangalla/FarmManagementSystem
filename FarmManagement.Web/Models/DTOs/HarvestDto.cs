namespace FarmManagement.Web.Models.DTOs;

public class HarvestDto
{
    public int HarvestId { get; set; }
    public DateTime HarvestedDate { get; set; }
    public decimal ActualYieldKg { get; set; }
    public string? Notes { get; set; }


    public int ScheduleId { get; set; }


    public string? CropName { get; set; }
    public string? CropType { get; set; }


    public string? FieldName { get; set; }
}