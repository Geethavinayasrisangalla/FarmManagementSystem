namespace FarmManagement.Web.Models.Entities;

public class YieldReport
{
    public int YieldReportId { get; set; }
    public int CropId { get; set; }
    public decimal TotalYieldKg { get; set; }
    public int Season { get; set; }
    public int Year { get; set; }
    public DateTime GeneratedAt { get; set; } = DateTime.Now;

    public Crop Crop { get; set; } = null!;
}