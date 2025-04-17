namespace HeatManager.Core.Models.Schedules;

public class HeatProductionUnitResultDataPoint : IHeatProductionUnitResultDataPoint
{
    public DateTime TimeFrom { get; }
    public DateTime TimeTo { get; }
    public double Utilization { get; }
    public double HeatProduction { get; }
    public decimal Cost { get; }
    public double ResourceConsumption { get; }
    public double Emissions { get; }

    public HeatProductionUnitResultDataPoint(DateTime timeFrom, DateTime timeTo, double utilization, double heatProduction, decimal cost, double resourceConsumption, double emissions)
    {
        TimeFrom = timeFrom;
        TimeTo = timeTo;
        Utilization = utilization;
        HeatProduction = heatProduction;
        Cost = cost;
        ResourceConsumption = resourceConsumption;
        Emissions = emissions;
    }
}