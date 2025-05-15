using HeatManager.Core.Models.Resources;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace HeatManager.Core.Models.Schedules;

/// <summary>
/// Represents a comprehensive schedule that aggregates heat and electricity production schedules.
/// This class manages time-based data, costs, emissions, and production metrics.
/// </summary>
public class Schedule
{
    /// <summary>
    /// Gets the total number of time periods in the schedule.
    /// </summary>
    public int Length { get; private set; }
    
    
    //Main Data
    public ImmutableList<HeatProductionUnitSchedule> HeatProductionUnitSchedules { get; }
    public ImmutableList<ElectricityProductionUnitSchedule> ElectricityProductionUnitSchedules { get; }

    
    //Time Data
    /// <summary>
    /// Gets the start time of the schedule.
    /// </summary>
    public DateTime Start { get; private set; }

    /// <summary>
    /// Gets the end time of the schedule.
    /// </summary>
    public DateTime End { get; private set; }

    /// <summary>
    /// Gets the time resolution of the schedule.
    /// </summary>
    public TimeSpan Resolution { get; private set; }


    //Cost related data
    public decimal[] Costs => GetCostsByHour(HeatProductionUnitSchedules); 
    public decimal TotalCost => Costs.Sum();
    
    /// <summary>
    /// Gets the total emissions across all time periods.
    /// </summary>
    public double[] Emissions => GetEmissionsByHour(HeatProductionUnitSchedules); 
    public double TotalEmissions => Emissions.Sum(); 
    

    /// <summary>
    /// Gets the total heat production across all time periods.
    /// </summary>
    public double[] HeatProduction => GetHeatProductionByHour(HeatProductionUnitSchedules);
    public double TotalHeatProduction => HeatProduction.Sum();
    
    public decimal[] ElectricityPrice
    {
        get
        {
            if (ElectricityProductionUnitSchedules.Any())
            {
                return GetElectricityCostsByHour(ElectricityProductionUnitSchedules.ElementAt(0));
            } else return new decimal[Length];
        }
    }

    public double[] ElectricityProduction
    {
        get
        {
            if (ElectricityProductionUnitSchedules.Any())
            {
                return GetElectricityProductionByHour(ElectricityProductionUnitSchedules);
            } else return new double[Length];
        }
    } 
    
    //Resource consumption related data
    public Dictionary<ResourceType, double[]> ResourceConsumption => GetResourceConsumptionByHour(HeatProductionUnitSchedules);

    /// <summary>
    /// Initializes a new instance of the Schedule class.
    /// </summary>
    /// <param name="heatProductionUnitSchedules">Collection of heat production unit schedules.</param>
    /// <param name="electricityProductionUnitSchedules">Collection of electricity production unit schedules.</param>
    public Schedule(IEnumerable<HeatProductionUnitSchedule> heatProductionUnitSchedules,
        IEnumerable<ElectricityProductionUnitSchedule> electricityProductionUnitSchedules)
    {
        HeatProductionUnitSchedules = heatProductionUnitSchedules.ToImmutableList();
        ElectricityProductionUnitSchedules = electricityProductionUnitSchedules.ToImmutableList();
        CreateProperties();
    }

    private void CreateProperties()
    {
        if (HeatProductionUnitSchedules.IsEmpty) 
        { 
            Length = 0; 
            Start = new DateTime(0);
            End = new DateTime(0); 
            Resolution = Start - End;  
        } 
        else 
        { 
            Length = HeatProductionUnitSchedules.ElementAt(0).DataPoints.Count();
            Start = HeatProductionUnitSchedules.ElementAt(0).DataPoints.ElementAt(0).TimeFrom; 
            End = HeatProductionUnitSchedules.ElementAt(0).DataPoints
                .ElementAt(Length - 1).TimeTo; //TODO: make this actually readable
            Resolution = End - Start; 
        }

    } 

    private double[] GetEmissionsByHour(IEnumerable<HeatProductionUnitSchedule> heatProductionUnitSchedules)
    {

        var emissions = new double[Length];
        
        for (int i = 0; i < Length; i++)
        {
            foreach (var schedule in heatProductionUnitSchedules)
            {
                emissions[i] += schedule.Emissions.ElementAt(i);
            }
        }
        return emissions; 
    }
    
    private decimal[] GetCostsByHour(ImmutableList<HeatProductionUnitSchedule> heatProductionUnitSchedules)
    {
        var costs = new decimal[Length];
        
        for (int i = 0; i < Length; i++)
        {
            foreach (var schedule in heatProductionUnitSchedules)
            {
                costs[i] += schedule.Costs.ElementAt(i);
            }
        }
        return costs; 
    }

    private double[] GetHeatProductionByHour(ImmutableList<HeatProductionUnitSchedule> heatProductionUnitSchedules)
    {
        var heatProduction = new double[Length];
        
        for (int i = 0; i < Length; i++)
        {
            foreach (var schedule in heatProductionUnitSchedules)
            {
                heatProduction[i] += schedule.HeatProduction[i];
            }
        }
        return heatProduction; 
    }

    private decimal[] GetElectricityCostsByHour(ElectricityProductionUnitSchedule electricityProductionUnitSchedule)
    {
        var electricityCosts = new decimal[Length];
        var dataPoints = electricityProductionUnitSchedule.DataPoints.ToList();
        for (int i = 0; i < Length; i++)
        {
            electricityCosts[i] = dataPoints.ElementAt(i).ElectricityPrice;
        }
        
        return electricityCosts;
    }

    private double[] GetElectricityProductionByHour(
        IEnumerable<ElectricityProductionUnitSchedule> electricityProductionUnitSchedules)
    {
        var electricityProduction = new double[Length];
        var schedules = electricityProductionUnitSchedules.ToList();

        for (int i = 0; i < Length; i++)
        {
            foreach (var schedule in schedules)
            {
                electricityProduction[i] += schedule.ElectricityProduction.ElementAt(i);
            }
        }
        return electricityProduction;
    }

    private Dictionary<ResourceType, double[]> GetResourceConsumptionByHour(
        IEnumerable<HeatProductionUnitSchedule> heatProductionUnitSchedules)
    {
        var schedules = heatProductionUnitSchedules.ToList();
        var resourceConsumption = new Dictionary<ResourceType, double[]>();

        for (int i = 0; i < Length; i++)
        {
            foreach (var schedule in schedules)
            {
                var key = schedule.ResourceConsumptionTyped.Key;
                var value = schedule.ResourceConsumptionTyped.Value[i];
                
                if (!resourceConsumption.ContainsKey(key))
                {
                    resourceConsumption[key] = new double[Length];
                }
                
                resourceConsumption[key][i] += value;
            }
        }

        return resourceConsumption;
    }
}