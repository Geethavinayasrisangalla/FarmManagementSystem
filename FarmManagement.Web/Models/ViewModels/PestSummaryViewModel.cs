using FarmManagement.Web.Models.Entities;

namespace FarmManagement.Web.Models.ViewModels;

public class PestSummaryViewModel
{
    public int TotalIncidents { get; set; }
    public int ActiveCount { get; set; }
    public int MonitoringCount { get; set; }
    public int ResolvedCount { get; set; }
    public IEnumerable<PestIncident> Incidents { get; set; } = new List<PestIncident>();
}
