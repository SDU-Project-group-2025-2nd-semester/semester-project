using HeatManager.Core.ResultData;

namespace HeatManager.Core.Models.Schedules;

public class ElectricityProductionUnitSchedule
{
    public string Name { get; init;  }
    public double[] ElectricityProduction => DataPoints.Select(x => x.ElectricityProduction).ToArray();
    public double MaxElectricityProduction => ElectricityProduction.Max();
    public double TotalElectricityProduction => ElectricityProduction.Sum();
    
    
    public decimal[] ElectricityPrices => DataPoints.Select(x => x.ElectricityPrice).ToArray();
    public decimal MaxElectricityPrice => ElectricityPrices.Max(); 
    public decimal TotalElectricityPrice => ElectricityPrices.Sum();
    public List<ElectricityProductionResultDataPoint> DataPoints { get; set; }

    public ElectricityProductionUnitSchedule(string name)
    {
        Name = name; 
        DataPoints = new List<ElectricityProductionResultDataPoint>();
    }
    
    public void AddDataPoint(ElectricityProductionResultDataPoint dataPoint)
    {
        DataPoints.Add(dataPoint);
    }
    
}