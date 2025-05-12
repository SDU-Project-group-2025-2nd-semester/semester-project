using HeatManager.Core.Models.Producers;
using HeatManager.Core.Models.SourceData;
using HeatManager.Core.Services.Optimizers;
using HeatManager.Core.Models.Resources;
using Moq;
using System.Collections.ObjectModel;
using HeatManager.Core.Services.AssetManagers;
using HeatManager.Core.Services.SourceDataProviders;

namespace HeatManager.Core.Tests.Services;
/*
 * dotnet test --filter "FullyQualifiedName~DefaultOptimizerTest"
 */
public class DefaultOptimizerTest
{
    private readonly Mock<IAssetManager> _mockAssetManager;
    private readonly Mock<ISourceDataProvider> _mockSourceDataProvider;
    private readonly Mock<IOptimizerSettings> _mockOptimizerSettings;
    private readonly Mock<IOptimizerStrategy> _mockOptimizerStrategy;
    private readonly Mock<object> _mockResultManager;
    private readonly DefaultOptimizer _optimizer;

    private readonly Resource _oil = new("Oil");
    private readonly Resource _gas = new("Gas");
    private readonly Resource _electricity = new("Electricity");

    public DefaultOptimizerTest()
    {
        _mockAssetManager = new Mock<IAssetManager>();
        _mockSourceDataProvider = new Mock<ISourceDataProvider>();
        _mockOptimizerSettings = new Mock<IOptimizerSettings>();
        _mockOptimizerStrategy = new Mock<IOptimizerStrategy>();

        _optimizer = new DefaultOptimizer(
            _mockAssetManager.Object,
            _mockSourceDataProvider.Object,
            _mockOptimizerSettings.Object,
            _mockOptimizerStrategy.Object);
    }

    [Fact]
    public void Constructor_InitializesCorrectly()
    {
        // Assert
        Assert.NotNull(_optimizer);
    }

