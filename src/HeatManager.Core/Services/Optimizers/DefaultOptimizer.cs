using HeatManager.Core.Models.Producers;
using HeatManager.Core.Models.Schedules;
using HeatManager.Core.Models.SourceData;
using HeatManager.Core.Services;
using System.Collections.Immutable;

namespace HeatManager.Core.Services.Optimizers;

internal class DefaultOptimizer : IOptimizer
{

    private readonly IAssetManager _assetManager;
    private readonly IResourceManager _resourceManager;
    private readonly ISourceDataProvider _sourceDataProvider;

    private IOptimizerSettings _optimizerSettings; //TODO: Implement 
    private IOptimizerStrategy _optimizerStrategy; //TODO: Implement 

    private object _resultManager; //TODO: Implement result manager 

    public DefaultOptimizer(IAssetManager assetManager, IResourceManager resourceManager,
        ISourceDataProvider sourceDataProvider, IOptimizerSettings optimizerSettings)
    {
        _assetManager = assetManager; // TODO: Get all the necessary data from services
        _resourceManager = resourceManager;
        _sourceDataProvider = sourceDataProvider;
        _optimizerSettings = optimizerSettings;
    }

    public async Task OptimizeAsync()
    {
        await Task.Run(Optimize); 
    }

    private void Optimize()
    {
        //Set up the data
        var scheduledEntries = _sourceDataProvider.SourceDataCollection.DataPoints;
        var heatSources = GetAvailableUnits(_assetManager, _optimizerSettings);


        var heatProductionUnitSchedules = GenerateHeatProductionUnitSchedules(heatSources); 
        var electricityProductionUnitSchedules = new List<IElectricityProductionUnitSchedule>();
        
        for (int i = 0; i < scheduledEntries.Count(); i++)
        {
            var entry = scheduledEntries.ElementAt(i); 
            var priorityList = 
                GetHeatSourcePriorityList(heatSources, entry, _optimizerStrategy);
            
            double remainingDemand = entry.HeatDemand;
            foreach (var heatSource in priorityList)
            {
                if (remainingDemand <= 0)
                {
                    //TODO: replace all other entries with 0 utilization
                }
                
                double production = Math.Min(heatSource.MaxHeatProduction, remainingDemand);
                remainingDemand -= production;
                
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
                //TODO: null reference? 
                heatProductionUnitSchedules.Find(unit => unit.Name == heatSource.Name)?.AddDataPoint(dataPoint);
                
            }
        }
    }

    private List<IHeatProductionUnit> GetAvailableUnits(IAssetManager assetManager, IOptimizerSettings settings)
    {
        List<string> activeUnitsNames = settings.GetActiveUnits(); 
        List<IHeatProductionUnit> availableUnits = new List<IHeatProductionUnit>();
        for (int i = 0; i < assetManager.ProductionUnits.Count(); i++)
        {
            var unit = assetManager.ProductionUnits.ElementAt(i);
            if (activeUnitsNames.Contains(unit.Name))
            {
                availableUnits.Add(unit);
            }
        }
        return availableUnits;
    }
    
    public void ChangeOptimizationSettings(IOptimizerSettings optimizerSettings)
    {
        _optimizerSettings = optimizerSettings;
    }

    private IEnumerable<IHeatProductionUnit> GetHeatSourcePriorityList(IEnumerable<IHeatProductionUnit> availableUnits,
        ISourceDataPoint entry, IOptimizerStrategy strategy)
    {
        // Data setup from the source data entry 
        double heatDemand = entry.HeatDemand;
        decimal electricityPrice = entry.ElectricityPrice;
        
        // Get all the units that are enabled at the moment 
        List<IHeatProductionUnit> availableUnitsList = availableUnits.ToList();
        
        // Determine the prices for the electricity-based devices TODO: implement it as an enum in the BasicResource class
        var heatPumps = availableUnitsList.FindAll(unit => unit.Resource.Name == "Electricity");    
        for (int i = 0; i < heatPumps.Count(); i++)
        {
            var unit = heatPumps.ElementAt(i);
            unit.Cost += electricityPrice; //TODO: this will overwrite the cost of the unit and then we will not be able to use it again, needs a fix
        }
        
        // Determine the prices for the units that also generate electricity
        var electricityProductionUnits =
            availableUnitsList.OfType<IElectricityProductionUnit>().ToList();
        for (int i = 0; i < electricityProductionUnits.Count(); i++)
        {
            var unit = electricityProductionUnits.ElementAt(i);
            unit.Cost -= electricityPrice; //TODO: same as above 
        }
        
        // Sort the units based on the strategy
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


    private List<IHeatProductionUnitSchedule> GenerateHeatProductionUnitSchedules(
        IEnumerable<IHeatProductionUnit> heatProductionUnits)
    {
        List<IHeatProductionUnitSchedule> schedules = new List<IHeatProductionUnitSchedule>();
        foreach (var unit in heatProductionUnits)
        {
            var schedule = new HeatProductionUnitSchedule(unit.Name);
            schedules.Add(schedule);
        }

        return schedules;
    }

}
    
