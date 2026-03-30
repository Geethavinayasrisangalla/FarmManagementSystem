namespace FarmManagement.Web.Models.Enums;

public enum IncidentStatus
{
    Active = 0,
    Monitoring = 1,    // fixed: was UnderTreatment
    Resolved = 2
}