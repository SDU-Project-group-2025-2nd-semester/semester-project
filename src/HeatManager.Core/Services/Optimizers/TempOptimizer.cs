namespace HeatManager.Core.Services.Optimizers;

public class TempOptimizer
{
    
    
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
                
                /*
                if (remainingDemand <= 0)
                    break; // Stop if demand is fully met
                */
                
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
}