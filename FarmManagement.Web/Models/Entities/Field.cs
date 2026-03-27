namespace FarmManagement.Web.Models.Entities;

public class Field
{
    public int FieldId { get; set; }
    public string FieldName { get; set; } = string.Empty;
    public double AreaHectares { get; set; }
    public string SoilType { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public ICollection<Crop> Crops { get; set; } = new List<Crop>();
    public ICollection<ResourceUsage> ResourceUsages { get; set; } = new List<ResourceUsage>();
}
