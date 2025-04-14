using Xunit;
using Moq;
using HeatManager.Core.Models.Producers;
using HeatManager.Core.Models.Schedules;
using HeatManager.Core.Models.SourceData;
using HeatManager.Core.Models.Resources;
using HeatManager.Core.Services;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using HeatManager.Core.Services.Optimizers;
using System.Linq;
using System;
using System.Threading.Tasks;
using System.Collections.Immutable;

namespace HeatManager.Core.Tests.Services.Optimizers
{
    public class DefaultOptimizerTests
    {
        private readonly Mock<IAssetManager> _mockAssetManager;
        private readonly Mock<IResourceManager> _mockResourceManager;
        private readonly Mock<ISourceDataProvider> _mockSourceDataProvider;
        private readonly Mock<IOptimizerSettings> _mockOptimizerSettings;
        private readonly Mock<IOptimizerStrategy> _mockOptimizerStrategy;
        
        public DefaultOptimizerTests()
        {
            _mockAssetManager = new Mock<IAssetManager>();
            _mockResourceManager = new Mock<IResourceManager>();
            _mockSourceDataProvider = new Mock<ISourceDataProvider>();
            _mockOptimizerSettings = new Mock<IOptimizerSettings>();
            _mockOptimizerStrategy = new Mock<IOptimizerStrategy>();
        }

        [Fact]
        public async Task OptimizeAsync_WithValidData_GeneratesCorrectSchedule()
        {
            // Arrange
            var optimizer = new DefaultOptimizer(
                _mockAssetManager.Object,
                _mockResourceManager.Object,
                _mockSourceDataProvider.Object,
                _mockOptimizerSettings.Object
            );

            var sourceDataPoints = new List<ISourceDataPoint>
            {
                CreateSourceDataPoint(DateTime.Now, DateTime.Now.AddHours(1), 5.0, 500m)
            };

            var productionUnits = new ObservableCollection<IHeatProductionUnit>
            {
                CreateHeatProductionUnit("GB1", 520m, 4.0, 0.9, "Gas", 175.0),
                CreateHeatProductionUnit("HP1", 60m, 6.0, 0.0, "Electricity", 0.0)
            };

            SetupMocks(sourceDataPoints, productionUnits);

            // Act
            await optimizer.OptimizeAsync();

            // Assert
            // Verify the schedule was created correctly
            // Add specific assertions based on expected optimization results
        }

        [Fact]
        public void GetHeatSourcePriorityList_PriceOptimization_SortsCorrectly()
        {
            // Arrange
            var optimizer = new DefaultOptimizer(
                _mockAssetManager.Object,
                _mockResourceManager.Object,
                _mockSourceDataProvider.Object,
                _mockOptimizerSettings.Object
            );

            var units = new List<IHeatProductionUnit>
            {
                CreateHeatProductionUnit("GB1", 520m, 4.0, 0.9, "Gas", 175.0),
                CreateHeatProductionUnit("HP1", 60m, 6.0, 0.0, "Electricity", 0.0)
            };

            var sourceDataPoint = CreateSourceDataPoint(DateTime.Now, DateTime.Now.AddHours(1), 5.0, 500m);

            _mockOptimizerStrategy.Setup(s => s.Optimization)
                .Returns(OptimizationType.PriceOptimization);

            // Act
            var priorityList = optimizer.GetHeatSourcePriorityList(units, sourceDataPoint, _mockOptimizerStrategy.Object);

            // Assert
            Assert.Equal("HP1", priorityList.First().Name);
            Assert.Equal("GB1", priorityList.Last().Name);
        }

        [Fact]
        public void GetHeatSourcePriorityList_CO2Optimization_SortsCorrectly()
        {
            // Arrange
            var optimizer = new DefaultOptimizer(
                _mockAssetManager.Object,
                _mockResourceManager.Object,
                _mockSourceDataProvider.Object,
                _mockOptimizerSettings.Object
            );

            var units = new List<IHeatProductionUnit>
            {
                CreateHeatProductionUnit("GB1", 520m, 4.0, 0.9, "Gas", 175.0),
                CreateHeatProductionUnit("HP1", 60m, 6.0, 0.0, "Electricity", 0.0)
            };

            var sourceDataPoint = CreateSourceDataPoint(DateTime.Now, DateTime.Now.AddHours(1), 5.0, 500m);

            _mockOptimizerStrategy.Setup(s => s.Optimization)
                .Returns(OptimizationType.Co2Optimization);

            // Act
            var priorityList = optimizer.GetHeatSourcePriorityList(units, sourceDataPoint, _mockOptimizerStrategy.Object);

            // Assert
            Assert.Equal("HP1", priorityList.First().Name);
            Assert.Equal("GB1", priorityList.Last().Name);
        }

