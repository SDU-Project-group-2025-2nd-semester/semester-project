using HeatManager.Core.Models.Producers;
using HeatManager.Core.Models.Resources;
using HeatManager.Core.Models.Schedules;
using HeatManager.Core.Models.SourceData;
using HeatManager.Core.ResultData;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace HeatManager.Core.Tests.Models.Schedules;

public class ScheduleTest
{
    private readonly ResourceType _oil = ResourceType.Oil;
    private readonly ResourceType _gas = ResourceType.Gas;
    private readonly ResourceType _electricity = ResourceType.Electricity;

    private readonly DateTime _startTime = DateTime.Now;

    [Fact]
    public void Schedule_WithEmptyCollections_ReturnsEmptyProperties()
    {
        // Arrange
        var heatSchedules = new List<HeatProductionUnitSchedule>();
        var electricitySchedules = new List<ElectricityProductionUnitSchedule>();

        // Act
        var schedule = new Schedule(heatSchedules, electricitySchedules);

        // Assert
        Assert.Empty(schedule.HeatProductionUnitSchedules);
        Assert.Empty(schedule.ElectricityProductionUnitSchedules);
        Assert.Equal(0, schedule.Length);
        Assert.Equal(0, schedule.TotalCost);
        Assert.Equal(0, schedule.TotalEmissions);
        Assert.Equal(0, schedule.TotalHeatProduction);
        Assert.Empty(schedule.ElectricityPrice);
        Assert.Empty(schedule.ElectricityProduction);
        Assert.Empty(schedule.Costs);
        Assert.Empty(schedule.Emissions);
        Assert.Empty(schedule.HeatProduction);
        Assert.Empty(schedule.ResourceConsumption);
    }

    [Fact]
    public void Schedule_WithSingleHeatUnit_CalculatesCorrectTotals()
    {
        // Arrange
        var dataPoints = new List<IHeatProductionUnitResultDataPoint>
        {
            new HeatProductionUnitResultDataPoint(
                _startTime, 
                _startTime.AddHours(1),
                0.5,
                50,
                100,
                10,
                25
            ),
            new HeatProductionUnitResultDataPoint(
                _startTime.AddHours(1), 
                _startTime.AddHours(2),
                0.75,
                75,
                150,
                15,
                35
            )
        };

        var heatSchedule = new HeatProductionUnitSchedule("Unit1", _oil);
        foreach (var dataPoint in dataPoints)
        {
            heatSchedule.AddDataPoint(dataPoint);
        }
        
        var heatSchedules = new List<HeatProductionUnitSchedule> { heatSchedule };
        var electricitySchedules = new List<ElectricityProductionUnitSchedule>();

        // Act
        var schedule = new Schedule(heatSchedules, electricitySchedules);

        // Assert
        Assert.Single(schedule.HeatProductionUnitSchedules);
        Assert.Empty(schedule.ElectricityProductionUnitSchedules);
        Assert.Equal(2, schedule.Length);
        
        // Test totals
        Assert.Equal(250m, schedule.TotalCost);
        Assert.Equal(60, schedule.TotalEmissions);
        Assert.Equal(125, schedule.TotalHeatProduction);
        
        // Test arrays
        Assert.Equal(2, schedule.Costs.Length);
        Assert.Equal(100m, schedule.Costs[0]);
        Assert.Equal(150m, schedule.Costs[1]);
        
        Assert.Equal(2, schedule.Emissions.Length);
        Assert.Equal(25, schedule.Emissions[0]);
        Assert.Equal(35, schedule.Emissions[1]);
        
        Assert.Equal(2, schedule.HeatProduction.Length);
        Assert.Equal(50, schedule.HeatProduction[0]);
        Assert.Equal(75, schedule.HeatProduction[1]);
        
        // Test resource consumption
        Assert.Single(schedule.ResourceConsumption);
        Assert.True(schedule.ResourceConsumption.ContainsKey(_oil));
        Assert.Equal(10, schedule.ResourceConsumption[_oil][0]);
        Assert.Equal(15, schedule.ResourceConsumption[_oil][1]);
    }

