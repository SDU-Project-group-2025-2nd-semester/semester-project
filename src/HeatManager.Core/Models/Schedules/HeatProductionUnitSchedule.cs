namespace HeatManager.Core.Models.Schedules;

internal class HeatProductionUnitSchedule : IHeatProductionUnitSchedule
{
    public string Name { get; }
    public decimal TotalCost { get; private set; }
    public double MaxHeatProduction { get; private set;}
    public double ResourceConsumption { get; private set;  }
    public double TotalEmissions { get; private set;  }
    
    public List<IHeatProductionUnitResultDataPoint> DataPoints { get; }
    
    public HeatProductionUnitSchedule(string name)
    {
        Name = name;
        DataPoints = new List<IHeatProductionUnitResultDataPoint>();
    }

    public void AddDataPoint(IHeatProductionUnitResultDataPoint dataPoint)
    {
        DataPoints.Add(dataPoint);
        UpdateProperties();
    }

    private void UpdateProperties()
    {
        UpdateTotalCost();
        UpdateMaxHeatProduction();
        UpdateResourceConsumption();
        UpdateTotalEmissions();
    }
    private void UpdateTotalCost()
    {
        TotalCost = DataPoints.Sum(x => x.Cost);
    }
    private void UpdateMaxHeatProduction()
    {
        MaxHeatProduction = DataPoints.Max(x => x.HeatProduction);
    }
    private void UpdateResourceConsumption()
    {
        ResourceConsumption = DataPoints.Sum(x => x.ResourceConsumption);
    }
    private void UpdateTotalEmissions()
    {
        TotalEmissions = DataPoints.Sum(x => x.Emissions);
    }
        
}