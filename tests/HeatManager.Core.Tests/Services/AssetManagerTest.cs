using System.IO;
using HeatManager.Core.Models.Producers;
using HeatManager.Core.Services;
using Shouldly;
using Xunit;
using JetBrains.Annotations;
using System;

namespace HeatManager.Core.Tests.Services;

[TestSubject(typeof(AssetManager))]
public class AssetManagerTest
{
    private const string ValidJson = @"
        {
            ""HeatProductionUnits"":
            [
                { ""Name"": ""GB1"", ""Cost"": 520.0, ""MaxHeatProduction"": 4.0, ""ResourceConsumption"": 0.9, ""Resource"": ""Gas"", ""Emissions"": 175.0 },
                { ""Name"": ""GB2"", ""Cost"": 560.0, ""MaxHeatProduction"": 3.0, ""ResourceConsumption"": 0.7, ""Resource"": ""Gas"", ""Emissions"": 130.0 },
                { ""Name"": ""OB1"", ""Cost"": 670.0, ""MaxHeatProduction"": 4.0, ""ResourceConsumption"": 1.5, ""Resource"": ""Oil"", ""Emissions"": 330.0 }
            ],
            ""ElectricityProductionUnits"":
            [
                { ""Name"": ""GM1"", ""Cost"": 990.0, ""MaxHeatProduction"": 3.5, ""ResourceConsumption"": 1.8, ""Resource"": ""Gas"", ""Emissions"": 650.0, ""MaxElectricity"": 2.6 },
                { ""Name"": ""HP1"", ""Cost"": 60.0, ""MaxHeatProduction"": 6.0, ""ResourceConsumption"": 0.0, ""Resource"": ""Electricity"", ""Emissions"": 0.0, ""MaxElectricity"": -6.0 }
            ]
        }";

    [Fact]
    public void LoadUnits_Should_Deserialize_Correct_ProductionUnits()
    {
        // Arrange
        var filePath = "test.json";
        File.WriteAllText(filePath, ValidJson);
        var service = new AssetManager();

        // Act
        service.LoadUnits(filePath);

        // Assert
        service.ProductionUnits.ShouldNotBeNull();
        service.ProductionUnits.Count.ShouldBe(5);

        // Verify class types
        service.ProductionUnits[0].ShouldBeOfType<HeatProductionUnit>(); // GB1
        service.ProductionUnits[1].ShouldBeOfType<HeatProductionUnit>(); // GB2
        service.ProductionUnits[2].ShouldBeOfType<HeatProductionUnit>(); // OB1
        service.ProductionUnits[3].ShouldBeOfType<ElectricityProductionUnit>(); // GM1
        service.ProductionUnits[4].ShouldBeOfType<ElectricityProductionUnit>(); // HP1

        // Cleanup
        File.Delete(filePath);
    }

    [Fact]
    public void LoadUnits_Should_Assign_Correct_Properties()
    {
        // Arrange
        var filePath = "test.json";
        File.WriteAllText(filePath, ValidJson);
        var service = new AssetManager();

        // Act
        service.LoadUnits(filePath);

        // Assert
        var gb1 = service.ProductionUnits[0];
        gb1.ShouldNotBeNull();
        gb1.Name.ShouldBe("GB1");
        gb1.Cost.ShouldBe(520.0m);
        gb1.MaxHeatProduction.ShouldBe(4.0);
        gb1.Resource.Name.ShouldBe("Gas");
        gb1.Emissions.ShouldBe(175.0);

        var gm1 = service.ProductionUnits[3] as IElectricityProductionUnit;
        gm1.ShouldNotBeNull();
        gm1.Name.ShouldBe("GM1");
        gm1.MaxElectricity.ShouldBe(2.6);

        var hp1 = service.ProductionUnits[4] as IElectricityProductionUnit;
        hp1.ShouldNotBeNull();
        hp1.MaxElectricity.ShouldBe(-6.0);

        // Cleanup
        File.Delete(filePath);
    }

    [Fact]
    public void LoadUnits_Should_Throw_Exception_For_InvalidJson()
    {
        var filePath = "invalid.json";
        File.WriteAllText(filePath, "{ invalid_json ");
        var service = new AssetManager();

        Should.Throw<Exception>(() => service.LoadUnits(filePath));

        File.Delete(filePath);
    }

    [Fact]
    public void LoadUnits_Should_Handle_Missing_Fields()
    {
        var json = "{\"HeatProductionUnits\": [{ \"Name\": \"GB1\" }] }";
        var filePath = "missing_fields.json";
        File.WriteAllText(filePath, json);
        var service = new AssetManager();

        Should.Throw<Exception>(() => service.LoadUnits(filePath));

        File.Delete(filePath);
    }

    [Fact]
    public void LoadUnits_Should_Handle_Invalid_Data_Types()
    {
        var json = "{\"HeatProductionUnits\": [{ \"Name\": \"GB1\", \"Cost\": \"not_a_number\" }] }";
        var filePath = "invalid_data.json";
        File.WriteAllText(filePath, json);
        var service = new AssetManager();

        Should.Throw<Exception>(() => service.LoadUnits(filePath));
        File.Delete(filePath);
    }

    [Fact]
    public void LoadUnits_Should_Handle_Empty_File()
    {
        var filePath = "empty.json";
        File.WriteAllText(filePath, "");
        var service = new AssetManager();

        Should.Throw<Exception>(() => service.LoadUnits(filePath));
        File.Delete(filePath);
    }

    [Fact]
    public void LoadUnits_Should_Handle_NonExistent_File()
    {
        var service = new AssetManager();
        Should.Throw<FileNotFoundException>(() => service.LoadUnits("nonexistent.json"));
    }

    [Fact]
    public void LoadUnits_Should_Handle_Unrecognized_Resource()
    {
        var json = "{\"HeatProductionUnits\": [{ \"Name\": \"GB1\", \"Cost\": 100, \"MaxHeatProduction\": 5, \"Resource\": \"UnknownResource\", \"Emissions\": 50 }] }";
        var filePath = "unknown_resource.json";
        File.WriteAllText(filePath, json);
        var service = new AssetManager();

        Should.Throw<Exception>(() => service.LoadUnits(filePath));
        File.Delete(filePath);
    }

    [Fact]
    public void LoadUnits_Should_Handle_Duplicate_Entries()
    {
        var json = "{\"HeatProductionUnits\": [ { \"Name\": \"GB1\", \"Cost\": 100, \"MaxHeatProduction\": 5, \"Resource\": \"Gas\", \"Emissions\": 50 }, { \"Name\": \"GB1\", \"Cost\": 100, \"MaxHeatProduction\": 5, \"Resource\": \"Gas\", \"Emissions\": 50 } ] }";
        var filePath = "duplicate.json";
        File.WriteAllText(filePath, json);
        var service = new AssetManager();

        service.LoadUnits(filePath);
        service.ProductionUnits.Count.ShouldBe(2);
        File.Delete(filePath);
    }

    [Fact]
    public void LoadUnits_Should_Handle_Large_Numbers()
    {
        var json = "{\"HeatProductionUnits\": [{ \"Name\": \"GB1\", \"Cost\": 9999999999, \"MaxHeatProduction\": 9999999999, \"Resource\": \"Gas\", \"Emissions\": 9999999999 }] }";
        var filePath = "large_numbers.json";
        File.WriteAllText(filePath, json);
        var service = new AssetManager();

        Should.NotThrow(() => service.LoadUnits(filePath));
        File.Delete(filePath);
    }
}
