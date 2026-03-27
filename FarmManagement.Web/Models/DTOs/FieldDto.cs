namespace FarmManagement.Models.DTOs;

//Used for summary displays and dropdown population.

public class FieldDto
{
    public int FieldId { get; set; }
    public string FieldName { get; set; } = string.Empty;
    public double AreaHectares { get; set; }
    public string SoilType { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public int TotalCrops { get; set; }
}