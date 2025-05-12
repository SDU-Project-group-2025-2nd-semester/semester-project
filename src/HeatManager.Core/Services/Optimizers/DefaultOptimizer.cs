using HeatManager.Core.Models.Producers;
using HeatManager.Core.Models.Schedules;
using HeatManager.Core.Models.SourceData;
using HeatManager.Core.ResultData;
using HeatManager.Core.Services.AssetManagers;
using HeatManager.Core.Services.SourceDataProviders;

namespace HeatManager.Core.Services.Optimizers;

public class DefaultOptimizer : IOptimizer
{
    private readonly IAssetManager _assetManager;
    private readonly ISourceDataProvider _sourceDataProvider;
    private IOptimizerSettings _optimizerSettings;
    private readonly IOptimizerStrategy _optimizerStrategy;

    public DefaultOptimizer(IAssetManager assetManager,
        ISourceDataProvider sourceDataProvider, IOptimizerSettings optimizerSettings, IOptimizerStrategy optimizerStrategy)
    {
        _assetManager = assetManager;
        _sourceDataProvider = sourceDataProvider;
        _optimizerSettings = optimizerSettings;
        _optimizerStrategy = optimizerStrategy;
    }

    public Schedule Optimize()
    {
        //Set up the data
        var scheduledEntries = _sourceDataProvider.SourceDataCollection.DataPoints;
        var heatSources = GetAvailableUnits(_assetManager, _optimizerSettings);
        var electricitySources = heatSources
            .OfType<ElectricityProductionUnit>()
            .ToList();

        var heatProductionUnitSchedules = GenerateHeatProductionUnitSchedules(heatSources);
        var electricityProductionUnitSchedules = GenerateElectricityProductionUnitSchedules(electricitySources);

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

        var resultSchedule = new Schedule(heatProductionUnitSchedules, electricityProductionUnitSchedules);
        return resultSchedule; 
    }

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
    
    public void ChangeOptimizationSettings(IOptimizerSettings optimizerSettings)
    {
        _optimizerSettings = optimizerSettings;
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
            //let's see what will happen
            var maxEmissions = availableUnitsList.Max(unit => unit.Emissions);
            var maxCost = availableUnitsList.Max(unit => unit.Cost);

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