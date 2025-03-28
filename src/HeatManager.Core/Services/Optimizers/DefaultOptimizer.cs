using HeatManager.Core.Models.Producers;
using HeatManager.Core.Models.Schedules;
using HeatManager.Core.Models.SourceData;
using HeatManager.Core.Services;
using System.Runtime.InteropServices.JavaScript;

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

    public DefaultOptimizer(IHeatSourceManager heatSourceManager, IResourceManager resourceManager, ISourceDataProvider sourceDataProvider)
    {
        _heatSourceManager = heatSourceManager; // TODO: Get all the necessary data from services
        _resourceManager = resourceManager;
        _sourceDataProvider = sourceDataProvider;
    }

    public async Task OptimizeAsync()
    {
        var scheduledEntries = _sourceDataProvider.SourceDataCollection;
        var heatSources = GetAvailableUnits(_heatSourceManager, _optimizerSettings); // TODO: Implement this method
        
        // var resources = _resourceManager.Resources; // TODO: Probably not needed at all in the code
        
        
        await Task.Run(() => // To offload it to a background thread, TODO: Probably put to separate method
        {
            var heatSourcePriorityList = GetHeatSourcePriorityList(heatSources, scheduledEntries.DataPoints[0]); // TODO: Implement this method
            // Probably load some settings here, or get them as parameters

            // Make heat source priority list

            // Iterate over each entry in the schedule

            // Create heat schedule 

            new Schedule();
        });
    }

    public void ChangeOptimizationSettings(IOptimizerSettings optimizerSettings)
    {
        _optimizerSettings = optimizerSettings; 
    }
    
    private IEnumerable<IHeatProductionUnit> GetHeatSourcePriorityList(IEnumerable<IHeatProductionUnit> availableUnits, ISourceDataPoint entry, IOptimizerStrategy strategy)
    {
        decimal electricityPrice = entry.ElectricityPrice;
        if (strategy.Optimization == OptimizationType.PriceOptimization)
        {
            
        }
        else if (strategy.Optimization == OptimizationType.Co2Optimization)
        {
            
        }
        else
        {
            throw new Exception("Optimization strategy not selected, caught in DefaultOptimizer.GetHeatSourcePriorityList"); 
        }
        
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
}