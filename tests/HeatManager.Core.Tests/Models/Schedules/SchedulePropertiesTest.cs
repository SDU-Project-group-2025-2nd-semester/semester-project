using HeatManager.Core.Models.Resources;
using HeatManager.Core.Models.Schedules;
using HeatManager.Core.ResultData;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace HeatManager.Core.Tests.Models.Schedules;

public class SchedulePropertiesTest
{
    private readonly ResourceType _oil = ResourceType.Oil;
    private readonly ResourceType _gas = ResourceType.Gas;
    private readonly DateTime _startTime = DateTime.Now;

    [Fact]
    public void GetCostsByHour_ProperlyAggregatesCostsFromMultipleUnits()
    {
        // Arrange
        var heatSchedule1 = CreateHeatSchedule("Unit1", _oil, new decimal[] { 100m, 200m }, new double[] { 25, 35 }, new double[] { 50, 75 });
        var heatSchedule2 = CreateHeatSchedule("Unit2", _oil, new decimal[] { 50m, 75m }, new double[] { 15, 20 }, new double[] { 30, 45 });

        var heatSchedules = new List<HeatProductionUnitSchedule> { heatSchedule1, heatSchedule2 };
        var electricitySchedules = new List<ElectricityProductionUnitSchedule>();

        // Act
        var schedule = new Schedule(heatSchedules, electricitySchedules);

        // Assert
        Assert.Equal(2, schedule.Costs.Length);
        Assert.Equal(150m, schedule.Costs[0]); // 100 + 50
        Assert.Equal(275m, schedule.Costs[1]); // 200 + 75
        Assert.Equal(425m, schedule.TotalCost); // 150 + 275
    }

    [Fact]
    public void GetEmissionsByHour_ProperlyAggregatesEmissionsFromMultipleUnits()
    {
        // Arrange
        var heatSchedule1 = CreateHeatSchedule("Unit1", _oil, new decimal[] { 100m, 200m }, new double[] { 25, 35 }, new double[] { 50, 75 });
        var heatSchedule2 = CreateHeatSchedule("Unit2", _oil, new decimal[] { 50m, 75m }, new double[] { 15, 20 }, new double[] { 30, 45 });

        var heatSchedules = new List<HeatProductionUnitSchedule> { heatSchedule1, heatSchedule2 };
        var electricitySchedules = new List<ElectricityProductionUnitSchedule>();

        // Act
        var schedule = new Schedule(heatSchedules, electricitySchedules);

        // Assert
        Assert.Equal(2, schedule.Emissions.Length);
        Assert.Equal(40.0, schedule.Emissions[0]); // 25 + 15
        Assert.Equal(55.0, schedule.Emissions[1]); // 35 + 20
        Assert.Equal(95.0, schedule.TotalEmissions); // 40 + 55
    }

    [Fact]
    public void GetHeatProductionByHour_ProperlyAggregatesHeatFromMultipleUnits()
    {
        // Arrange
        var heatSchedule1 = CreateHeatSchedule("Unit1", _oil, new decimal[] { 100m, 200m }, new double[] { 25, 35 }, new double[] { 50, 75 });
        var heatSchedule2 = CreateHeatSchedule("Unit2", _oil, new decimal[] { 50m, 75m }, new double[] { 15, 20 }, new double[] { 30, 45 });

        var heatSchedules = new List<HeatProductionUnitSchedule> { heatSchedule1, heatSchedule2 };
        var electricitySchedules = new List<ElectricityProductionUnitSchedule>();

        // Act
        var schedule = new Schedule(heatSchedules, electricitySchedules);

        // Assert
        Assert.Equal(2, schedule.HeatProduction.Length);
        Assert.Equal(80.0, schedule.HeatProduction[0]); // 50 + 30
        Assert.Equal(120.0, schedule.HeatProduction[1]); // 75 + 45
        Assert.Equal(200.0, schedule.TotalHeatProduction); // 80 + 120
    }

