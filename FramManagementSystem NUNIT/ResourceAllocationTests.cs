





using FarmManagement.Web.Models.Entities;
using FarmManagement.Web.Models.Enums;

namespace FarmManagementSystem_NUNIT;

[TestFixture]
[Description("Resource Allocation & Usage Tracking — entity and stock logic tests")]
public class ResourceAllocationTests
{
    private Resource _resource;

    [SetUp]
    public void Setup()
    {
        _resource = new Resource
        {
            ResourceId = 1,
            Name = "Urea Fertilizer",
            Type = ResourceType.Fertilizer,
            Quantity = 100m,
            Unit = "kg"
        };
    }


    [Test]
    public void Resource_Creation_ShouldSetProperties()
    {
        Assert.That(_resource.Name, Is.EqualTo("Urea Fertilizer"));
        Assert.That(_resource.Type, Is.EqualTo(ResourceType.Fertilizer));
        Assert.That(_resource.Quantity, Is.EqualTo(100m));
        Assert.That(_resource.Unit, Is.EqualTo("kg"));
    }


    [Test]
    public void Resource_AfterAllocation_StockShouldDecrease()
    {

        decimal allocated = 25m;


        _resource.Quantity -= allocated;


        Assert.That(_resource.Quantity, Is.EqualTo(75m));
    }


    [Test]
    public void ResourceUsage_ShouldReferenceResource()
    {

        var usage = new ResourceUsage
        {
            ResourceUsageId = 1,
            ResourceId = _resource.ResourceId,
            ScheduleId = 10,
            QuantityUsed = 30m,
            Resource = _resource
        };


        Assert.That(usage.ResourceId, Is.EqualTo(1));
        Assert.That(usage.QuantityUsed, Is.EqualTo(30m));
        Assert.That(usage.Resource.Name, Is.EqualTo("Urea Fertilizer"));
    }


    [Test]
    public void Resource_LowStockCheck_ShouldReturnTrue_WhenQuantityLow()
    {

        _resource.Quantity = 8m;


        bool isLowStock = _resource.Quantity <= 10;


        Assert.That(isLowStock, Is.True);
    }


    [Test]
    public void Resource_ShouldHaveEmptyUsagesCollection()
    {
        Assert.That(_resource.ResourceUsages, Is.Not.Null);
        Assert.That(_resource.ResourceUsages.Count, Is.EqualTo(0));
    }
}
