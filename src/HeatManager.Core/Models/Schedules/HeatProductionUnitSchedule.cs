using HeatManager.Core.Models.Producers;
using HeatManager.Core.Models.Resources;
using HeatManager.Core.ResultData;

namespace HeatManager.Core.Models.Schedules;

public class HeatProductionUnitSchedule
{
    public string Name { get; }
    public ResourceType ResourceType { get; }
    public List<IHeatProductionUnitResultDataPoint> DataPoints { get; }
    //Heat Production
    public double[] HeatProduction => DataPoints.Select(x => x.HeatProduction).ToArray();
    public double TotalHeatProduction => HeatProduction.Sum();
    public double MaxHeatProduction => HeatProduction.Max();
    
    //Emissions
    public double[] Emissions => DataPoints.Select(x => x.Emissions).ToArray();
    public double TotalEmissions => Emissions.Sum();
    public double MaxEmissions => Emissions.Max();
    
    //Costs
    public decimal[] Costs => DataPoints.Select(x => x.Cost).ToArray();
    public decimal TotalCost => Costs.Sum(); 
    public decimal MaxCost => Costs.Max();
    
    //Resources
    public double[] ResourceConsumption => DataPoints.Select(x => x.ResourceConsumption).ToArray();
    public KeyValuePair<ResourceType, double[]> ResourceConsumptionTyped => new(ResourceType, ResourceConsumption); 
    public KeyValuePair<ResourceType, double> TotalResourceConsumption => new (ResourceType, ResourceConsumption.Sum()) ;
    public KeyValuePair<ResourceType, double> MaxResourceConsumption => new (ResourceType, ResourceConsumption.Max()) ;
    
    //Utilization 
    public double[] Utilization => DataPoints.Select(x => x.Utilization).ToArray();
    public double TotalUtilization => Utilization.Sum();
    public double MaxUtilization => Utilization.Max();

    


    public HeatProductionUnitSchedule(string name, ResourceType resourceType)
    {
        Name = name;
        ResourceType = resourceType;
        DataPoints = new List<IHeatProductionUnitResultDataPoint>();
    }

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