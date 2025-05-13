using HeatManager.Core.Models.Producers;
using HeatManager.Core.Models.Schedules;
using HeatManager.Core.Models.SourceData;
using HeatManager.Core.ResultData;
using HeatManager.Core.Services.AssetManagers;
using HeatManager.Core.Services.SourceDataProviders;

namespace HeatManager.Core.Services.Optimizers;

///  <summary>
/// Default implementation of the IOptimizer interface.
/// Responsible for optimizing heat and electricity production schedules
/// based on available production units, source data, and optimization strategies.
/// </summary>
public class DefaultOptimizer : IOptimizer
{
    private readonly IAssetManager _assetManager;
    private readonly ISourceDataProvider _sourceDataProvider;
    private IOptimizerSettings _optimizerSettings;
    private readonly IOptimizerStrategy _optimizerStrategy;
    private object _resultManager;
    
    /// <summary>
    /// Initializes a new instance of the DefaultOptimizer class.
    /// </summary>
    /// <param name="assetManager">The asset manager providing production units.</param>
    /// <param name="sourceDataProvider">The source data provider for input data points.</param>
    /// <param name="optimizerSettings">The settings for optimization.</param>
    /// <param name="optimizerStrategy">The strategy used for optimization.</param>
    /// <param name="resultManager">The result manager for handling optimization results.</param>
    public DefaultOptimizer(IAssetManager assetManager,
        ISourceDataProvider sourceDataProvider, IOptimizerSettings optimizerSettings, IOptimizerStrategy optimizerStrategy, object resultManager)
    {
        _assetManager = assetManager;
        _sourceDataProvider = sourceDataProvider;
        _optimizerSettings = optimizerSettings;
        _optimizerStrategy = optimizerStrategy;
        _resultManager = resultManager;
    }
    /// <summary>
    /// Optimizes the heat and electricity production schedules based on the provided data and strategy.
    /// </summary>
    /// <returns>A <see cref="Schedule"/> object containing the optimized schedules for heat and electricity production units.</returns>
    public Schedule Optimize()
    {
        //Set up the data
        var scheduledEntries = _sourceDataProvider.SourceDataCollection?.DataPoints?? throw new Exception("Source data need to be imported before optimization!");
        var heatSources = GetAvailableUnits(_assetManager, _optimizerSettings);
        var electricitySources = heatSources
            .OfType<ElectricityProductionUnit>()
            .ToList();

        var heatProductionUnitSchedules = GenerateHeatProductionUnitSchedules(heatSources);
        var electricityProductionUnitSchedules = GenerateElectricityProductionUnitSchedules(electricitySources);
        
        // Iterate through each scheduled entry and optimize production
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
                    heatProductionUnitSchedules.Find(unit => unit.Name == heatSource.Name)
                        ?.AddDataPoint(new HeatProductionUnitResultDataPoint(
                        timeFrom: entry.TimeFrom,
                        timeTo: entry.TimeTo,
                        utilization: 0,
                        heatProduction: 0,
                        cost: 0,
                        resourceConsumption: 0,
                        emissions: 0
                    ));
                    continue;
                }
                
                // Calculate production and associated metrics
                double production = Math.Min(heatSource.MaxHeatProduction, remainingDemand);

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

                heatProductionUnitSchedules.Find(unit => unit.Name == heatSource.Name)?.AddDataPoint(dataPoint);
                
                // Handle electricity production for electricity-producing units
                if (heatSource is ElectricityProductionUnit electricityProductionUnit)
                {
                    var electricityProduction = utilization * electricityProductionUnit.MaxElectricity;
                    var electricityDataPoint = new ElectricityProductionResultDataPoint(
                        timeFrom: entry.TimeFrom,
                        timeTo: entry.TimeTo,
                        electricityProduction: electricityProduction,
                        electricityPrice: entry.ElectricityPrice
                    );
                    electricityProductionUnitSchedules.Find(unit => unit.Name == electricityProductionUnit.Name)
                        ?.AddDataPoint(electricityDataPoint);
                }

