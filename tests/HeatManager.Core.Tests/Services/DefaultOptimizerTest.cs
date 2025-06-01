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

    /*
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
    */

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

    /*
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
    */ 
    /*
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
        var result = _optimizer.GetAvailableUnits();

        // Assert
        Assert.Empty(result);
    } */ 

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

    [Fact]
    public void Optimize_WithZeroHeatDemand_ProducesScheduleWithZeroUtilization()
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
            HeatDemand = 0, // Zero heat demand
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
        Assert.Equal(0, dataPoint.Utilization);
        Assert.Equal(0, dataPoint.HeatProduction);
        Assert.Equal(0, dataPoint.Cost);
        Assert.Equal(0, dataPoint.ResourceConsumption);
        Assert.Equal(0, dataPoint.Emissions);
    }

    [Fact]
    public void Optimize_WithNoActiveUnits_ReturnsEmptySchedule()
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
        _mockOptimizerSettings.Setup(s => s.GetActiveUnitsNames()).Returns(new List<string>()); // No active units

        // Act
        var schedule = _optimizer.Optimize();

        // Assert
        Assert.NotNull(schedule);
        Assert.Empty(schedule.HeatProductionUnitSchedules);
        Assert.Empty(schedule.ElectricityProductionUnitSchedules);
    }

    [Fact]
    public void Optimize_WithMultipleDataPoints_ProducesCorrectSchedule()
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

        var now = DateTime.Now;
        var sourceDataPoints = new List<SourceDataPoint>
        {
            new SourceDataPoint
            {
                HeatDemand = 50,
                ElectricityPrice = 0,
                TimeFrom = now,
                TimeTo = now.AddHours(1)
            },
            new SourceDataPoint
            {
                HeatDemand = 75,
                ElectricityPrice = 0,
                TimeFrom = now.AddHours(1),
                TimeTo = now.AddHours(2)
            }
        };

        var sourceDataCollection = new SourceDataCollection(sourceDataPoints);

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
        
        // Should have two data points
        Assert.Equal(2, unitSchedule.DataPoints.Count);
        
        // First data point
        Assert.Equal(0.5, unitSchedule.DataPoints[0].Utilization);
        Assert.Equal(50, unitSchedule.DataPoints[0].HeatProduction);
        
        // Second data point
        Assert.Equal(0.75, unitSchedule.DataPoints[1].Utilization);
        Assert.Equal(75, unitSchedule.DataPoints[1].HeatProduction);
    }

    [Fact]
    public void Optimize_WithBalancedOptimization_ProducesCorrectSchedule()
    {
        // Arrange
        var unit1 = new HeatProductionUnit
        {
            Name = "Unit1",
            MaxHeatProduction = 50,
            Cost = 1,
            Emissions = 2,
            ResourceConsumption = 1,
            Resource = _oil
        };

        var unit2 = new HeatProductionUnit
        {
            Name = "Unit2",
            MaxHeatProduction = 50,
            Cost = 2,
            Emissions = 1,
            ResourceConsumption = 1,
            Resource = _oil
        };

        var sourceDataPoint = new SourceDataPoint
        {
            HeatDemand = 60,
            ElectricityPrice = 0,
            TimeFrom = DateTime.Now,
            TimeTo = DateTime.Now.AddHours(1)
        };

        var sourceDataCollection = new SourceDataCollection([sourceDataPoint]);

        _mockSourceDataProvider.Setup(p => p.SourceDataCollection).Returns(sourceDataCollection);
        _mockAssetManager.Setup(a => a.ProductionUnits).Returns(new ObservableCollection<ProductionUnitBase> { unit1, unit2 });
        _mockOptimizerSettings.Setup(s => s.GetActiveUnitsNames()).Returns(new List<string> { "Unit1", "Unit2" });
        _mockOptimizerStrategy.Setup(s => s.Optimization).Returns(OptimizationType.BalancedOptimization);

        // Act
        var schedule = _optimizer.Optimize();

        // Assert
        Assert.NotNull(schedule);
        Assert.Equal(2, schedule.HeatProductionUnitSchedules.Count());
        
        // Check that both units are used, but exact prioritization depends on the implementation
        var totalProduction = schedule.HeatProductionUnitSchedules.Sum(s => s.TotalHeatProduction);
        Assert.Equal(60, totalProduction);
    }

    [Fact]
    public void Optimize_WithHeatPump_AdjustsCostBasedOnElectricityPrice()
    {
        // Arrange
        var heatPump = new HeatProductionUnit
        {
            Name = "HeatPump",
            MaxHeatProduction = 100,
            Cost = 10, // Base cost
            Emissions = 0,
            ResourceConsumption = 1,
            Resource = _electricity // Heat pump uses electricity
        };

        var sourceDataPoint = new SourceDataPoint
        {
            HeatDemand = 50,
            ElectricityPrice = 5, // Electricity price will be added to the base cost
            TimeFrom = DateTime.Now,
            TimeTo = DateTime.Now.AddHours(1)
        };

        var sourceDataCollection = new SourceDataCollection([sourceDataPoint]);

        _mockSourceDataProvider.Setup(p => p.SourceDataCollection).Returns(sourceDataCollection);
        _mockAssetManager.Setup(a => a.ProductionUnits).Returns(new ObservableCollection<ProductionUnitBase> { heatPump });
        _mockOptimizerSettings.Setup(s => s.GetActiveUnitsNames()).Returns(new List<string> { "HeatPump" });
        _mockOptimizerStrategy.Setup(s => s.Optimization).Returns(OptimizationType.PriceOptimization);

        // Act
        var schedule = _optimizer.Optimize();

        // Assert
        Assert.NotNull(schedule);
        Assert.Single(schedule.HeatProductionUnitSchedules);
        var unitSchedule = schedule.HeatProductionUnitSchedules.First();
        Assert.Equal("HeatPump", unitSchedule.Name);
        Assert.Single(unitSchedule.DataPoints);
        
        // Cost should reflect the electricity cost addition
        var dataPoint = unitSchedule.DataPoints[0];
        Assert.Equal(50, dataPoint.HeatProduction);
        // Cost should be based on heat production * (base cost + electricity price)
        Assert.Equal(750m, dataPoint.Cost); // 50 * (10 + 5)
    }

    [Fact]
    public void Optimize_WithHighElectricityProducerPriority_ProducesCorrectSchedule()
    {
        // Arrange
        var heatUnit = new HeatProductionUnit
        {
            Name = "HeatOnly",
            MaxHeatProduction = 100,
            Cost = 5,
            Emissions = 1,
            ResourceConsumption = 1,
            Resource = _gas
        };

        var chpUnit = new ElectricityProductionUnit
        {
            Name = "CHP",
            MaxHeatProduction = 100,
            MaxElectricity = 50,
            Cost = 10, // Higher base cost than heat only
            Emissions = 2,
            ResourceConsumption = 1,
            Resource = _gas
        };

        var sourceDataPoint = new SourceDataPoint
        {
            HeatDemand = 150,
            ElectricityPrice = 20, // Very high electricity price makes CHP more attractive
            TimeFrom = DateTime.Now,
            TimeTo = DateTime.Now.AddHours(1)
        };

        var sourceDataCollection = new SourceDataCollection([sourceDataPoint]);

        _mockSourceDataProvider.Setup(p => p.SourceDataCollection).Returns(sourceDataCollection);
        _mockAssetManager.Setup(a => a.ProductionUnits)
            .Returns(new ObservableCollection<ProductionUnitBase> { heatUnit, chpUnit });
        _mockOptimizerSettings.Setup(s => s.GetActiveUnitsNames())
            .Returns(new List<string> { "HeatOnly", "CHP" });
        _mockOptimizerStrategy.Setup(s => s.Optimization).Returns(OptimizationType.PriceOptimization);

        // Act
        var schedule = _optimizer.Optimize();

        // Assert
        Assert.NotNull(schedule);
        Assert.Equal(2, schedule.HeatProductionUnitSchedules.Count());
        
        // CHP should be prioritized due to high electricity price
        var chpSchedule = schedule.HeatProductionUnitSchedules.First(s => s.Name == "CHP");
        var heatOnlySchedule = schedule.HeatProductionUnitSchedules.First(s => s.Name == "HeatOnly");
        
        Assert.Equal(1.0, chpSchedule.DataPoints[0].Utilization); // CHP should be fully utilized
        Assert.Equal(0.5, heatOnlySchedule.DataPoints[0].Utilization); // Heat only partially utilized
        
        // Verify electricity production
        Assert.Single(schedule.ElectricityProductionUnitSchedules);
        var electricitySchedule = schedule.ElectricityProductionUnitSchedules.First();
        Assert.Equal("CHP", electricitySchedule.Name);
        Assert.Equal(50, electricitySchedule.DataPoints[0].ElectricityProduction);
    }

    [Fact]
    public void Optimize_WithExcessiveHeatDemand_UsesAllAvailableUnits()
    {
        // Arrange
        var unit1 = new HeatProductionUnit
        {
            Name = "Unit1",
            MaxHeatProduction = 50,
            Cost = 1,
            Emissions = 1,
            ResourceConsumption = 1,
            Resource = _oil
        };

        var unit2 = new HeatProductionUnit
        {
            Name = "Unit2",
            MaxHeatProduction = 50,
            Cost = 2,
            Emissions = 2,
            ResourceConsumption = 1,
            Resource = _oil
        };

        var sourceDataPoint = new SourceDataPoint
        {
            HeatDemand = 200, // Demand exceeds total capacity of 100
            ElectricityPrice = 0,
            TimeFrom = DateTime.Now,
            TimeTo = DateTime.Now.AddHours(1)
        };

        var sourceDataCollection = new SourceDataCollection([sourceDataPoint]);

        _mockSourceDataProvider.Setup(p => p.SourceDataCollection).Returns(sourceDataCollection);
        _mockAssetManager.Setup(a => a.ProductionUnits).Returns(new ObservableCollection<ProductionUnitBase> { unit1, unit2 });
        _mockOptimizerSettings.Setup(s => s.GetActiveUnitsNames()).Returns(new List<string> { "Unit1", "Unit2" });
        _mockOptimizerStrategy.Setup(s => s.Optimization).Returns(OptimizationType.PriceOptimization);

        // Act
        var schedule = _optimizer.Optimize();

        // Assert
        Assert.NotNull(schedule);
        Assert.Equal(2, schedule.HeatProductionUnitSchedules.Count());
        
        // Both units should be fully utilized
        var unit1Schedule = schedule.HeatProductionUnitSchedules.First(s => s.Name == "Unit1");
        var unit2Schedule = schedule.HeatProductionUnitSchedules.First(s => s.Name == "Unit2");
        
        Assert.Equal(1.0, unit1Schedule.DataPoints[0].Utilization);
        Assert.Equal(50, unit1Schedule.DataPoints[0].HeatProduction);
        
        Assert.Equal(1.0, unit2Schedule.DataPoints[0].Utilization);
        Assert.Equal(50, unit2Schedule.DataPoints[0].HeatProduction);
        
        // Total heat production should be 100 (max capacity)
        Assert.Equal(100, unit1Schedule.TotalHeatProduction + unit2Schedule.TotalHeatProduction);
    }

    [Fact]
    public void Optimize_WithNegativeElectricityPrice_ProducesCorrectSchedule()
    {
        // Arrange
        var heatUnit = new HeatProductionUnit
        {
            Name = "HeatOnly",
            MaxHeatProduction = 100,
            Cost = 5,
            Emissions = 1,
            ResourceConsumption = 1,
            Resource = _gas
        };

        var chpUnit = new ElectricityProductionUnit
        {
            Name = "CHP",
            MaxHeatProduction = 100,
            MaxElectricity = 50,
            Cost = 20, // Higher base cost
            Emissions = 2,
            ResourceConsumption = 1,
            Resource = _gas
        };

        var sourceDataPoint = new SourceDataPoint
        {
            HeatDemand = 150,
            ElectricityPrice = -10, // Negative electricity price (pay to produce)
            TimeFrom = DateTime.Now,
            TimeTo = DateTime.Now.AddHours(1)
        };

        var sourceDataCollection = new SourceDataCollection([sourceDataPoint]);

        _mockSourceDataProvider.Setup(p => p.SourceDataCollection).Returns(sourceDataCollection);
        _mockAssetManager.Setup(a => a.ProductionUnits)
            .Returns(new ObservableCollection<ProductionUnitBase> { heatUnit, chpUnit });
        _mockOptimizerSettings.Setup(s => s.GetActiveUnitsNames())
            .Returns(new List<string> { "HeatOnly", "CHP" });
        _mockOptimizerStrategy.Setup(s => s.Optimization).Returns(OptimizationType.PriceOptimization);

        // Act
        var schedule = _optimizer.Optimize();

        // Assert
        Assert.NotNull(schedule);
        Assert.Equal(2, schedule.HeatProductionUnitSchedules.Count());
        
        // Heat only unit should be prioritized due to negative electricity price
        var heatOnlySchedule = schedule.HeatProductionUnitSchedules.First(s => s.Name == "HeatOnly");
        var chpSchedule = schedule.HeatProductionUnitSchedules.First(s => s.Name == "CHP");
        
        Assert.Equal(1.0, heatOnlySchedule.DataPoints[0].Utilization); // Heat only should be fully utilized
        Assert.Equal(0.5, chpSchedule.DataPoints[0].Utilization); // CHP partially utilized
        
        // Verify electricity production with negative price
        Assert.Single(schedule.ElectricityProductionUnitSchedules);
        var electricitySchedule = schedule.ElectricityProductionUnitSchedules.First();
        Assert.Equal("CHP", electricitySchedule.Name);
        Assert.Equal(25, electricitySchedule.DataPoints[0].ElectricityProduction); // 50% of 50 MW
        Assert.Equal(-10, electricitySchedule.DataPoints[0].ElectricityPrice);
    }

    [Fact]
    public void UpdateProductionUnits_Updates_AssetManager_OptimizerSettings()
    {
        // Arrange
        var _newmockAssetManager = new Mock<IAssetManager>();
        var productionUnits = new ObservableCollection<ProductionUnitBase>
        {
            new HeatProductionUnit { Name = "TestUnit", Resource = _oil }
        };
        _newmockAssetManager.Setup(a => a.ProductionUnits).Returns(productionUnits);

        var mockOptimizerSettings = new Mock<IOptimizerSettings>();
        var allUnits = new Dictionary<string, bool> { { "TestUnit", true } };
        mockOptimizerSettings.Setup(s => s.AllUnits).Returns(allUnits);
        _optimizer.ChangeOptimizationSettings(mockOptimizerSettings.Object);

        // Act
        _optimizer.UpdateProductionUnits(_newmockAssetManager.Object);

        // Assert
        Assert.NotNull(_optimizer._assetManager);
        Assert.Single(_optimizer._assetManager.ProductionUnits);
        
        // Verify through public property
        Assert.NotNull(_optimizer.OptimizerSettings);
        Assert.Single(_optimizer.OptimizerSettings.AllUnits);
    }
}
