





using FarmManagement.Web.Models.Entities;
using FarmManagement.Web.Models.Enums;

namespace FarmManagementSystem_NUNIT;

[TestFixture]
[Description("Crop & Field Management — entity creation and relationship tests")]
public class CropFieldTests
{
    private Field _field;
    private Crop _crop;


    [SetUp]
    public void Setup()
    {
        _field = new Field
        {
            FieldId = 1,
            FieldName = "North Paddy",
            AreaHectares = 5.5m,
            SoilType = "Alluvial",
            Location = "Block A"
        };

        _crop = new Crop
        {
            CropId = 1,
            CropName = "Rice",
            CropType = "Cereal",
            Season = SeasonType.Monsoon,
            PlantingDate = new DateTime(2025, 6, 1),
            ExpectedHarvestDate = new DateTime(2025, 10, 15),
            Status = "Growing",
            FieldId = _field.FieldId,
            Field = _field
        };
    }


    [Test]
    public void Field_Creation_ShouldSetPropertiesCorrectly()
    {

        Assert.That(_field.FieldName, Is.EqualTo("North Paddy"));
        Assert.That(_field.AreaHectares, Is.EqualTo(5.5m));
        Assert.That(_field.SoilType, Is.EqualTo("Alluvial"));
    }


    [Test]
    public void Crop_Creation_ShouldLinkToField()
    {

        Assert.That(_crop.CropName, Is.EqualTo("Rice"));
        Assert.That(_crop.FieldId, Is.EqualTo(1));
        Assert.That(_crop.Field, Is.Not.Null);
        Assert.That(_crop.Field.FieldName, Is.EqualTo("North Paddy"));
    }


    [Test]
    public void Field_ShouldHaveEmptyCropsCollection_ByDefault()
    {

        Assert.That(_field.Crops, Is.Not.Null);
        Assert.That(_field.Crops.Count, Is.EqualTo(0));
    }


    [Test]
    public void Field_AddCrop_ShouldIncreaseCropCount()
    {

        _field.Crops.Add(_crop);


        Assert.That(_field.Crops.Count, Is.EqualTo(1));
        Assert.That(_field.Crops.First().CropName, Is.EqualTo("Rice"));
    }


    [Test]
    public void Crop_Season_ShouldBeValidEnum()
    {

        Assert.That(_crop.Season, Is.EqualTo(SeasonType.Monsoon));
        Assert.That(Enum.IsDefined(typeof(SeasonType), _crop.Season), Is.True);
    }
}
