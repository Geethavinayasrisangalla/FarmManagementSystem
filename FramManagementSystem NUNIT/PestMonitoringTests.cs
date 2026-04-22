// ═══════════════════════════════════════════════════════════════
//  Pest/Disease Monitoring & Treatment Logging Tests
//  Tests that PestIncident entity tracks status transitions
//  and treatment logging correctly.
// ═══════════════════════════════════════════════════════════════

using FarmManagement.Web.Models.Entities;
using FarmManagement.Web.Models.Enums;

namespace FarmManagementSystem_NUNIT;

[TestFixture]
[Description("Pest/Disease Monitoring — incident creation, status updates, treatment")]
public class PestMonitoringTests
{
    private PestIncident _incident;

    [SetUp]
    public void Setup()
    {
        _incident = new PestIncident
        {
            PestIncidentId = 1,
            PestName = "Aphids",
            Description = "Small green insects on rice leaves",
            Status = IncidentStatus.Active,
            CropId = 1
        };
    }

    // ── Test 1: New incident should default to Active status ──
    [Test]
    public void PestIncident_NewIncident_ShouldBeActive()
    {
        Assert.That(_incident.Status, Is.EqualTo(IncidentStatus.Active));
    }

    // ── Test 2: Status can transition from Active → Monitoring ──
    [Test]
    public void PestIncident_StatusChange_ActiveToMonitoring()
    {
        // Act
        _incident.Status = IncidentStatus.Monitoring;

        // Assert
        Assert.That(_incident.Status, Is.EqualTo(IncidentStatus.Monitoring));
    }

    // ── Test 3: Status can transition from Monitoring → Resolved ──
    [Test]
    public void PestIncident_StatusChange_MonitoringToResolved()
    {
        // Act
        _incident.Status = IncidentStatus.Monitoring;
        _incident.Status = IncidentStatus.Resolved;

        // Assert
        Assert.That(_incident.Status, Is.EqualTo(IncidentStatus.Resolved));
    }

    // ── Test 4: Treatment notes should be updatable ──
    [Test]
    public void PestIncident_TreatmentNotes_ShouldBeUpdatable()
    {
        // Arrange — initially no treatment
        Assert.That(_incident.TreatmentNotes, Is.Null);

        // Act — log treatment
        _incident.TreatmentNotes = "Applied neem oil spray on 15 Jun 2025";

        // Assert
        Assert.That(_incident.TreatmentNotes, Is.Not.Null);
        Assert.That(_incident.TreatmentNotes, Does.Contain("neem oil"));
    }

    // ── Test 5: Incident should belong to a Crop (via CropId) ──
    [Test]
    public void PestIncident_ShouldHaveCropId()
    {
        Assert.That(_incident.CropId, Is.GreaterThan(0));
        Assert.That(_incident.CropId, Is.EqualTo(1));
    }
}