    [Fact]
    public void ElectricityPrice_WithNoElectricitySchedules_ReturnsEmptyArray()
    {
        // Arrange
        var heatSchedules = new List<HeatProductionUnitSchedule>();
        var electricitySchedules = new List<ElectricityProductionUnitSchedule>();

        // Act
        var schedule = new Schedule(heatSchedules, electricitySchedules);

        // Assert
        Assert.Empty(schedule.ElectricityPrice);
    }

    [Fact]
    public void ElectricityPrice_WithElectricitySchedule_ReturnsCorrectPrices()
    {
        // Arrange
        var electricitySchedule = CreateElectricitySchedule("CHP", new decimal[] { 3.5m, 4.2m }, new double[] { 25, 30 });
        
        var heatSchedules = new List<HeatProductionUnitSchedule>();
        var electricitySchedules = new List<ElectricityProductionUnitSchedule> { electricitySchedule };

        // Act
        var schedule = new Schedule(heatSchedules, electricitySchedules);

        // Assert
        Assert.Equal(2, schedule.ElectricityPrice.Length);
        Assert.Equal(3.5m, schedule.ElectricityPrice[0]);
        Assert.Equal(4.2m, schedule.ElectricityPrice[1]);
    }

    [Fact]
    public void ElectricityProduction_WithNoElectricitySchedules_ReturnsEmptyArray()
    {
        // Arrange
        var heatSchedules = new List<HeatProductionUnitSchedule>();
        var electricitySchedules = new List<ElectricityProductionUnitSchedule>();

        // Act
        var schedule = new Schedule(heatSchedules, electricitySchedules);

        // Assert
        Assert.Empty(schedule.ElectricityProduction);
    }

    [Fact]
    public void ElectricityProduction_WithMultipleSchedules_AggregatesCorrectly()
    {
        // Arrange
        var electricitySchedule1 = CreateElectricitySchedule("CHP1", new decimal[] { 3.5m, 4.2m }, new double[] { 25, 30 });
        var electricitySchedule2 = CreateElectricitySchedule("CHP2", new decimal[] { 3.5m, 4.2m }, new double[] { 15, 20 });
        
        var heatSchedules = new List<HeatProductionUnitSchedule>();
        var electricitySchedules = new List<ElectricityProductionUnitSchedule> 
        { 
            electricitySchedule1, 
            electricitySchedule2 
        };

        // Act
        var schedule = new Schedule(heatSchedules, electricitySchedules);

        // Assert
        Assert.Equal(2, schedule.ElectricityProduction.Length);
        Assert.Equal(40.0, schedule.ElectricityProduction[0]); // 25 + 15
        Assert.Equal(50.0, schedule.ElectricityProduction[1]); // 30 + 20
    }

    [Fact]
    public void ResourceConsumption_WithMultipleResourcesAndUnits_AggregatesCorrectly()
    {
        // Arrange
        var oil = ResourceType.Oil;
        var gas = ResourceType.Gas;

        // Create schedules with different resources
        var heatSchedule1 = CreateHeatScheduleWithResources("Unit1", oil, 
            new decimal[] { 100, 200 }, new double[] { 25, 35 }, new double[] { 50, 75 }, new double[] { 10, 15 });
        
        var heatSchedule2 = CreateHeatScheduleWithResources("Unit2", gas, 
            new decimal[] { 60, 80 }, new double[] { 15, 25 }, new double[] { 30, 40 }, new double[] { 5, 8 });
        
        var heatSchedule3 = CreateHeatScheduleWithResources("Unit3", oil, 
            new decimal[] { 40, 60 }, new double[] { 10, 15 }, new double[] { 20, 30 }, new double[] { 7, 9 });

        var heatSchedules = new List<HeatProductionUnitSchedule> 
        { 
            heatSchedule1, 
            heatSchedule2,
            heatSchedule3
        };
        var electricitySchedules = new List<ElectricityProductionUnitSchedule>();

        // Act
        var schedule = new Schedule(heatSchedules, electricitySchedules);

        // Assert
        Assert.Equal(2, schedule.ResourceConsumption.Count);
        
        // Check oil consumption (should be sum of unit1 and unit3)
        Assert.True(schedule.ResourceConsumption.ContainsKey(oil));
        Assert.Equal(17.0, schedule.ResourceConsumption[oil][0]); // 10 + 7
        Assert.Equal(24.0, schedule.ResourceConsumption[oil][1]); // 15 + 9
        
        // Check gas consumption (should be from unit2)
        Assert.True(schedule.ResourceConsumption.ContainsKey(gas));
        Assert.Equal(5.0, schedule.ResourceConsumption[gas][0]);
        Assert.Equal(8.0, schedule.ResourceConsumption[gas][1]);
    }

