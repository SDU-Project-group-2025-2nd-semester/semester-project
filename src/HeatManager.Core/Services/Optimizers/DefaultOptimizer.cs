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
    internal IAssetManager _assetManager { get; private set; }
    private readonly ISourceDataProvider _sourceDataProvider;
    internal IOptimizerSettings _optimizerSettings { get; private set; }
    private readonly IOptimizerStrategy _optimizerStrategy;

    
    /// <summary>
    /// Initializes a new instance of the DefaultOptimizer class.
    /// </summary>
    /// <param name="assetManager">The asset manager providing production units.</param>
    /// <param name="sourceDataProvider">The source data provider for input data points.</param>
    /// <param name="optimizerSettings">The settings for optimization.</param>
    /// <param name="optimizerStrategy">The strategy used for optimization.</param>
    /// <param name="resultManager">The result manager for handling optimization results.</param>
    public DefaultOptimizer(IAssetManager assetManager,
        ISourceDataProvider sourceDataProvider, IOptimizerSettings optimizerSettings, IOptimizerStrategy optimizerStrategy)
    {
        _assetManager = assetManager;
        _sourceDataProvider = sourceDataProvider;
        _optimizerSettings = optimizerSettings;
        _optimizerStrategy = optimizerStrategy;
    }
    /// <summary>
    /// Optimizes the heat and electricity production schedules based on the provided data and strategy.
    /// </summary>
    /// <returns>A <see cref="Schedule"/> object containing the optimized schedules for heat and electricity production units.</returns>
    public Schedule Optimize()
    {
        //Set up the data
        if (_sourceDataProvider.SourceDataCollection == null)
        {
            throw new InvalidOperationException("SourceDataCollection is null. Ensure that it is properly initialized before calling Optimize.");
        }
        var scheduledEntries = _sourceDataProvider.SourceDataCollection.DataPoints;
        var heatSources = GetAvailableUnits();

        var electricitySources = heatSources
            .OfType<ElectricityProductionUnit>()
            .ToList();

        var heatProductionUnitSchedules = GenerateHeatProductionUnitSchedules(heatSources);
        var electricityProductionUnitSchedules = GenerateElectricityProductionUnitSchedules(electricitySources);
        
        // Iterate through each scheduled entry and optimize production
        for (int i = 0; i < scheduledEntries.Count(); i++)
        {
            var entry = scheduledEntries.ElementAt(i);
            var priorityList = GetHeatSourcePriorityList(heatSources, entry);

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

    private List<ProductionUnitBase> GetAvailableUnits()

    {
        List<string> activeUnitsNames = _optimizerSettings.GetActiveUnitsNames(); 
        List<ProductionUnitBase> availableUnits = [];
        for (int i = 0; i < _assetManager.ProductionUnits.Count; i++)
        {
            var unit = _assetManager.ProductionUnits.ElementAt(i);
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
    /// Updates the production units in the optimizer.
    /// </summary>
    /// <param name="assetManager"></param>
    public void UpdateProductionUnits(IAssetManager assetManager)
    {
        _assetManager = assetManager
            ?? throw new ArgumentNullException(nameof(assetManager));

        if (assetManager.ProductionUnits == null)
            throw new ArgumentNullException(nameof(assetManager.ProductionUnits));

        _optimizerSettings.AllUnits = assetManager.ProductionUnits
            .Select(u => u.Name)
            .ToDictionary(name => name, name => true);
    }

    private IEnumerable<ProductionUnitBase> GetHeatSourcePriorityList(IEnumerable<ProductionUnitBase> availableUnits,
        SourceDataPoint entry)

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
        
        if (_optimizerStrategy.Optimization == OptimizationType.PriceOptimization)
        {
            heatSourcePriorityList = availableUnitsList.OrderBy(unit => unit.Cost).ThenBy(unit => unit.Emissions);
        }
        else if (_optimizerStrategy.Optimization == OptimizationType.Co2Optimization)
        {
            heatSourcePriorityList = availableUnitsList.OrderBy(unit => unit.Emissions).ThenBy(unit => unit.Cost);
        }
        else if (_optimizerStrategy.Optimization == OptimizationType.BalancedOptimization)
        {
            /*
                For the BalancedOptimization strategy, normalize emissions and costs to a common scale
                by dividing each unit's value by the maximum value in the list. This ensures that both
                factors are weighted equally regardless of their original scales. Then, calculate a 
                composite score as the average of the normalized emissions and costs. Units are prioritized
                based on this score, with lower scores indicating higher priority.
            */ 
            var maxEmissions = availableUnitsList.Max(unit => unit.Emissions);
            var maxCost = availableUnitsList.Max(unit => unit.Cost);
            
            //explicit 0 check
            if (maxEmissions == 0 || maxCost == 0)
            {
                throw new InvalidOperationException("Max emissions or max cost is zero, cannot perform normalization");
            }

            heatSourcePriorityList = availableUnitsList
                .OrderBy(unit =>
                {
                    double normalizedEmissions = unit.Emissions / maxEmissions; // Normalize emissions based on the max value in the list
                    decimal normalizedCost = unit.Cost / maxCost; // Normalize cost similarly
                    
                    var score = (normalizedCost + (decimal)normalizedEmissions) / 2;
                    return score;
                }); 
        }
        else
        {
            throw new Exception("Optimization strategy not selected, caught in DefaultOptimizer.GetHeatSourcePriorityList"); 
        }

        return heatSourcePriorityList; 
    }

    private List<HeatProductionUnitSchedule> GenerateHeatProductionUnitSchedules(

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

    private List<ElectricityProductionUnitSchedule> GenerateElectricityProductionUnitSchedules(
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