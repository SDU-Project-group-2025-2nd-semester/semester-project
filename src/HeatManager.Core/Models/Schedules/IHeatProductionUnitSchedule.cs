namespace HeatManager.Core.Models.Schedules;

public interface IHeatProductionUnitSchedule /*: IHeatProductionUnit*/
{
    public string Name { get; }
    public decimal TotalCost { get; }
    public double MaxHeatProduction { get; }
    public double ResourceConsumption { get; }
    public double TotalEmissions { get; }
    
    public List<IHeatProductionUnitResultDataPoint> DataPoints { get; }
    void AddDataPoint(IHeatProductionUnitResultDataPoint dataPoint);
}