        [Fact]
        public void GetAvailableUnits_ReturnsOnlyActiveUnits()
        {
            // Arrange
            var optimizer = new DefaultOptimizer(
                _mockAssetManager.Object,
                _mockResourceManager.Object,
                _mockSourceDataProvider.Object,
                _mockOptimizerSettings.Object
            );

            var allUnits = new ObservableCollection<IHeatProductionUnit>
            {
                CreateHeatProductionUnit("GB1", 520m, 4.0, 0.9, "Gas", 175.0),
                CreateHeatProductionUnit("HP1", 60m, 6.0, 0.0, "Electricity", 0.0)
            };

            _mockAssetManager.Setup(m => m.ProductionUnits).Returns(allUnits);
            _mockOptimizerSettings.Setup(s => s.GetActiveUnits())
                .Returns(new List<string> { "GB1" });

            // Act
            var availableUnits = optimizer.GetAvailableUnits(_mockAssetManager.Object, _mockOptimizerSettings.Object);

            // Assert
            Assert.Single(availableUnits);
            Assert.Equal("GB1", availableUnits.First().Name);
        }

        [Fact]
        public void ChangeOptimizationSettings_UpdatesSettings()
        {
            // Arrange
            var optimizer = new DefaultOptimizer(
                _mockAssetManager.Object,
                _mockResourceManager.Object,
                _mockSourceDataProvider.Object,
                _mockOptimizerSettings.Object
            );

            var newSettings = new Mock<IOptimizerSettings>();

            // Act
            optimizer.ChangeOptimizationSettings(newSettings.Object);

            // Assert
            // You would need to add a way to verify the settings were changed, 
            // possibly by adding a public property or method to check the current settings
        }

        private void SetupMocks(List<ISourceDataPoint> sourceDataPoints, ObservableCollection<IHeatProductionUnit> productionUnits)
        {
            var sourceDataCollection = new Mock<ISourceDataCollection>();
            sourceDataCollection.Setup(s => s.DataPoints).Returns(sourceDataPoints.ToImmutableList());
            
            _mockSourceDataProvider.Setup(s => s.SourceDataCollection)
                .Returns(sourceDataCollection.Object);

            _mockAssetManager.Setup(m => m.ProductionUnits)
                .Returns(productionUnits);

            _mockOptimizerSettings.Setup(s => s.GetActiveUnits())
                .Returns(productionUnits.Select(u => u.Name).ToList());
        }

        private IHeatProductionUnit CreateHeatProductionUnit(
            string name, 
            decimal cost, 
            double maxHeatProduction, 
            double resourceConsumption, 
            string resourceName, 
            double emissions)
        {
            var unit = new Mock<IHeatProductionUnit>();
            unit.Setup(u => u.Name).Returns(name);
            unit.Setup(u => u.Cost).Returns(cost);
            unit.Setup(u => u.MaxHeatProduction).Returns(maxHeatProduction);
            unit.Setup(u => u.ResourceConsumption).Returns(resourceConsumption);
            unit.Setup(u => u.Resource).Returns(new BasicResource { Name = resourceName });
            unit.Setup(u => u.Emissions).Returns(emissions);
            unit.Setup(u => u.Clone()).Returns(() => 
            {
                var clone = new Mock<IHeatProductionUnit>();
                clone.Setup(c => c.Name).Returns(name);
                clone.Setup(c => c.Cost).Returns(cost);
                clone.Setup(c => c.MaxHeatProduction).Returns(maxHeatProduction);
                clone.Setup(c => c.ResourceConsumption).Returns(resourceConsumption);
                clone.Setup(c => c.Resource).Returns(new BasicResource { Name = resourceName });
                clone.Setup(c => c.Emissions).Returns(emissions);
                return clone.Object;
            });
            return unit.Object;
        }

        private ISourceDataPoint CreateSourceDataPoint(
            DateTime timeFrom, 
            DateTime timeTo, 
            double heatDemand, 
            decimal electricityPrice)
        {
            var dataPoint = new Mock<ISourceDataPoint>();
            dataPoint.Setup(d => d.TimeFrom).Returns(timeFrom);
            dataPoint.Setup(d => d.TimeTo).Returns(timeTo);
            dataPoint.Setup(d => d.HeatDemand).Returns(heatDemand);
            dataPoint.Setup(d => d.ElectricityPrice).Returns(electricityPrice);
            return dataPoint.Object;
        }
    }
}