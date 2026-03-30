using FarmManagement.Web.Models.Enums;

namespace FarmManagement.Web.Models.Entities;

public class YieldReport
{
    public int YieldReportId { get; set; }
    public int CropId { get; set; }
    public decimal TotalYieldKg { get; set; }
    public SeasonType Season { get; set; }         // fixed: was int
    public int Year { get; set; }
    public DateTime GeneratedAt { get; set; } = DateTime.Now;

    // ── Navigation Property ───────────────────────────────────────
    public Crop Crop { get; set; } = null!;
}