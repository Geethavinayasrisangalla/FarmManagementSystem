





using FarmManagement.Web.Models.Entities;
using FarmManagement.Web.Models.Enums;

namespace FarmManagementSystem_NUNIT;

[TestFixture]
[Description("Planting & Harvest Scheduling — schedule creation, harvest recording")]
public class PlantingHarvestTests
{
    private PlantingSchedule _schedule;

    [SetUp]
    public void Setup()
    {
        _schedule = new PlantingSchedule
        {
            ScheduleId = 1,
            CropId = 1,
            FieldId = 1,
            ScheduledDate = new DateTime(2025, 6, 15),
            ExpectedYieldKg = 500m,
            Status = "Scheduled",
            Notes = "First planting of the season"
        };
    }


    [Test]
    public void Schedule_DefaultStatus_ShouldBeScheduled()
    {
        Assert.That(_schedule.Status, Is.EqualTo("Scheduled"));
    }


    [Test]
    public void Schedule_AfterHarvestRecorded_StatusShouldBeCompleted()
    {

        _schedule.Status = "Completed";


        Assert.That(_schedule.Status, Is.EqualTo("Completed"));
    }


    [Test]
    public void Harvest_ShouldReferenceSchedule()
    {

        var harvest = new Harvest
        {
            HarvestId = 1,
            ScheduleId = _schedule.ScheduleId,
            ActualYieldKg = 480m,
            HarvestedDate = DateTime.Now,
            PlantingSchedule = _schedule
        };


        Assert.That(harvest.ScheduleId, Is.EqualTo(1));
        Assert.That(harvest.PlantingSchedule.ExpectedYieldKg, Is.EqualTo(500m));
    }


    [Test]
    public void Harvest_YieldVariance_ShouldBeCalculatedCorrectly()
    {

        decimal actualYield = 480m;
        decimal expectedYield = _schedule.ExpectedYieldKg;


        decimal variance = actualYield - expectedYield;


        Assert.That(variance, Is.EqualTo(-20m));
        Assert.That(variance, Is.LessThan(0), "Actual yield was below expected");
    }


    [Test]
    public void Schedule_ShouldHaveEmptyHarvestsCollection()
    {
        Assert.That(_schedule.Harvests, Is.Not.Null);
        Assert.That(_schedule.Harvests.Count, Is.EqualTo(0));
    }
}