    [Fact]
    public void GetCostsByHour_WithEmptySchedules_ReturnsEmptyArray()
    {
        // Arrange
        var heatSchedules = new List<HeatProductionUnitSchedule>();
        var electricitySchedules = new List<ElectricityProductionUnitSchedule>();

        // Act
        var schedule = new Schedule(heatSchedules, electricitySchedules);

        // Assert
        Assert.Empty(schedule.Costs);
        Assert.Equal(0m, schedule.TotalCost);
    }

    [Fact]
    public void GetEmissionsByHour_WithEmptySchedules_ReturnsEmptyArray()
    {
        // Arrange
        var heatSchedules = new List<HeatProductionUnitSchedule>();
        var electricitySchedules = new List<ElectricityProductionUnitSchedule>();

        // Act
        var schedule = new Schedule(heatSchedules, electricitySchedules);

        // Assert
        Assert.Empty(schedule.Emissions);
        Assert.Equal(0.0, schedule.TotalEmissions);
    }

    [Fact]
    public void GetHeatProductionByHour_WithEmptySchedules_ReturnsEmptyArray()
    {
        // Arrange
        var heatSchedules = new List<HeatProductionUnitSchedule>();
        var electricitySchedules = new List<ElectricityProductionUnitSchedule>();

        // Act
        var schedule = new Schedule(heatSchedules, electricitySchedules);

        // Assert
        Assert.Empty(schedule.HeatProduction);
        Assert.Equal(0.0, schedule.TotalHeatProduction);
    }

    // Helper methods
    private HeatProductionUnitSchedule CreateHeatSchedule(string name, ResourceType resourceType, 
        decimal[] costs, double[] emissions, double[] heatProduction)
    {
        var schedule = new HeatProductionUnitSchedule(name, resourceType);
        
        for (int i = 0; i < costs.Length; i++)
        {
            var dataPoint = new HeatProductionUnitResultDataPoint(
                _startTime.AddHours(i), 
                _startTime.AddHours(i + 1),
                heatProduction[i] / 100, // Utilization value
                heatProduction[i],
                costs[i],
                10, // Default resource consumption
                emissions[i]
            );
            
            schedule.AddDataPoint(dataPoint);
        }
        
        return schedule;
    }
    
    private HeatProductionUnitSchedule CreateHeatScheduleWithResources(string name, ResourceType resourceType, 
        decimal[] costs, double[] emissions, double[] heatProduction, double[] resourceConsumption)
    {
        var schedule = new HeatProductionUnitSchedule(name, resourceType);
        
        for (int i = 0; i < costs.Length; i++)
        {
            var dataPoint = new HeatProductionUnitResultDataPoint(
                _startTime.AddHours(i), 
                _startTime.AddHours(i + 1),
                heatProduction[i] / 100, // Utilization value
                heatProduction[i],
                costs[i],
                resourceConsumption[i],
                emissions[i]
            );
            
            schedule.AddDataPoint(dataPoint);
        }
        
        return schedule;
    }
    
    private ElectricityProductionUnitSchedule CreateElectricitySchedule(string name, decimal[] prices, double[] production)
    {
        var schedule = new ElectricityProductionUnitSchedule(name);
        
        for (int i = 0; i < prices.Length; i++)
        {
            var dataPoint = new ElectricityProductionResultDataPoint(
                _startTime.AddHours(i), 
                _startTime.AddHours(i + 1),
                prices[i],
                production[i]
            );
            
            schedule.AddDataPoint(dataPoint);
        }
        
        return schedule;
    }
} 