using HeatManager.Core.Services.AssetManagers;
using JetBrains.Annotations;
using Shouldly;

namespace HeatManager.Core.Tests.Services;

[TestSubject(typeof(AssetManager))]
public class AssetManagerTest
{
    [Fact]
    public async Task LoadUnits_Should_Deserialize_Correct_ProductionUnits()
    {
        // Arrange
        var service = new AssetManager();

        // Act
        service.LoadUnits("./Services/AssetManagerTest_Valid.json");

        // Validate
        await Verify(service.ProductionUnits);
    }

    [Fact]
    public async Task LoadUnits_Should_Throw_Exception_For_InvalidJson()
    {
        // Arrange
        var service = new AssetManager();

        // Act & Validate
        await Throws(() => service.LoadUnits("./Services/AssetManagerTest_Invalid.json"));
    }

    [Fact]
    public async Task LoadUnits_Should_Handle_Missing_Fields()
    {
        // Arrange
        var service = new AssetManager();

        // Act & Validate
        await Throws(() => service.LoadUnits("./Services/AssetManagerTest_MissingFields.json"));
    }

    [Fact]
    public async Task LoadUnits_Should_Handle_Invalid_Data_Types()
    {
        // Arrange
        var service = new AssetManager();

        // Act & Validate
        await Throws(() => service.LoadUnits("./Services/AssetManagerTest_InvalidData.json"));
    }

    [Fact]
    public async Task LoadUnits_Should_Handle_Empty_File()
    {
        // Arrange
        var service = new AssetManager();

        // Act & Validate
        await Throws(() => service.LoadUnits("./Services/AssetManagerTest_Empty.json"));
    }

    [Fact]
    public async Task LoadUnits_Should_Handle_NonExistent_File()
    {
        // Arrange
        var service = new AssetManager();

        // Act & Validate
        await Throws(() => service.LoadUnits("./Services/Nonexistent.json"));
    }

    [Fact]
    public async Task LoadUnits_Should_Handle_Unrecognized_Resource()
    {
        // Arrange
        var service = new AssetManager();

        // Act
        await Throws(() => service.LoadUnits("./Services/AssetManagerTest_UnknownResource.json"));
    }

    [Fact]
    public async Task LoadUnits_Should_Handle_Duplicate_Entries()
    {
        // Arrange
        var service = new AssetManager();

        // Act
        service.LoadUnits("./Services/AssetManagerTest_Duplicate.json");

        // Validate
        service.ProductionUnits.Count.ShouldBe(2);
        
        await Verify(service.ProductionUnits);
    }

    [Fact]
    public async Task LoadUnits_Should_Handle_Large_Numbers()
    {
        // Arrange
        var service = new AssetManager();

        // Act
        service.LoadUnits("./Services/AssetManagerTest_LargeNumbers.json");

        // Validate
        await Verify(service.ProductionUnits);
    }
}
