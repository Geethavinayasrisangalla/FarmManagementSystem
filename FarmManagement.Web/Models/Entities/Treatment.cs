namespace FarmManagement.Web.Models.Entities;

public class Treatment
{
    public int TreatmentId { get; set; }
    public int PestIncidentId { get; set; }
    public string TreatmentType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime AppliedDate { get; set; } = DateTime.Now;
    public string? Outcome { get; set; }

    public PestIncident PestIncident { get; set; } = null!;
}