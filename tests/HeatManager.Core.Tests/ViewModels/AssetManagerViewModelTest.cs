using System.IO;
using HeatManager.Core.Models.Producers;
using HeatManager.Core.ViewModels;
using Shouldly;
using Xunit;
using JetBrains.Annotations;

namespace HeatManager.Core.Tests.ViewModels;

[TestSubject(typeof(AssetManagerViewModel))]
public class AssetManagerViewModelTest
{
    private const string ValidJson = """ 
                                     [
                                       {
                                         "Name": "GB1",
                                         "Cost": 520,
                                         "MaxHeatProduction": 4,
                                         "ResourceConsumption": 0.9,
                                         "Resource": "Gas",
                                         "Emissions": 175
                                       },
                                       {
                                         "Name": "GB2",
                                         "Cost": 560,
                                         "MaxHeatProduction": 3,
                                         "ResourceConsumption": 0.7,
                                         "Resource": "Gas",
                                         "Emissions": 130
                                       },
                                       {
                                         "Name": "OB1",
                                         "Cost": 670,
                                         "MaxHeatProduction": 4,
                                         "ResourceConsumption": 1.5,
                                         "Resource": "Oil",
                                         "Emissions": 330
                                       },
                                       {
                                         "Name": "GM1",
                                         "Cost": 990,
                                         "MaxHeatProduction": 3.5,
                                         "ResourceConsumption": 1.8,
                                         "Resource": "Gas",
                                         "Emissions": 650,
                                         "MaxElectricity": 2.6
                                       },
                                       {
                                         "Name": "HP1",
                                         "Cost": 60,
                                         "MaxHeatProduction": 6,
                                         "ResourceConsumption": 0,
                                         "Resource": "Electricity",
                                         "Emissions": 0,
                                         "MaxElectricity": -6
                                       }
                                     ]
                                     """;

    [Fact]
    public void LoadUnits_Should_Deserialize_Correct_ProductionUnits()
    {
        // Arrange
        var filePath = "test.json";
        File.WriteAllText(filePath, ValidJson);
        var viewModel = new AssetManagerViewModel();

        // Act
        viewModel.LoadUnits(filePath);

        // Assert
        viewModel.ProductionUnits.ShouldNotBeNull();
        viewModel.ProductionUnits.Count.ShouldBe(5);

        // Verify class types
        viewModel.ProductionUnits[0].ShouldBeOfType<HeatProductionUnit>(); // GB1
        viewModel.ProductionUnits[1].ShouldBeOfType<HeatProductionUnit>(); // GB2
        viewModel.ProductionUnits[2].ShouldBeOfType<HeatProductionUnit>(); // OB1
        viewModel.ProductionUnits[3].ShouldBeOfType<ElectricityProductionUnit>(); // GM1
        viewModel.ProductionUnits[4].ShouldBeOfType<ElectricityProductionUnit>(); // HP1

        // Cleanup
        File.Delete(filePath);
    }

    [Fact]
    public void LoadUnits_Should_Assign_Correct_Properties()
    {
        // Arrange
        var filePath = "test.json";
        File.WriteAllText(filePath, ValidJson);
        var viewModel = new AssetManagerViewModel();

        // Act
        viewModel.LoadUnits(filePath);

        // Assert
        var gb1 = viewModel.ProductionUnits[0];
        gb1.ShouldNotBeNull();
        gb1.Name.ShouldBe("GB1");
        gb1.Cost.ShouldBe(520.0m);
        gb1.MaxHeatProduction.ShouldBe(4.0);
        gb1.Resource.Name.ShouldBe("Gas");
        gb1.Emissions.ShouldBe(175.0);

        var gm1 = viewModel.ProductionUnits[3] as IElectricityProductionUnit;
        gm1.ShouldNotBeNull();
        gm1.Name.ShouldBe("GM1");
        gm1.MaxElectricity.ShouldBe(2.6);

        var hp1 = viewModel.ProductionUnits[4] as IElectricityProductionUnit;
        hp1.ShouldNotBeNull();
        hp1.MaxElectricity.ShouldBe(-6.0);

        // Cleanup
        File.Delete(filePath);
    }
}
