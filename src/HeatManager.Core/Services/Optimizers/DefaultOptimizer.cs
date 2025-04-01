using HeatManager.Core.Models.Producers;
using HeatManager.Core.Models.Schedules;
using HeatManager.Core.Models.SourceData;
using HeatManager.Core.Services;
using System.Collections.Immutable;

namespace HeatManager.Core.Services.Optimizers;

// 1. Let user choose from where to load data
// 2. Let user set-up heat sources
// 3. we have CSV entries and Heat sources with their resources and electricity production
// 4. call optimizer to create optimal schedule
// 5. Display data to the user
// 6. Export data

// *1 User may change the heat sources and their resources any time, then the schedule should be re-optimized
// *2 User may change the which units should run before optimization 

internal class DefaultOptimizer : IOptimizer
{
    private readonly IHeatSourceManager _heatSourceManager;
    private readonly IResourceManager _resourceManager;
    private readonly ISourceDataProvider _sourceDataProvider;
    
    private IOptimizerSettings _optimizerSettings; //TODO: Implement settings

    private object _resultManager; //TODO: Implement result manager 

    public DefaultOptimizer(IHeatSourceManager heatSourceManager, IResourceManager resourceManager, ISourceDataProvider sourceDataProvider, IOptimizerSettings optimizerSettings)
    {
        _heatSourceManager = heatSourceManager; // TODO: Get all the necessary data from services
        _resourceManager = resourceManager;
        _sourceDataProvider = sourceDataProvider;
        _optimizerSettings = optimizerSettings;
    }

    
    public async Task OptimizeAsync()
    {
        var scheduledEntries = _sourceDataProvider.SourceDataCollection;
        var heatSources = GetAvailableUnits(_heatSourceManager, _optimizerSettings); // TODO: Implement this method
        
        // var resources = _resourceManager.Resources; // TODO: Probably not needed at all in the code
        
        
        await Task.Run(() => // To offload it to a background thread, TODO: Probably put to separate method
        {
            new Schedule();
        });
    }
    /*
     * This method gets the available heat sources and the scheduled entries and then optimizes the schedule
     * TODO: Return type ResultDataPoint ???????
     */
    private void Optimize(IEnumerable<IHeatProductionUnit> availableUnits, IEnumerable<ISourceDataPoint> scheduledEntries, IOptimizerStrategy strategy)
    {
        IEnumerable<IHeatProductionUnit> heatProductionUnits = availableUnits.ToList();
        IEnumerable<ISourceDataPoint> sourceDataPoints = scheduledEntries.ToList();
        
        IEnumerable<IHeatProductionUnit> heatSourcePriorityList;
        
        for (int i = 0; i < sourceDataPoints.Count(); i++)
        {
            var entry = sourceDataPoints.ElementAt(i);
            double remainingDemand = entry.HeatDemand;
            heatSourcePriorityList = GetHeatSourcePriorityList(heatProductionUnits, entry, strategy);
            
            foreach (var heatSource in heatSourcePriorityList)
            {
                if (remainingDemand <= 0)
                    break; // Stop if demand is fully met
                
                double production = Math.Min(heatSource.MaxHeatProduction, remainingDemand);
                
                //TODO: this batch of code should probably be some sort of a getter in a heatProductionUnit
                double utilization = production / heatSource.MaxHeatProduction;
                decimal cost = (decimal)production * heatSource.Cost;
                double consumption = production * heatSource.ResourceConsumption;
                double emissions = production * heatSource.Emissions;
                
                
                var dataPoint = new HeatProductionUnitResultDataPoint(
                timeFrom: entry.TimeFrom,
                timeTo: entry.TimeTo,
                utilization: utilization,
                heatProduction: production,
                cost: cost,
                resourceConsumption: consumption,
                emissions: emissions
                );
                
                //TODO: 

                remainingDemand -= production;
            }
            
        }
    }

    public void ChangeOptimizationSettings(IOptimizerSettings optimizerSettings)
    {
        _optimizerSettings = optimizerSettings; 
    }
    
    
    /*
     * 
     */
    private IEnumerable<IHeatProductionUnit> GetHeatSourcePriorityList(IEnumerable<IHeatProductionUnit> availableUnits, ISourceDataPoint entry, IOptimizerStrategy strategy)
    {
        decimal electricityPrice = entry.ElectricityPrice;
        List<IHeatProductionUnit> availableUnitsList = availableUnits.ToList();
        
        /*
         * Here decide, if a heatpump should not rather be its own object
         */
        List<IHeatProductionUnit>? heatPumps = availableUnitsList.FindAll(unit => unit.Name.Contains("HP"));  // I don't like this at all 
        if (heatPumps.Count > 0)
        {
            foreach (var heatPump in heatPumps)
            {
                heatPump.Cost = electricityPrice; 
            }
        }
        
        /*
         * I don't even know if this is how we the cost of the gas motor should be determined
         */

        
        List<IElectricityProductionUnit> gasMotors = availableUnitsList.FindAll(unit => unit is IElectricityProductionUnit electricityProductionUnit); //will not work, because of different types
        
        if (gasMotors.Count > 0)
        {
            foreach (var gasMotor in gasMotors)
            {
                gasMotor.Cost = CalculateGasMotorCost(gasMotor, electricityPrice);
            }
        }
        
        
        IEnumerable<IHeatProductionUnit> heatSourcePriorityList;
        
        if (strategy.Optimization == OptimizationType.PriceOptimization)
        {
            heatSourcePriorityList = availableUnitsList.OrderBy(unit => unit.Cost).ThenBy(unit => unit.Emissions);
        }
        else if (strategy.Optimization == OptimizationType.Co2Optimization)
        {
            heatSourcePriorityList = availableUnitsList.OrderBy(unit => unit.Emissions).ThenBy(unit => unit.Cost);
        }
        else
        {
            throw new Exception("Optimization strategy not selected, caught in DefaultOptimizer.GetHeatSourcePriorityList"); 
        }

        return heatSourcePriorityList; 
    }

    private IEnumerable<IHeatProductionUnit> GetAvailableUnits(IHeatSourceManager heatSourceManager, IOptimizerSettings optimizerSettings)
    {
        List<IHeatProductionUnit> activeUnits = new List<IHeatProductionUnit>();
        List<string> availableUnitsNames = optimizerSettings.GetActiveUnits();

        foreach (var heatSourceUnit in heatSourceManager.HeatSources)
        {
            if (availableUnitsNames.Contains(heatSourceUnit.Name))
            {
                activeUnits.Add(heatSourceUnit);
            }
        }

        return activeUnits; 
    }

    private decimal CalculateGasMotorCost(IElectricityProductionUnit gasMotor, decimal electricityPrice)
    {
        return gasMotor.Cost - ((decimal) gasMotor.MaxElectricity * electricityPrice);
    }

    
    //TODO: Check, if this is the right approach and move it to the RDM
    private IEnumerable<IHeatProductionUnitSchedule> CreateHeatProductionUnitSchedules(IEnumerable<IHeatProductionUnit> heatProductionUnits)
    {
        List<IHeatProductionUnitSchedule> heatProductionUnitSchedules = new List<IHeatProductionUnitSchedule>();
        
        foreach (var heatProductionUnit in heatProductionUnits)
        {
            heatProductionUnitSchedules.Add(new HeatProductionUnitSchedule(heatProductionUnit.Name));
        }

        return heatProductionUnitSchedules; 
    }
}