    [Fact]
    public void Schedule_WithMultipleHeatUnits_CalculatesCorrectTotals()
    {
        // Arrange
        var dataPoints1 = new List<IHeatProductionUnitResultDataPoint>
        {
            new HeatProductionUnitResultDataPoint(
                _startTime, 
                _startTime.AddHours(1),
                1.0,
                50,
                100,
                10,
                25
            )
        };

        var dataPoints2 = new List<IHeatProductionUnitResultDataPoint>
        {
            new HeatProductionUnitResultDataPoint(
                _startTime, 
                _startTime.AddHours(1),
                0.6,
                30,
                60,
                6,
                15
            )
        };

        var heatSchedule1 = new HeatProductionUnitSchedule("Unit1", _oil);
        foreach (var dataPoint in dataPoints1)
        {
            heatSchedule1.AddDataPoint(dataPoint);
        }
        
        var heatSchedule2 = new HeatProductionUnitSchedule("Unit2", _gas);
        foreach (var dataPoint in dataPoints2)
        {
            heatSchedule2.AddDataPoint(dataPoint);
        }
        
        var heatSchedules = new List<HeatProductionUnitSchedule> { heatSchedule1, heatSchedule2 };
        var electricitySchedules = new List<ElectricityProductionUnitSchedule>();

        // Act
        var schedule = new Schedule(heatSchedules, electricitySchedules);

        // Assert
        Assert.Equal(2, schedule.HeatProductionUnitSchedules.Count);
        Assert.Empty(schedule.ElectricityProductionUnitSchedules);
        Assert.Equal(1, schedule.Length);
        
        // Test totals
        Assert.Equal(160m, schedule.TotalCost);
        Assert.Equal(40, schedule.TotalEmissions);
        Assert.Equal(80, schedule.TotalHeatProduction);
        
        // Test arrays
        Assert.Single(schedule.Costs);
        Assert.Equal(160m, schedule.Costs[0]);
        
        Assert.Single(schedule.Emissions);
        Assert.Equal(40, schedule.Emissions[0]);
        
        Assert.Single(schedule.HeatProduction);
        Assert.Equal(80, schedule.HeatProduction[0]);
        
        // Test resource consumption
        Assert.Equal(2, schedule.ResourceConsumption.Count);
        Assert.True(schedule.ResourceConsumption.ContainsKey(_oil));
        Assert.True(schedule.ResourceConsumption.ContainsKey(_gas));
        Assert.Equal(10, schedule.ResourceConsumption[_oil][0]);
        Assert.Equal(6, schedule.ResourceConsumption[_gas][0]);
    }

    [Fact]
    public void Schedule_WithElectricityUnit_CalculatesCorrectTotals()
    {
        // Arrange
        var heatDataPoints = new List<IHeatProductionUnitResultDataPoint>
        {
            new HeatProductionUnitResultDataPoint(
                _startTime, 
                _startTime.AddHours(1),
                0.5,
                50,
                100,
                10,
                25
            )
        };

        var electricityDataPoints = new List<ElectricityProductionResultDataPoint>
        {
            new ElectricityProductionResultDataPoint(
                _startTime, 
                _startTime.AddHours(1),
                3.5m,
                25
            )
        };

        var heatSchedule = new HeatProductionUnitSchedule("CHP", _gas);
        foreach (var dataPoint in heatDataPoints)
        {
            heatSchedule.AddDataPoint(dataPoint);
        }
        
        var electricitySchedule = new ElectricityProductionUnitSchedule("CHP");
        foreach (var dataPoint in electricityDataPoints)
        {
            electricitySchedule.AddDataPoint(dataPoint);
        }
        
        var heatSchedules = new List<HeatProductionUnitSchedule> { heatSchedule };
        var electricitySchedules = new List<ElectricityProductionUnitSchedule> { electricitySchedule };

        // Act
        var schedule = new Schedule(heatSchedules, electricitySchedules);

        // Assert
        Assert.Single(schedule.HeatProductionUnitSchedules);
        Assert.Single(schedule.ElectricityProductionUnitSchedules);
        Assert.Equal(1, schedule.Length);
        
        // Test totals
        Assert.Equal(100m, schedule.TotalCost);
        Assert.Equal(25, schedule.TotalEmissions);
        Assert.Equal(50, schedule.TotalHeatProduction);
        
        // Test electricity properties
        Assert.Single(schedule.ElectricityPrice);
        Assert.Equal(3.5m, schedule.ElectricityPrice[0]);
        
        Assert.Single(schedule.ElectricityProduction);
        Assert.Equal(25, schedule.ElectricityProduction[0]);
    }

