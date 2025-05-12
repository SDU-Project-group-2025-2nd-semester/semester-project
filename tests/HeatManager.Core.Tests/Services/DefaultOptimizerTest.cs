using HeatManager.Core.Models.Producers;
using HeatManager.Core.Models.SourceData;
using HeatManager.Core.Services.Optimizers;
using HeatManager.Core.Models.Resources;
using Moq;
using System.Collections.ObjectModel;
using HeatManager.Core.Services.AssetManagers;
using HeatManager.Core.Services.SourceDataProviders;
using HeatManager.Core.Models.Schedules;

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
        _mockOptimizerStrategy.Setup(s => s.Optimization).Returns(OptimizationType.PriceOptimization);

        // Act
        var schedule = _optimizer.Optimize();

        // Assert
        Assert.NotNull(schedule);
        Assert.Single(schedule.HeatProductionUnitSchedules);
        var unitSchedule = schedule.HeatProductionUnitSchedules.First();
        Assert.Equal("Unit1", unitSchedule.Name);
        Assert.Single(unitSchedule.DataPoints);
        var dataPoint = unitSchedule.DataPoints[0];
        Assert.Equal(0.5, dataPoint.Utilization);
        Assert.Equal(50, dataPoint.HeatProduction);
    }

    [Fact]
    public void Optimize_WithPriceOptimizationAndMultipleUnits_ProducesCorrectSchedule()
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
        _mockOptimizerStrategy.Setup(s => s.Optimization).Returns(OptimizationType.PriceOptimization);

        // Act
        var schedule = _optimizer.Optimize();

        // Assert
        Assert.NotNull(schedule);
        Assert.Equal(2, schedule.HeatProductionUnitSchedules.Count());
        
        var unit1Schedule = schedule.HeatProductionUnitSchedules.First(s => s.Name == "Unit1");
        var unit2Schedule = schedule.HeatProductionUnitSchedules.First(s => s.Name == "Unit2");
        
        // Unit1 should be fully utilized (50/50) because it's cheaper
        Assert.Single(unit1Schedule.DataPoints);
        Assert.Equal(1.0, unit1Schedule.DataPoints[0].Utilization);
        Assert.Equal(50, unit1Schedule.DataPoints[0].HeatProduction);
        
        // Unit2 should be partially utilized (25/50) as it's more expensive
        Assert.Single(unit2Schedule.DataPoints);
        Assert.Equal(0.5, unit2Schedule.DataPoints[0].Utilization);
        Assert.Equal(25, unit2Schedule.DataPoints[0].HeatProduction);
    }

    [Fact]
    public void Optimize_WithCo2OptimizationAndMultipleUnits_ProducesCorrectSchedule()
    {
        // Arrange
        var productionUnit1 = new HeatProductionUnit
        {
            Name = "Unit1",
            MaxHeatProduction = 50,
            Cost = 1,
            Emissions = 2,
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
        _mockOptimizerStrategy.Setup(s => s.Optimization).Returns(OptimizationType.Co2Optimization);

        // Act
        var schedule = _optimizer.Optimize();

        // Assert
        Assert.NotNull(schedule);
        Assert.Equal(2, schedule.HeatProductionUnitSchedules.Count());
        
        var unit1Schedule = schedule.HeatProductionUnitSchedules.First(s => s.Name == "Unit1");
        var unit2Schedule = schedule.HeatProductionUnitSchedules.First(s => s.Name == "Unit2");
        
        // Unit2 should be fully utilized (50/50) because it has lower emissions
        Assert.Single(unit2Schedule.DataPoints);
        Assert.Equal(1.0, unit2Schedule.DataPoints[0].Utilization);
        Assert.Equal(50, unit2Schedule.DataPoints[0].HeatProduction);
        
        // Unit1 should be partially utilized (25/50) as it has higher emissions
        Assert.Single(unit1Schedule.DataPoints);
        Assert.Equal(0.5, unit1Schedule.DataPoints[0].Utilization);
        Assert.Equal(25, unit1Schedule.DataPoints[0].HeatProduction);
    }

    [Fact]
    public void Optimize_WithElectricityProducingUnit_ProducesCorrectSchedules()
    {
        // Arrange
        var electricityUnit = new ElectricityProductionUnit
        {
            Name = "CHP",
            MaxHeatProduction = 100,
            MaxElectricity = 50,
            Cost = 2,
            Emissions = 1,
            ResourceConsumption = 1,
            Resource = _gas
        };

        var sourceDataPoint = new SourceDataPoint
        {
            HeatDemand = 50,
            ElectricityPrice = 3,
            TimeFrom = DateTime.Now,
            TimeTo = DateTime.Now.AddHours(1)
        };

        var sourceDataCollection = new SourceDataCollection([sourceDataPoint]);

        _mockSourceDataProvider.Setup(p => p.SourceDataCollection).Returns(sourceDataCollection);
        _mockAssetManager.Setup(a => a.ProductionUnits).Returns(new ObservableCollection<ProductionUnitBase> { electricityUnit });
        _mockOptimizerSettings.Setup(s => s.GetActiveUnitsNames()).Returns(new List<string> { "CHP" });
        _mockOptimizerStrategy.Setup(s => s.Optimization).Returns(OptimizationType.PriceOptimization);

        // Act
        var schedule = _optimizer.Optimize();

        // Assert
        Assert.NotNull(schedule);
        
        // Verify heat production schedule
        Assert.Single(schedule.HeatProductionUnitSchedules);
        var heatSchedule = schedule.HeatProductionUnitSchedules.First();
        Assert.Equal("CHP", heatSchedule.Name);
        Assert.Single(heatSchedule.DataPoints);
        Assert.Equal(0.5, heatSchedule.DataPoints[0].Utilization); // 50/100
        Assert.Equal(50, heatSchedule.DataPoints[0].HeatProduction);
        
        // Verify electricity production schedule
        Assert.Single(schedule.ElectricityProductionUnitSchedules);
        var electricitySchedule = schedule.ElectricityProductionUnitSchedules.First();
        Assert.Equal("CHP", electricitySchedule.Name);
        Assert.Single(electricitySchedule.DataPoints);
        Assert.Equal(25, electricitySchedule.DataPoints[0].ElectricityProduction); // 50% utilization * 50 max
        Assert.Equal(3, electricitySchedule.DataPoints[0].ElectricityPrice);
    }
}
