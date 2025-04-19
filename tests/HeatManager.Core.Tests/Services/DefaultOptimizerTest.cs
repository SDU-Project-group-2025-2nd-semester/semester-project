using HeatManager.Core.Models.Producers;
using HeatManager.Core.Models.Schedules;
using HeatManager.Core.Models.SourceData;
using HeatManager.Core.Services;
using HeatManager.Core.Services.Optimizers;
using HeatManager.Core.Models.Resources;
using Moq;
using Xunit;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System;
using System.Collections.Immutable;
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
    private readonly Mock<Resource> _mockResource;

    public DefaultOptimizerTest()
    {
        _mockAssetManager = new Mock<IAssetManager>();
        _mockSourceDataProvider = new Mock<ISourceDataProvider>();
        _mockOptimizerSettings = new Mock<IOptimizerSettings>();
        _mockOptimizerStrategy = new Mock<IOptimizerStrategy>(); 
        _mockResultManager = new Mock<object>();
        _mockResource = new Mock<Resource>();

        _optimizer = new DefaultOptimizer(
            _mockAssetManager.Object,
            _mockSourceDataProvider.Object,
            _mockOptimizerSettings.Object,
            _mockOptimizerStrategy.Object,
            _mockResultManager.Object);
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
        var allUnits = new ObservableCollection<HeatProductionUnitBase>
        {
            new HeatProductionUnit { Name = "Unit1", Resource = _mockResource.Object },
            new HeatProductionUnit { Name = "Unit2", Resource = _mockResource.Object },
            new HeatProductionUnit { Name = "Unit3", Resource = _mockResource.Object }
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
            new HeatProductionUnit { Name = "Unit1", Resource = _mockResource.Object },
            new HeatProductionUnit { Name = "Unit2", Resource = _mockResource.Object }
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
            new HeatProductionUnit { Name = "Unit1", Cost = 2, Emissions = 1, Resource = _mockResource.Object },
            new HeatProductionUnit { Name = "Unit2", Cost = 1, Emissions = 2, Resource = _mockResource.Object }
        };
        var strategy = new OptimizerStrategy(true);
        var mockSourceDataPoint = new Mock<SourceDataPoint>();
        mockSourceDataPoint.Setup(s => s.ElectricityPrice).Returns(0);

        // Act
        var result = DefaultOptimizer.GetHeatSourcePriorityList(units, mockSourceDataPoint.Object, strategy);

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
            new HeatProductionUnit { Name = "Unit1", Cost = 1, Emissions = 2, Resource = _mockResource.Object },
            new HeatProductionUnit { Name = "Unit2", Cost = 2, Emissions = 1, Resource = _mockResource.Object }
        };
        var strategy = new OptimizerStrategy(false);
        var mockSourceDataPoint = new Mock<SourceDataPoint>();
        mockSourceDataPoint.Setup(s => s.ElectricityPrice).Returns(0);

        // Act
        var result = DefaultOptimizer.GetHeatSourcePriorityList(units, mockSourceDataPoint.Object, strategy);

        // Assert
        Assert.Equal("Unit2", result.First().Name); // Lower emissions unit should be first
        Assert.Equal("Unit1", result.Last().Name);
    }

    [Fact]
    public void GetHeatSourcePriorityList_WithElectricityBasedUnits_AdjustsCosts()
    {
        // Arrange
        var mockElectricityResource = new Mock<Resource>();
        mockElectricityResource.Setup(r => r.Name).Returns("Electricity");
        
        var unit = new HeatProductionUnit 
        { 
            Name = "Unit1", 
            Cost = 1, 
            Emissions = 1,
            Resource = mockElectricityResource.Object
        };
        
        var strategy = new OptimizerStrategy(true);
        var mockSourceDataPoint = new Mock<SourceDataPoint>();
        mockSourceDataPoint.Setup(s => s.ElectricityPrice).Returns(2);

        // Act
        var result = DefaultOptimizer.GetHeatSourcePriorityList(new[] { unit }, mockSourceDataPoint.Object, strategy);

        // Assert
        Assert.Equal(3, result.First().Cost); // Original cost + electricity price
    }

    [Fact]
    public void GetHeatSourcePriorityList_WithMixedResourceTypes_HandlesCorrectly()
    {
        // Arrange
        var mockGasResource = new Mock<Resource>();
        mockGasResource.Setup(r => r.Type).Returns(ResourceType.Gas);
        
        var mockElectricityResource = new Mock<Resource>();
        mockElectricityResource.Setup(r => r.Type).Returns(ResourceType.Electricity);
        
        var units = new List<HeatProductionUnit>
        {
            new HeatProductionUnit { Name = "GasUnit", Cost = 2, Emissions = 1, Resource = mockGasResource.Object },
            new HeatProductionUnit { Name = "ElectricUnit", Cost = 1, Emissions = 2, Resource = mockElectricityResource.Object }
        };
        
        var strategy = new OptimizerStrategy(true);
        var mockSourceDataPoint = new Mock<SourceDataPoint>();
        mockSourceDataPoint.Setup(s => s.ElectricityPrice).Returns(1);

        // Act
        var result = DefaultOptimizer.GetHeatSourcePriorityList(units, mockSourceDataPoint.Object, strategy);

        // Assert
        Assert.Equal("ElectricUnit", result.First().Name); // Should be first despite higher emissions due to lower cost
    }

    [Fact]
    public void GetHeatSourcePriorityList_WithNegativeCost_HandlesCorrectly()
    {
        // Arrange
        var units = new List<HeatProductionUnit>
        {
            new HeatProductionUnit { Name = "Unit1", Cost = -1, Emissions = 1, Resource = _mockResource.Object },
            new HeatProductionUnit { Name = "Unit2", Cost = 1, Emissions = 1, Resource = _mockResource.Object }
        };
        
        var strategy = new OptimizerStrategy(true);
        var mockSourceDataPoint = new Mock<SourceDataPoint>();
        mockSourceDataPoint.Setup(s => s.ElectricityPrice).Returns(0);

        // Act
        var result = DefaultOptimizer.GetHeatSourcePriorityList(units, mockSourceDataPoint.Object, strategy);

        // Assert
        Assert.Equal("Unit1", result.First().Name); // Negative cost should be first
    }

    [Fact]
    public void GetHeatSourcePriorityList_WithEqualCosts_OrdersByEmissions()
    {
        // Arrange
        var units = new List<HeatProductionUnit>
        {
            new HeatProductionUnit { Name = "Unit1", Cost = 1, Emissions = 2, Resource = _mockResource.Object },
            new HeatProductionUnit { Name = "Unit2", Cost = 1, Emissions = 1, Resource = _mockResource.Object }
        };
        
        var strategy = new OptimizerStrategy(true);
        var mockSourceDataPoint = new Mock<SourceDataPoint>();
        mockSourceDataPoint.Setup(s => s.ElectricityPrice).Returns(0);

        // Act
        var result = DefaultOptimizer.GetHeatSourcePriorityList(units, mockSourceDataPoint.Object, strategy);

        // Assert
        Assert.Equal("Unit2", result.First().Name); // Lower emissions should be first when costs are equal
    }

    [Fact]
    public void GetHeatSourcePriorityList_WithElectricityProduction_AdjustsCostsCorrectly()
    {
        // Arrange
        var mockElectricityResource = new Mock<Resource>();
        mockElectricityResource.Setup(r => r.Type).Returns(ResourceType.Electricity);
        mockElectricityResource.Setup(r => r.Name).Returns("Electricity");
        
        var mockUnit = new Mock<ElectricityProductionUnit>();
        mockUnit.Setup(u => u.Name).Returns("Unit1");
        mockUnit.SetupProperty(u => u.Cost, (decimal)1);
        mockUnit.Setup(u => u.Emissions).Returns(1);
        mockUnit.Setup(u => u.Resource).Returns(mockElectricityResource.Object);
        mockUnit.Setup(u => u.MaxHeatProduction).Returns(3);
        mockUnit.Setup(u => u.MaxElectricity).Returns(3);
        
        // Create a new mock for the cloned unit
        var mockClonedUnit = new Mock<ElectricityProductionUnit>();
        mockClonedUnit.Setup(u => u.Name).Returns("Unit1");
        mockClonedUnit.SetupProperty(u => u.Cost, (decimal)1);
        mockClonedUnit.Setup(u => u.Emissions).Returns(1);
        mockClonedUnit.Setup(u => u.Resource).Returns(mockElectricityResource.Object);
        mockClonedUnit.Setup(u => u.MaxHeatProduction).Returns(3);
        mockClonedUnit.Setup(u => u.MaxElectricity).Returns(3);
        mockClonedUnit.Setup(u => u.Clone()).Returns(mockClonedUnit.Object);
        
        // Setup Clone to return the new mock
        mockUnit.Setup(u => u.Clone()).Returns(mockClonedUnit.Object);
        
        var units = new List<HeatProductionUnitBase> { mockUnit.Object };
        var strategy = new OptimizerStrategy(true);
        var mockSourceDataPoint = new Mock<SourceDataPoint>();
        mockSourceDataPoint.Setup(s => s.ElectricityPrice).Returns(2);
        mockSourceDataPoint.Setup(s => s.HeatDemand).Returns(2);

        // Act
        var result = DefaultOptimizer.GetHeatSourcePriorityList(units, mockSourceDataPoint.Object, strategy);

        // Assert
        var resultUnit = result.First();
        Assert.Equal("Unit1", resultUnit.Name);
        Assert.Equal(-1, resultUnit.Cost); // Original cost (1) - electricity price (2) = -1
    }

    [Fact]
    public void GetHeatSourcePriorityList_WithMixedElectricityUnits_AdjustsCostsCorrectly()
    {
        // Arrange
        var mockElectricityResource = new Mock<Resource>();
        mockElectricityResource.Setup(r => r.Type).Returns(ResourceType.Electricity);
        mockElectricityResource.Setup(r => r.Name).Returns("Electricity");
        
        // Unit that only consumes electricity
        var mockConsumingUnit = new Mock<HeatProductionUnit>();
        mockConsumingUnit.Setup(u => u.Name).Returns("ConsumingUnit");
        mockConsumingUnit.Setup(u => u.Cost).Returns((decimal)1);
        mockConsumingUnit.Setup(u => u.Emissions).Returns(1);
        mockConsumingUnit.Setup(u => u.Resource).Returns(mockElectricityResource.Object);
        mockConsumingUnit.Setup(u => u.MaxHeatProduction).Returns(3);

        
        // Create a new mock for the cloned consuming unit
        var mockClonedConsumingUnit = new Mock<HeatProductionUnit>();
        mockClonedConsumingUnit.Setup(u => u.Name).Returns("ConsumingUnit");
        mockClonedConsumingUnit.SetupProperty(u => u.Cost, (decimal)1);
        mockClonedConsumingUnit.Setup(u => u.Emissions).Returns(1);
        mockClonedConsumingUnit.Setup(u => u.Resource).Returns(mockElectricityResource.Object);
        mockClonedConsumingUnit.Setup(u => u.MaxHeatProduction).Returns(3);
        mockClonedConsumingUnit.Setup(u => u.Clone()).Returns(mockClonedConsumingUnit.Object);
        
        mockConsumingUnit.Setup(u => u.Clone()).Returns(mockClonedConsumingUnit.Object);
        
        // Unit that produces electricity
        var mockProducingUnit = new Mock<ElectricityProductionUnit>();
        mockProducingUnit.Setup(u => u.Name).Returns("ProducingUnit");
        mockProducingUnit.SetupProperty(u => u.Cost, (decimal)1);
        mockProducingUnit.Setup(u => u.Emissions).Returns(1);
        mockProducingUnit.Setup(u => u.Resource).Returns(mockElectricityResource.Object);
        mockProducingUnit.Setup(u => u.MaxHeatProduction).Returns(3);
        mockProducingUnit.Setup(u => u.MaxElectricity).Returns(3);
        
        // Create a new mock for the cloned producing unit
        var mockClonedProducingUnit = new Mock<ElectricityProductionUnit>();
        mockClonedProducingUnit.Setup(u => u.Name).Returns("ProducingUnit");
        mockClonedProducingUnit.SetupProperty(u => u.Cost, (decimal)1);
        mockClonedProducingUnit.Setup(u => u.Emissions).Returns(1);
        mockClonedProducingUnit.Setup(u => u.Resource).Returns(mockElectricityResource.Object);
        mockClonedProducingUnit.Setup(u => u.MaxElectricity).Returns(3);
        mockClonedProducingUnit.Setup(u => u.MaxHeatProduction).Returns(3);

        mockClonedProducingUnit.Setup(u => u.Clone()).Returns(mockClonedProducingUnit.Object);
        
        mockProducingUnit.Setup(u => u.Clone()).Returns(mockClonedProducingUnit.Object);
        
        var units = new List<HeatProductionUnitBase> { mockConsumingUnit.Object, mockProducingUnit.Object };
        var strategy = new OptimizerStrategy(true);
        var mockSourceDataPoint = new Mock<SourceDataPoint>();
        mockSourceDataPoint.Setup(s => s.ElectricityPrice).Returns((decimal)2);
        mockSourceDataPoint.Setup(s => s.HeatDemand).Returns(2);

        // Act
        var result = DefaultOptimizer.GetHeatSourcePriorityList(units, mockSourceDataPoint.Object, strategy);

        var amazingName = result.ElementAt(0); 
        var amazingName2 = result.ElementAt(1); 

        // Assert
        var resultList = result.ToList();
        Assert.Equal(2, resultList.Count);
        
        // Consuming unit should have increased cost
        var consumingUnit = resultList.First(u => u.Name == "ConsumingUnit");
        Assert.Equal((decimal)3, amazingName2.Cost); // Original cost (1) + electricity price (2) = 3
        
        // Producing unit should have decreased cost
        var producingUnit = resultList.First(u => u.Name == "ProducingUnit");
        Assert.Equal((decimal)-1, amazingName.Cost); // Original cost (1) - electricity price (2) = -1
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
        var allUnits = new ObservableCollection<HeatProductionUnitBase>
        {
            new HeatProductionUnit { Name = "Unit1", Resource = _mockResource.Object }
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
        var allUnits = new ObservableCollection<HeatProductionUnitBase>
        {
            new HeatProductionUnit { Name = "Unit1", Resource = _mockResource.Object }
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
        var mockUnit = new Mock<HeatProductionUnit>();
        mockUnit.Setup(u => u.Name).Returns("Unit1");
        mockUnit.Setup(u => u.MaxHeatProduction).Returns(100);
        mockUnit.Setup(u => u.Cost).Returns(1);
        mockUnit.Setup(u => u.Emissions).Returns(1);
        mockUnit.Setup(u => u.ResourceConsumption).Returns(1);
        mockUnit.Setup(u => u.Resource).Returns(_mockResource.Object);

        var mockSourceDataPoint = new Mock<SourceDataPoint>();
        mockSourceDataPoint.Setup(s => s.HeatDemand).Returns(50);
        mockSourceDataPoint.Setup(s => s.ElectricityPrice).Returns(0);
        mockSourceDataPoint.Setup(s => s.TimeFrom).Returns(DateTime.Now);
        mockSourceDataPoint.Setup(s => s.TimeTo).Returns(DateTime.Now.AddHours(1));

        var sourceDataCollection = new Mock<SourceDataCollection>();
        sourceDataCollection.Setup(s => s.DataPoints).Returns(ImmutableList.Create(mockSourceDataPoint.Object));

        _mockSourceDataProvider.Setup(p => p.SourceDataCollection).Returns(sourceDataCollection.Object);
        _mockAssetManager.Setup(a => a.ProductionUnits).Returns(new ObservableCollection<HeatProductionUnitBase> { mockUnit.Object });
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
        var mockUnit1 = new Mock<HeatProductionUnitBase>();
        mockUnit1.Setup(u => u.Name).Returns("Unit1");
        mockUnit1.Setup(u => u.MaxHeatProduction).Returns(50);
        mockUnit1.Setup(u => u.Cost).Returns(1);
        mockUnit1.Setup(u => u.Emissions).Returns(1);
        mockUnit1.Setup(u => u.ResourceConsumption).Returns(1);
        mockUnit1.Setup(u => u.Resource).Returns(_mockResource.Object);

        var mockUnit2 = new Mock<HeatProductionUnit>();
        mockUnit2.Setup(u => u.Name).Returns("Unit2");
        mockUnit2.Setup(u => u.MaxHeatProduction).Returns(50);
        mockUnit2.Setup(u => u.Cost).Returns(2);
        mockUnit2.Setup(u => u.Emissions).Returns(1);
        mockUnit2.Setup(u => u.ResourceConsumption).Returns(1);
        mockUnit2.Setup(u => u.Resource).Returns(_mockResource.Object);

        var mockSourceDataPoint = new Mock<SourceDataPoint>();
        mockSourceDataPoint.Setup(s => s.HeatDemand).Returns(75);
        mockSourceDataPoint.Setup(s => s.ElectricityPrice).Returns(0);
        mockSourceDataPoint.Setup(s => s.TimeFrom).Returns(DateTime.Now);
        mockSourceDataPoint.Setup(s => s.TimeTo).Returns(DateTime.Now.AddHours(1));

        var sourceDataCollection = new Mock<SourceDataCollection>();
        sourceDataCollection.Setup(s => s.DataPoints).Returns(ImmutableList.Create(mockSourceDataPoint.Object));

        _mockSourceDataProvider.Setup(p => p.SourceDataCollection).Returns(sourceDataCollection.Object);
        _mockAssetManager.Setup(a => a.ProductionUnits).Returns(new ObservableCollection<HeatProductionUnitBase> { mockUnit1.Object, mockUnit2.Object });
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
        var mockUnit = new Mock<HeatProductionUnit>();
        mockUnit.Setup(u => u.Name).Returns("Unit1");
        mockUnit.Setup(u => u.MaxHeatProduction).Returns(50);
        mockUnit.Setup(u => u.Cost).Returns(1);
        mockUnit.Setup(u => u.Emissions).Returns(1);
        mockUnit.Setup(u => u.ResourceConsumption).Returns(1);
        mockUnit.Setup(u => u.Resource).Returns(_mockResource.Object);

        var mockSourceDataPoint = new Mock<SourceDataPoint>();
        mockSourceDataPoint.Setup(s => s.HeatDemand).Returns(100);
        mockSourceDataPoint.Setup(s => s.ElectricityPrice).Returns(0);
        mockSourceDataPoint.Setup(s => s.TimeFrom).Returns(DateTime.Now);
        mockSourceDataPoint.Setup(s => s.TimeTo).Returns(DateTime.Now.AddHours(1));

        var sourceDataCollection = new Mock<SourceDataCollection>();
        sourceDataCollection.Setup(s => s.DataPoints).Returns(ImmutableList.Create(mockSourceDataPoint.Object));

        _mockSourceDataProvider.Setup(p => p.SourceDataCollection).Returns(sourceDataCollection.Object);
        _mockAssetManager.Setup(a => a.ProductionUnits).Returns(new ObservableCollection<HeatProductionUnitBase> { mockUnit.Object });
        _mockOptimizerSettings.Setup(s => s.GetActiveUnitsNames()).Returns(new List<string> { "Unit1" });

        // Act
        _optimizer.Optimize();

        // Assert
        // Note: Since we don't have access to the result manager, we can't verify the actual schedule
        // This test is more of a smoke test to ensure the method runs without exceptions
    }
}
