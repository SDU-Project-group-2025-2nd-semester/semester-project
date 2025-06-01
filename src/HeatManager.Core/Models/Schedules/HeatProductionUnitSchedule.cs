using HeatManager.Core.Models.Resources;
using HeatManager.Core.ResultData;

namespace HeatManager.Core.Models.Schedules;

/// <summary>
/// Represents a schedule for a single heat production unit, tracking production, emissions, costs, and resource consumption.
/// </summary>
public class HeatProductionUnitSchedule
{
    /// <summary>
    /// Gets the name of the heat production unit.
    /// </summary>
    public string Name { get; }
    public ResourceType ResourceType { get; }
    public List<IHeatProductionUnitResultDataPoint> DataPoints { get; }

    /// <summary>
    /// Gets the array of heat production values for each time period.
    /// </summary>
    public double[] HeatProduction => DataPoints.Select(x => x.HeatProduction).ToArray();

    /// <summary>
    /// Gets the total heat production across all time periods.
    /// </summary>
    public double TotalHeatProduction => HeatProduction.Sum();

    /// <summary>
    /// Gets the maximum heat production value across all time periods.
    /// </summary>
    public double MaxHeatProduction => HeatProduction.Max();
    
    /// <summary>
    /// Gets the array of emissions values for each time period.
    /// </summary>
    public double[] Emissions => DataPoints.Select(x => x.Emissions).ToArray();

    /// <summary>
    /// Gets the total emissions across all time periods.
    /// </summary>
    public double TotalEmissions => Emissions.Sum();

    /// <summary>
    /// Gets the maximum emissions value across all time periods.
    /// </summary>
    public double MaxEmissions => Emissions.Max();
    
    /// <summary>
    /// Gets the array of cost values for each time period.
    /// </summary>
    public decimal[] Costs => DataPoints.Select(x => x.Cost).ToArray();

    /// <summary>
    /// Gets the total cost across all time periods.
    /// </summary>
    public decimal TotalCost => Costs.Sum(); 

    /// <summary>
    /// Gets the maximum cost value across all time periods.
    /// </summary>
    public decimal MaxCost => Costs.Max();
    
    /// <summary>
    /// Gets the array of resource consumption values for each time period.
    /// </summary>
    public double[] ResourceConsumption => DataPoints.Select(x => x.ResourceConsumption).ToArray();

    public KeyValuePair<ResourceType, double[]> ResourceConsumptionTyped => new(ResourceType, ResourceConsumption); 
    public KeyValuePair<ResourceType, double> TotalResourceConsumption => new (ResourceType, ResourceConsumption.Sum()) ;
    public KeyValuePair<ResourceType, double> MaxResourceConsumption => new (ResourceType, ResourceConsumption.Max()) ;

    
    /// <summary>
    /// Gets the array of utilization values for each time period.
    /// </summary>
    public double[] Utilization => DataPoints.Select(x => x.Utilization).ToArray();

    /// <summary>
    /// Gets the total utilization across all time periods.
    /// </summary>
    public double TotalUtilization => Utilization.Sum();

    /// <summary>
    /// Gets the maximum utilization value across all time periods.
    /// </summary>
    public double MaxUtilization => Utilization.Max();


    public HeatProductionUnitSchedule(string name, ResourceType resourceType)
    {
        Name = name;
        ResourceType = resourceType;
        DataPoints = new List<IHeatProductionUnitResultDataPoint>();
    }

    /// <summary>
    /// Adds a new data point to the schedule.
    /// </summary>
    /// <param name="dataPoint">The data point to add.</param>
    public void AddDataPoint(IHeatProductionUnitResultDataPoint dataPoint)
    {
        DataPoints.Add(dataPoint);
    }

    private List<KeyValuePair<ResourceType, double>> GetResourceConsumptionByHour(double[] resourceConsumptions)
    {
        List<KeyValuePair<ResourceType, double>> result = new List<KeyValuePair<ResourceType, double>>();

        foreach (var consumptionPoint in resourceConsumptions)
        {
            result.Add(new KeyValuePair<ResourceType, double>(ResourceType, consumptionPoint));
        }

        return result; 
    }
}