using FarmManagement.Web.Models.Entities;

namespace FarmManagement.Web.States;

// State Pattern — defines the contract every pest-status state must fulfil
public interface IPestIncidentState
{
    string StatusName { get; }

    // Attempt a transition; returns the updated incident or throws if invalid
    PestIncident Transition(PestIncident incident, string targetStatus);
}
