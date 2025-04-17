namespace HeatManager.Core.Models.Schedules;

internal class ElectricityProductionUnitSchedule : IElectricityProductionUnitSchedule
{
    public string Name { get; init;  }
    public double[] ElectricityProduction => DataPoints.Select(x => x.ElectricityProduction).ToArray();
    public double MaxElectricityProduction => ElectricityProduction.Max();
    public double TotalElectricityProduction => ElectricityProduction.Sum();
    
    
    public decimal[] ElectricityPrices => DataPoints.Select(x => x.ElectricityPrice).ToArray();
    public decimal MaxElectricityPrice => ElectricityPrices.Max(); 
    public decimal TotalElectricityPrice => ElectricityPrices.Sum();
    public List<IElectricityProductionResultDataPoint> DataPoints { get; set; }

    public ElectricityProductionUnitSchedule(string name)
    {
        Name = name; 
        DataPoints = new List<IElectricityProductionResultDataPoint>();
    }
    
    public void AddDataPoint(IElectricityProductionResultDataPoint dataPoint)
    {
        DataPoints.Add(dataPoint);
    }
    
}