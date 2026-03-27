
// ── Place each class in its own file under Models/DTOs/ ──────

namespace FarmManagement.Models.DTOs;

// ── CropDto.cs ───────────────────────────────────────────────
// Used for lightweight data transfer (e.g. API responses,
// dropdown lists, summary cards) without loading navigations.
public class CropDto
{
    public int CropId { get; set; }
    public string CropName { get; set; } = string.Empty;
    public string CropType { get; set; } = string.Empty;
    public string Season { get; set; } = string.Empty;
    public string PlantingDate { get; set; } = string.Empty;
    public string ExpectedHarvestDate { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string FieldName { get; set; } = string.Empty;
    public int FieldId { get; set; }
}