                remainingDemand -= production;
            }
        }
        // Return the final optimized schedule
        var resultSchedule = new Schedule(heatProductionUnitSchedules, electricityProductionUnitSchedules);
        return resultSchedule; 
    }
    
    /// <summary>
    /// Retrieves the list of available production units based on the optimizer settings.
    /// </summary>
    /// <param name="assetManager">The asset manager providing production units.</param>
    /// <param name="settings">The optimizer settings specifying active units.</param>
    /// <returns>A list of available production units.</returns>
    
    //TODO: Make this private
    public List<ProductionUnitBase> GetAvailableUnits(IAssetManager assetManager, IOptimizerSettings settings)
    {
        List<string> activeUnitsNames = settings.GetActiveUnitsNames(); 
        List<ProductionUnitBase> availableUnits = new List<ProductionUnitBase>();
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
    
    /// <summary>
    /// Updates the optimization settings for the optimizer.
    /// </summary>
    /// <param name="optimizerSettings">The new optimization settings. <see cref="OptimizerSettings"/>></param>
    public void ChangeOptimizationSettings(IOptimizerSettings optimizerSettings)
    {
        _optimizerSettings = optimizerSettings;
    }
    
    /// <summary>
    /// Generates a priority list of heat sources based on the optimization strategy.
    /// </summary>
    /// <param name="availableUnits">The available production units.</param>
    /// <param name="entry">The source data entry for the current time period.</param>
    /// <param name="strategy">The optimization strategy to apply.</param>
    /// <returns>An ordered list of production units based on priority.</returns>
    
    //TODO: Make private
    public static IEnumerable<ProductionUnitBase> GetHeatSourcePriorityList(IEnumerable<ProductionUnitBase> availableUnits,
        SourceDataPoint entry, IOptimizerStrategy strategy)
    {
        // Data setup from the source data entry 
        decimal electricityPrice = entry.ElectricityPrice;
        
        // Get all the units that are enabled at the moment 
        List<ProductionUnitBase> availableUnitsList = availableUnits.ToList();
        
        /*
         * Dangerous part of modifying the prices based on electricity prices. 
         */

        // Handle heat pumps (electricity consumers)
        // var heatPumps = availableUnitsList.FindAll(unit => unit.Resource.Type == ResourceType.Electricity && !(unit is IElectricityProductionUnit));
        var heatPumps = availableUnitsList.FindAll(unit => unit.Resource.Name == "Electricity" && !(unit is ElectricityProductionUnit));   

        var modifiedHeatPumps = new List<ProductionUnitBase>();

        foreach (var unit in heatPumps)
        {
            var unitClone = unit.Clone();
            unitClone.Cost += electricityPrice;
            modifiedHeatPumps.Add(unitClone);
        }

        // Remove original heat pumps and add modified ones
        foreach (var unit in heatPumps)
        {
            availableUnitsList.Remove(unit);
        }
        availableUnitsList.AddRange(modifiedHeatPumps);

        // Handle electricity producers
        var electricityProductionUnits = availableUnitsList.OfType<ElectricityProductionUnit>().ToList();
        var modifiedProducers = new List<ProductionUnitBase>();

        foreach (var unit in electricityProductionUnits)
        {
            var unitClone = unit.Clone();
            // Calculate the electricity production ratio (electricity per unit of heat)
            double electricityRatio = unit.MaxElectricity / unit.MaxHeatProduction;
            // Adjust the cost by subtracting the value of produced electricity
            unitClone.Cost -= electricityPrice * (decimal)electricityRatio;
            modifiedProducers.Add(unitClone);
        }

        // Remove original producers and add modified ones
        foreach (var unit in electricityProductionUnits)
        {
            availableUnitsList.Remove(unit);
        }
        availableUnitsList.AddRange(modifiedProducers);
        
        // Sort the units based on the strategy
        IEnumerable<ProductionUnitBase> heatSourcePriorityList;
        
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

    /// <summary>
    /// Generates schedules for heat production units, that will be populated by the optimizer.
    /// </summary>
    /// <param name="heatProductionUnits">The heat production units to generate schedules for.</param>
    /// <returns>A list of heat production unit schedules.</returns>
    
    //TODO: Make private
    public List<HeatProductionUnitSchedule> GenerateHeatProductionUnitSchedules(
        IEnumerable<ProductionUnitBase> heatProductionUnits)
    {
        List<HeatProductionUnitSchedule> schedules = new List<HeatProductionUnitSchedule>();
        foreach (var unit in heatProductionUnits)
        {
            var schedule = new HeatProductionUnitSchedule(unit.Name);
            schedules.Add(schedule);
        }

        return schedules;
    }

    /// <summary>
    /// Generates schedules for electricity production units.
    /// </summary>
    /// <param name="electricityProductionUnits">The electricity production units to generate schedules for.</param>
    /// <returns>A list of electricity production unit schedules.</returns>
    
    //TODO: Make private
    public List<ElectricityProductionUnitSchedule> GenerateElectricityProductionUnitSchedules(
        IEnumerable<ElectricityProductionUnit> electricityProductionUnits)
    {
        List<ElectricityProductionUnitSchedule> schedules = new List<ElectricityProductionUnitSchedule>();
        foreach (var unit in electricityProductionUnits)
        {
            var schedule = new ElectricityProductionUnitSchedule(unit.Name);
            schedules.Add(schedule);
        }
        return schedules;
    }
}