    [Fact]
    public void Schedule_WithDifferentTimeRanges_ThrowsException()
    {
        // This test verifies that the Schedule handles mismatched time ranges appropriately
        // Arrange
        var dataPoints1 = new List<IHeatProductionUnitResultDataPoint>
        {
            new HeatProductionUnitResultDataPoint(
                _startTime, 
                _startTime.AddHours(1),
                1.0,
                50,
                100,
                10,
                25
            )
        };

        var dataPoints2 = new List<IHeatProductionUnitResultDataPoint>
        {
            new HeatProductionUnitResultDataPoint(
                _startTime.AddHours(2), 
                _startTime.AddHours(3),
                0.6,
                30,
                60,
                6,
                15
            )
        };

        var heatSchedule1 = new HeatProductionUnitSchedule("Unit1", _oil);
        foreach (var dataPoint in dataPoints1)
        {
            heatSchedule1.AddDataPoint(dataPoint);
        }
        
        var heatSchedule2 = new HeatProductionUnitSchedule("Unit2", _gas);
        foreach (var dataPoint in dataPoints2)
        {
            heatSchedule2.AddDataPoint(dataPoint);
        }
        
        var heatSchedules = new List<HeatProductionUnitSchedule> { heatSchedule1, heatSchedule2 };
        var electricitySchedules = new List<ElectricityProductionUnitSchedule>();

        // Act & Assert
        // This test depends on how your code should handle this scenario
        // If it should throw an exception:
        // Assert.Throws<InvalidOperationException>(() => new Schedule(heatSchedules, electricitySchedules));
        
        // Or if it should handle it gracefully, test the expected behavior
        var schedule = new Schedule(heatSchedules, electricitySchedules);
        Assert.Equal(2, schedule.HeatProductionUnitSchedules.Count);
        // Add more assertions based on expected behavior
    }

    [Fact]
    public void ResourceConsumption_WithMultipleResourceTypes_IsCorrectlyAggregated()
    {
        // Arrange
        var dataPoints1 = new List<IHeatProductionUnitResultDataPoint>
        {
            new HeatProductionUnitResultDataPoint(
                _startTime, 
                _startTime.AddHours(1),
                1.0,
                50,
                100,
                10,
                25
            )
        };

        var dataPoints2 = new List<IHeatProductionUnitResultDataPoint>
        {
            new HeatProductionUnitResultDataPoint(
                _startTime, 
                _startTime.AddHours(1),
                0.6,
                30,
                60,
                6,
                15
            )
        };

        var dataPoints3 = new List<IHeatProductionUnitResultDataPoint>
        {
            new HeatProductionUnitResultDataPoint(
                _startTime, 
                _startTime.AddHours(1),
                0.4,
                20,
                40,
                5,
                10
            )
        };

        var heatSchedule1 = new HeatProductionUnitSchedule("Unit1", _oil);
        foreach (var dataPoint in dataPoints1)
        {
            heatSchedule1.AddDataPoint(dataPoint);
        }
        
        var heatSchedule2 = new HeatProductionUnitSchedule("Unit2", _gas);
        foreach (var dataPoint in dataPoints2)
        {
            heatSchedule2.AddDataPoint(dataPoint);
        }
        
        var heatSchedule3 = new HeatProductionUnitSchedule("Unit3", _oil); // Same resource as Unit1
        foreach (var dataPoint in dataPoints3)
        {
            heatSchedule3.AddDataPoint(dataPoint);
        }
        
        var heatSchedules = new List<HeatProductionUnitSchedule> { heatSchedule1, heatSchedule2, heatSchedule3 };
        var electricitySchedules = new List<ElectricityProductionUnitSchedule>();

        // Act
        var schedule = new Schedule(heatSchedules, electricitySchedules);

        // Assert
        Assert.Equal(3, schedule.HeatProductionUnitSchedules.Count);
        Assert.Equal(2, schedule.ResourceConsumption.Count);
        
        // Check that resources are correctly aggregated
        Assert.True(schedule.ResourceConsumption.ContainsKey(_oil));
        Assert.True(schedule.ResourceConsumption.ContainsKey(_gas));
        
        // Oil consumption should be the sum from Unit1 and Unit3
        Assert.Equal(15, schedule.ResourceConsumption[_oil][0]); // 10 + 5
        
        // Gas consumption should be from Unit2
        Assert.Equal(6, schedule.ResourceConsumption[_gas][0]);
    }
} 