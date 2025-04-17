namespace HeatManager.Core.Models.Schedules;

public interface IElectricityProductionUnitSchedule
{
    public string Name { get; init;  }

    public double[] ElectricityProduction { get; }
    public double MaxElectricityProduction { get; }
    public double TotalElectricityProduction { get; }
    
    public decimal[] ElectricityPrices { get; }
    public decimal MaxElectricityPrice { get; }
    public decimal TotalElectricityPrice { get; }

    public List<IElectricityProductionResultDataPoint> DataPoints { get; set;  }
    public void AddDataPoint(IElectricityProductionResultDataPoint dataPoint);
}