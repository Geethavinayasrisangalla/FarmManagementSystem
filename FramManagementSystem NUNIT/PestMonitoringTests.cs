





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


    [Test]
    public void PestIncident_NewIncident_ShouldBeActive()
    {
        Assert.That(_incident.Status, Is.EqualTo(IncidentStatus.Active));
    }


    [Test]
    public void PestIncident_StatusChange_ActiveToMonitoring()
    {

        _incident.Status = IncidentStatus.Monitoring;


        Assert.That(_incident.Status, Is.EqualTo(IncidentStatus.Monitoring));
    }


    [Test]
    public void PestIncident_StatusChange_MonitoringToResolved()
    {

        _incident.Status = IncidentStatus.Monitoring;
        _incident.Status = IncidentStatus.Resolved;


        Assert.That(_incident.Status, Is.EqualTo(IncidentStatus.Resolved));
    }


    [Test]
    public void PestIncident_DiseaseName_ShouldBeUpdatable()
    {

        Assert.That(_incident.DiseaseName, Is.Null);


        _incident.DiseaseName = "Leaf Blight";


        Assert.That(_incident.DiseaseName, Is.Not.Null);
        Assert.That(_incident.DiseaseName, Does.Contain("Blight"));
    }


    [Test]
    public void PestIncident_ShouldHaveCropId()
    {
        Assert.That(_incident.CropId, Is.GreaterThan(0));
        Assert.That(_incident.CropId, Is.EqualTo(1));
    }
}
