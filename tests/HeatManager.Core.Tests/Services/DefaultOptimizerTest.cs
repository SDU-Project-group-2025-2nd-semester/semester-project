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
    private readonly Mock<IBasicResource> _mockResource;

    public DefaultOptimizerTest()
    {
        _mockAssetManager = new Mock<IAssetManager>();
        _mockSourceDataProvider = new Mock<ISourceDataProvider>();
        _mockOptimizerSettings = new Mock<IOptimizerSettings>();
        _mockOptimizerStrategy = new Mock<IOptimizerStrategy>(); 
        _mockResultManager = new Mock<object>();
        _mockResource = new Mock<IBasicResource>();

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
        var allUnits = new ObservableCollection<IHeatProductionUnit>
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
        var units = new List<IHeatProductionUnit>
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
        var units = new List<IElectricityProductionUnit>
        {
            new Mock<IElectricityProductionUnit>().Object,
            new Mock<IElectricityProductionUnit>().Object
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
        var units = new List<IHeatProductionUnit>
        {
            new HeatProductionUnit { Name = "Unit1", Cost = 2, Emissions = 1, Resource = _mockResource.Object },
            new HeatProductionUnit { Name = "Unit2", Cost = 1, Emissions = 2, Resource = _mockResource.Object }
        };
        var strategy = new OptimizerStrategy(true);
        var mockSourceDataPoint = new Mock<ISourceDataPoint>();
        mockSourceDataPoint.Setup(s => s.ElectricityPrice).Returns(0);

        // Act
        var result = _optimizer.GetHeatSourcePriorityList(units, mockSourceDataPoint.Object, strategy);

        // Assert
        Assert.Equal("Unit2", result.First().Name); // Cheaper unit should be first
        Assert.Equal("Unit1", result.Last().Name);
    }

    [Fact]
    public void GetHeatSourcePriorityList_WithCo2Optimization_OrdersByEmissions()
    {
        // Arrange
        var units = new List<IHeatProductionUnit>
        {
            new HeatProductionUnit { Name = "Unit1", Cost = 1, Emissions = 2, Resource = _mockResource.Object },
            new HeatProductionUnit { Name = "Unit2", Cost = 2, Emissions = 1, Resource = _mockResource.Object }
        };
        var strategy = new OptimizerStrategy(false);
        var mockSourceDataPoint = new Mock<ISourceDataPoint>();
        mockSourceDataPoint.Setup(s => s.ElectricityPrice).Returns(0);

        // Act
        var result = _optimizer.GetHeatSourcePriorityList(units, mockSourceDataPoint.Object, strategy);

        // Assert
        Assert.Equal("Unit2", result.First().Name); // Lower emissions unit should be first
        Assert.Equal("Unit1", result.Last().Name);
    }

    [Fact]
    public void GetHeatSourcePriorityList_WithElectricityBasedUnits_AdjustsCosts()
    {
        // Arrange
        var mockElectricityResource = new Mock<IBasicResource>();
        mockElectricityResource.Setup(r => r.Name).Returns("Electricity");
        
        var unit = new HeatProductionUnit 
        { 
            Name = "Unit1", 
            Cost = 1, 
            Emissions = 1,
            Resource = mockElectricityResource.Object
        };
        
        var strategy = new OptimizerStrategy(true);
        var mockSourceDataPoint = new Mock<ISourceDataPoint>();
        mockSourceDataPoint.Setup(s => s.ElectricityPrice).Returns(2);

        // Act
        var result = _optimizer.GetHeatSourcePriorityList(new[] { unit }, mockSourceDataPoint.Object, strategy);

        // Assert
        Assert.Equal(3, result.First().Cost); // Original cost + electricity price
    }
}
