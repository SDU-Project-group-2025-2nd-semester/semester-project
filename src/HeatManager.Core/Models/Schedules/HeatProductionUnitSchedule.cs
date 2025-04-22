namespace HeatManager.Core.Models.Schedules;

public class HeatProductionUnitSchedule
{
    public string Name { get; }
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
    public double TotalResourceConsumption => ResourceConsumption.Sum();
    public double MaxResourceConsumption => ResourceConsumption.Max();
    
    //Utilization 
    public double[] Utilization => DataPoints.Select(x => x.Utilization).ToArray();
    public double TotalUtilization => Utilization.Sum();
    public double MaxUtilization => Utilization.Max();

    


    public HeatProductionUnitSchedule(string name)
    {
        Name = name;
        DataPoints = new List<IHeatProductionUnitResultDataPoint>();
    }

    public void AddDataPoint(IHeatProductionUnitResultDataPoint dataPoint)
    {
        DataPoints.Add(dataPoint);
    }
    
}