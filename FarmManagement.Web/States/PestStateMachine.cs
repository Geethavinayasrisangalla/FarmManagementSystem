using FarmManagement.Web.Models.Entities;
using FarmManagement.Web.Models.Enums;

namespace FarmManagement.Web.States;

// State Pattern — resolves the correct state object and executes the transition
public static class PestStateMachine
{
    public static IPestIncidentState GetState(IncidentStatus status) => status switch
    {
        IncidentStatus.Active     => new ActivePestState(),
        IncidentStatus.Monitoring => new MonitoringPestState(),
        IncidentStatus.Resolved   => new ResolvedPestState(),
        _ => throw new ArgumentOutOfRangeException(nameof(status), $"Unknown status: {status}")
    };

    // Perform a validated transition — throws if the transition is not allowed
    public static PestIncident Transition(PestIncident incident, string targetStatus)
    {
        var currentState = GetState(incident.Status);
        return currentState.Transition(incident, targetStatus);
    }
}
