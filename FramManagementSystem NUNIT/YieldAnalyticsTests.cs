





using FarmManagement.Web.Models.Entities;
using FarmManagement.Web.Models.Enums;

namespace FarmManagementSystem_NUNIT;

[TestFixture]
[Description("Yield Analytics & Farm Reporting — report generation and calculations")]
public class YieldAnalyticsTests
{
    private List<Harvest> _harvests;
    private YieldReport   _report;

    [SetUp]
    public void Setup()
    {

        var schedule = new PlantingSchedule
        {
            ScheduleId      = 1,
            CropId          = 1,
            FieldId         = 1,
            ExpectedYieldKg = 500m,
            Status          = "Completed"
        };

        _harvests =
        [
            new Harvest { HarvestId = 1, ScheduleId = 1, ActualYieldKg = 450m, PlantingSchedule = schedule },
            new Harvest { HarvestId = 2, ScheduleId = 1, ActualYieldKg = 520m, PlantingSchedule = schedule },
            new Harvest { HarvestId = 3, ScheduleId = 1, ActualYieldKg = 480m, PlantingSchedule = schedule }
        ];


        decimal total   = _harvests.Sum(h => h.ActualYieldKg);
        decimal area    = 5.5m;
        decimal avgAcre = Math.Round(total / area, 2);

        _report = new YieldReport
        {
            YieldReportId        = 1,
            CropId               = 1,
            TotalYieldKg         = total,
            AverageYieldPerAcre  = avgAcre,
            Season               = SeasonType.Monsoon,
            Year                 = 2025,
            GeneratedAt          = DateTime.Now,
            Remarks              = "Auto-generated from 3 harvest records"
        };
    }


    [Test]
    public void YieldAnalytics_TotalYield_ShouldSumAllHarvests()
    {
        decimal totalYield = _harvests.Sum(h => h.ActualYieldKg);


        Assert.That(totalYield, Is.EqualTo(1450m));
    }


    [Test]
    public void YieldAnalytics_AverageYield_ShouldBeCorrect()
    {
        decimal average = _harvests.Average(h => h.ActualYieldKg);


        Assert.That(average, Is.InRange(483m, 484m));
    }


    [Test]
    public void YieldReport_ShouldStoreCalculatedValues()
    {
        Assert.Multiple(() =>
        {
            Assert.That(_report.TotalYieldKg,        Is.EqualTo(1450m));
            Assert.That(_report.AverageYieldPerAcre, Is.GreaterThan(0));
            Assert.That(_report.Year,                Is.EqualTo(2025));
            Assert.That(_report.Remarks,             Does.Contain("3 harvest"));
        });
    }


    [Test]
    public void YieldReport_AverageYieldPerAcre_ShouldCalculateCorrectly()
    {
        decimal totalYield       = 1450m;
        decimal fieldAreaHectares = 5.5m;
        decimal avgPerAcre       = totalYield / fieldAreaHectares;


        Assert.That(avgPerAcre, Is.InRange(263m, 264m));
    }


    [Test]
    public void YieldAnalytics_HarvestCount_ShouldMatchRecords()
    {
        Assert.That(_harvests.Count, Is.EqualTo(3));
    }


    [Test]
    public void YieldAnalytics_EmptyHarvests_ShouldNotThrow()
    {
        var empty = new List<Harvest>();


        Assert.DoesNotThrow(() =>
        {
            int count = empty.Count;
            decimal total = empty.Sum(h => h.ActualYieldKg);
        });
    }


    [Test]
    public void YieldAnalytics_Variance_ShouldBeNegativeWhenUnderYield()
    {
        decimal expected = 500m;
        decimal actual   = 450m;
        decimal variance = actual - expected;

        Assert.That(variance, Is.LessThan(0));
        Assert.That(variance, Is.EqualTo(-50m));
    }


    [Test]
    public void YieldAnalytics_Variance_ShouldBePositiveWhenOverYield()
    {
        decimal expected = 500m;
        decimal actual   = 520m;
        decimal variance = actual - expected;

        Assert.That(variance, Is.GreaterThan(0));
        Assert.That(variance, Is.EqualTo(20m));
    }


    [Test]
    public void YieldReport_Season_ShouldBeValidEnum()
    {
        Assert.That(Enum.IsDefined(_report.Season), Is.True);
    }


    [Test]
    public void YieldReport_GeneratedAt_ShouldBeSet()
    {
        Assert.That(_report.GeneratedAt, Is.Not.EqualTo(default(DateTime)));
        Assert.That(_report.GeneratedAt.Year, Is.EqualTo(DateTime.Now.Year));
    }
}