    [Fact]
    public void GetAvailableUnits_ReturnsOnlyActiveUnits()
    {
        // Arrange
        var activeUnits = new List<string> { "Unit1", "Unit2" };

        var allUnits = new ObservableCollection<ProductionUnitBase>
        {
            new HeatProductionUnit { Name = "Unit1", Resource = _oil },
            new HeatProductionUnit { Name = "Unit2", Resource = _oil },
            new HeatProductionUnit { Name = "Unit3", Resource = _oil }
        };

        _mockOptimizerSettings.Setup(s => s.GetActiveUnitsNames()).Returns(activeUnits);
        _mockAssetManager.Setup(a => a.ProductionUnits).Returns(allUnits);

        // Act
        var result = _optimizer.GetAvailableUnits(_mockAssetManager.Object, _mockOptimizerSettings.Object);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, u => u.Name == "Unit1");
        Assert.Contains(result, u => u.Name == "Unit2");
        Assert.DoesNotContain(result, u => u.Name == "Unit3");
    }

    [Fact]
    public void ChangeOptimizationSettings_UpdatesSettings()
    {
        // Arrange
        var newSettings = new Mock<IOptimizerSettings>();

        // Act
        _optimizer.ChangeOptimizationSettings(newSettings.Object);

        // Assert
        // Note: Since _optimizerSettings is private, we can't directly verify it
        // This test is more of a placeholder to show the intent
    }

    [Fact]
    public void GenerateHeatProductionUnitSchedules_CreatesCorrectNumberOfSchedules()
    {
        // Arrange
        var units = new List<HeatProductionUnit>
        {
            new HeatProductionUnit { Name = "Unit1", Resource = _oil },
            new HeatProductionUnit { Name = "Unit2", Resource = _oil }
        };

        // Act
        var result = _optimizer.GenerateHeatProductionUnitSchedules(units);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, s => s.Name == "Unit1");
        Assert.Contains(result, s => s.Name == "Unit2");
    }

    [Fact]
    public void GenerateElectricityProductionUnitSchedules_CreatesCorrectNumberOfSchedules()
    {
        // Arrange
        var units = new List<ElectricityProductionUnit>
        {
            new Mock<ElectricityProductionUnit>().Object,
            new Mock<ElectricityProductionUnit>().Object
        };

        // Act
        var result = _optimizer.GenerateElectricityProductionUnitSchedules(units);

        // Assert
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void GetHeatSourcePriorityList_WithPriceOptimization_OrdersByCost()
    {
        // Arrange
        var units = new List<HeatProductionUnit>
        {
            new HeatProductionUnit { Name = "Unit1", Cost = 2, Emissions = 1, Resource = _oil },
            new HeatProductionUnit { Name = "Unit2", Cost = 1, Emissions = 2, Resource = _oil }
        };
        var strategy = new OptimizerStrategy(true);
        var sourceDataPoint = new SourceDataPoint();

        // Act
        var result = DefaultOptimizer.GetHeatSourcePriorityList(units, sourceDataPoint, strategy);

        // Assert
        Assert.Equal("Unit2", result.First().Name); // Cheaper unit should be first
        Assert.Equal("Unit1", result.Last().Name);
    }

    [Fact]
    public void GetHeatSourcePriorityList_WithCo2Optimization_OrdersByEmissions()
    {
        // Arrange
        var units = new List<HeatProductionUnit>
        {
            new HeatProductionUnit { Name = "Unit1", Cost = 1, Emissions = 2, Resource = _oil },
            new HeatProductionUnit { Name = "Unit2", Cost = 2, Emissions = 1, Resource = _oil }
        };
        var strategy = new OptimizerStrategy(false);
        var mockSourceDataPoint = new SourceDataPoint();

        // Act
        var result = DefaultOptimizer.GetHeatSourcePriorityList(units, mockSourceDataPoint, strategy);

        // Assert
        Assert.Equal("Unit2", result.First().Name); // Lower emissions unit should be first
        Assert.Equal("Unit1", result.Last().Name);
    }

    [Fact]
    public void GetHeatSourcePriorityList_WithElectricityBasedUnits_AdjustsCosts()
    {
        // Arrange

        var unit = new HeatProductionUnit
        {
            Name = "Unit1",
            Cost = 1,
            Emissions = 1,
            Resource = _electricity
        };

        var strategy = new OptimizerStrategy(true);
        var sourceDataPoint = new SourceDataPoint { ElectricityPrice = 2 };

        // Act
        var result = DefaultOptimizer.GetHeatSourcePriorityList([unit], sourceDataPoint, strategy);

        // Assert
        Assert.Equal(3, result.First().Cost); // Original cost + electricity price
    }

    [Fact]
    public void GetHeatSourcePriorityList_WithMixedResourceTypes_HandlesCorrectly()
    {
        // Arrange

        var units = new List<HeatProductionUnit>
        {
            new HeatProductionUnit { Name = "GasUnit", Cost = 2, Emissions = 1, Resource = _gas},
            new HeatProductionUnit { Name = "ElectricUnit", Cost = 1, Emissions = 2, Resource = _electricity }
        };

        var strategy = new OptimizerStrategy(true);

        var sourceDataPoint = new SourceDataPoint();

        // Act
        var result = DefaultOptimizer.GetHeatSourcePriorityList(units, sourceDataPoint, strategy);

        // Assert
        Assert.Equal("ElectricUnit", result.First().Name); // Should be first despite higher emissions due to lower cost
    }

    [Fact]
    public void GetHeatSourcePriorityList_WithNegativeCost_HandlesCorrectly()
    {
        // Arrange
        var units = new List<HeatProductionUnit>
        {
            new HeatProductionUnit { Name = "Unit1", Cost = -1, Emissions = 1, Resource = _oil },
            new HeatProductionUnit { Name = "Unit2", Cost = 1, Emissions = 1, Resource = _oil }
        };

        var strategy = new OptimizerStrategy(true);
        var sourceDataPoint = new SourceDataPoint();

        // Act
        var result = DefaultOptimizer.GetHeatSourcePriorityList(units, sourceDataPoint, strategy);

        // Assert
        Assert.Equal("Unit1", result.First().Name); // Negative cost should be first
    }

    [Fact]
    public void GetHeatSourcePriorityList_WithEqualCosts_OrdersByEmissions()
    {
        // Arrange
        var units = new List<HeatProductionUnit>
        {
            new() { Name = "Unit1", Cost = 1, Emissions = 2, Resource = _oil },
            new() { Name = "Unit2", Cost = 1, Emissions = 1, Resource = _oil }
        };

        var strategy = new OptimizerStrategy(true);
        var sourceDataPoint = new SourceDataPoint();

        // Act
        var result = DefaultOptimizer.GetHeatSourcePriorityList(units, sourceDataPoint, strategy);

        // Assert
        Assert.Equal("Unit2", result.First().Name); // Lower emissions should be first when costs are equal
    }

    [Fact]
    public void GetHeatSourcePriorityList_WithElectricityProduction_AdjustsCostsCorrectly()
    {
        // Arrange
        var electricityProductionUnit = new ElectricityProductionUnit()
        {
            Name = "ProducingUnit",
            Cost = 1,
            Emissions = 1,
            Resource = _electricity,
            MaxHeatProduction = 3,
            MaxElectricity = 3
        };


        var units = new List<ProductionUnitBase> { electricityProductionUnit };
        var strategy = new OptimizerStrategy(true);

        var sourceDataPoint = new SourceDataPoint
        {
            ElectricityPrice = 2,
            HeatDemand = 2
        };
        
        // Act
        var result = DefaultOptimizer.GetHeatSourcePriorityList(units, sourceDataPoint, strategy);

        // Assert
        var resultUnit = result.First();
        Assert.Equal("ProducingUnit", resultUnit.Name);
        Assert.Equal(-1, resultUnit.Cost); // Original cost (1) - electricity price (2) = -1
    }

    [Fact]
    public void GetHeatSourcePriorityList_WithMixedElectricityUnits_AdjustsCostsCorrectly()
    {
        // Arrange

        // Unit that only consumes electricity
        var heatProductionUnit = new HeatProductionUnit
        {
            Name = "ConsumingUnit",
            Cost = 1,
            Emissions = 1,
            Resource = _electricity,
            MaxHeatProduction = 3
        };

        // Unit that produces electricity
        var electricityProductionUnit = new ElectricityProductionUnit()
        {
            Name = "ProducingUnit",
            Cost = 1,
            Emissions = 1,
            Resource = _electricity,
            MaxHeatProduction = 3,
            MaxElectricity = 3
        };

        var units = new List<ProductionUnitBase> { heatProductionUnit, electricityProductionUnit };
        var strategy = new OptimizerStrategy(true);

        var sourceDataPoint = new SourceDataPoint
        {
            ElectricityPrice = 2,
            HeatDemand = 2
        };

        // Act
        var result = DefaultOptimizer.GetHeatSourcePriorityList(units, sourceDataPoint, strategy);

        var amazingName = result.ElementAt(0);
        var amazingName2 = result.ElementAt(1);

        // Assert
        var resultList = result.ToList();
        Assert.Equal(2, resultList.Count);

        // Consuming unit should have increased cost
        var consumingUnit = resultList.First(u => u.Name == "ConsumingUnit");
        Assert.Equal(3, amazingName2.Cost); // Original cost (1) + electricity price (2) = 3

        // Producing unit should have decreased cost
        var producingUnit = resultList.First(u => u.Name == "ProducingUnit");
        Assert.Equal(-1, amazingName.Cost); // Original cost (1) - electricity price (2) = -1
    }

    [Fact]
    public void GenerateHeatProductionUnitSchedules_WithEmptyList_ReturnsEmptyList()
    {
        // Arrange
        var units = new List<HeatProductionUnit>();

        // Act
        var result = _optimizer.GenerateHeatProductionUnitSchedules(units);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void GenerateElectricityProductionUnitSchedules_WithEmptyList_ReturnsEmptyList()
    {
        // Arrange
        var units = new List<ElectricityProductionUnit>();

        // Act
        var result = _optimizer.GenerateElectricityProductionUnitSchedules(units);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void GetAvailableUnits_WithEmptyActiveUnits_ReturnsEmptyList()
    {
        // Arrange
        var activeUnits = new List<string>();
        var allUnits = new ObservableCollection<ProductionUnitBase>
        {
            new HeatProductionUnit { Name = "Unit1", Resource = _oil }
        };

        _mockOptimizerSettings.Setup(s => s.GetActiveUnitsNames()).Returns(activeUnits);
        _mockAssetManager.Setup(a => a.ProductionUnits).Returns(allUnits);

        // Act
        var result = _optimizer.GetAvailableUnits(_mockAssetManager.Object, _mockOptimizerSettings.Object);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void GetAvailableUnits_WithNonExistentActiveUnits_ReturnsEmptyList()
    {
        // Arrange
        var activeUnits = new List<string> { "NonExistentUnit" };
        var allUnits = new ObservableCollection<ProductionUnitBase>
        {
            new HeatProductionUnit { Name = "Unit1", Resource = _oil }
        };

        _mockOptimizerSettings.Setup(s => s.GetActiveUnitsNames()).Returns(activeUnits);
        _mockAssetManager.Setup(a => a.ProductionUnits).Returns(allUnits);

        // Act
        var result = _optimizer.GetAvailableUnits(_mockAssetManager.Object, _mockOptimizerSettings.Object);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void Optimize_WithSingleUnit_ProducesCorrectSchedule()
    {
        // Arrange
        var unit = new HeatProductionUnit
        {
            Name = "Unit1",
            MaxHeatProduction = 100,
            Cost = 1,
            Emissions = 1,
            ResourceConsumption = 1,
            Resource = _oil
        };

        var sourceDataPoint = new SourceDataPoint
        {
            HeatDemand = 50,
            ElectricityPrice = 0,
            TimeFrom = DateTime.Now,
            TimeTo = DateTime.Now.AddHours(1)
        };

        var sourceDataCollection = new SourceDataCollection([sourceDataPoint]);

        _mockSourceDataProvider.Setup(p => p.SourceDataCollection).Returns(sourceDataCollection);
        _mockAssetManager.Setup(a => a.ProductionUnits).Returns(new ObservableCollection<ProductionUnitBase> { unit });
        _mockOptimizerSettings.Setup(s => s.GetActiveUnitsNames()).Returns(new List<string> { "Unit1" });

        // Act
        _optimizer.Optimize();

        // Assert
        // Note: Since we don't have access to the result manager, we can't verify the actual schedule
        // This test is more of a smoke test to ensure the method runs without exceptions
    }

    [Fact]
    public void Optimize_WithMultipleUnits_HandlesDemandDistribution()
    {
        // Arrange
        var productionUnit1 = new HeatProductionUnit
        {
            Name = "Unit1",
            MaxHeatProduction = 50,
            Cost = 1,
            Emissions = 1,
            ResourceConsumption = 1,
            Resource = _oil
        };

        var productionUnit2 = new HeatProductionUnit
        {
            Name = "Unit2",
            MaxHeatProduction = 50,
            Cost = 2,
            Emissions = 1,
            ResourceConsumption = 1,
            Resource = _oil
        };

        var sourceDataPoint = new SourceDataPoint()
        {
            ElectricityPrice = 0,
            HeatDemand = 75,
            TimeFrom = DateTime.Now,
            TimeTo = DateTime.Now.AddHours(1)
        };

        var sourceDataCollection = new SourceDataCollection([sourceDataPoint]);

        _mockSourceDataProvider.Setup(p => p.SourceDataCollection).Returns(sourceDataCollection);
        _mockAssetManager.Setup(a => a.ProductionUnits).Returns(new ObservableCollection<ProductionUnitBase> { productionUnit1, productionUnit2 });
        _mockOptimizerSettings.Setup(s => s.GetActiveUnitsNames()).Returns(new List<string> { "Unit1", "Unit2" });

        // Act
        _optimizer.Optimize();

        // Assert
        // Note: Since we don't have access to the result manager, we can't verify the actual schedule
        // This test is more of a smoke test to ensure the method runs without exceptions
    }

    [Fact]
    public void Optimize_WithExcessDemand_HandlesCorrectly()
    {
        // Arrange

        var heatProductionUnit = new HeatProductionUnit
        {
            Name = "Unit1",
            MaxHeatProduction = 50,
            Cost = 1,
            Emissions = 1,
            ResourceConsumption = 1,
            Resource = _oil
        };

        var sourceDataPoint = new SourceDataPoint
        {
            HeatDemand = 100,
            ElectricityPrice = 0,
            TimeFrom = DateTime.Now,
            TimeTo = DateTime.Now.AddHours(1)
        };

        var sourceDataCollection = new SourceDataCollection([sourceDataPoint]);

        _mockSourceDataProvider.Setup(p => p.SourceDataCollection).Returns(sourceDataCollection);
        _mockAssetManager.Setup(a => a.ProductionUnits).Returns(new ObservableCollection<ProductionUnitBase> { heatProductionUnit });
        _mockOptimizerSettings.Setup(s => s.GetActiveUnitsNames()).Returns(["Unit1"]);

        // Act
        _optimizer.Optimize();

        // Assert
        // Note: Since we don't have access to the result manager, we can't verify the actual schedule
        // This test is more of a smoke test to ensure the method runs without exceptions
    }
}
