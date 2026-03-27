namespace FarmManagement.Web.Models.Entities;

public class ResourceUsage
{
    public int ResourceUsageId { get; set; }
    public int ResourceId { get; set; }
    public int FieldId { get; set; }
    public decimal QuantityUsed { get; set; }
    public DateTime UsedDate { get; set; } = DateTime.Now;
    public string? Notes { get; set; }

    public Resource Resource { get; set; } = null!;
    public Field Field { get; set; } = null!;
}
