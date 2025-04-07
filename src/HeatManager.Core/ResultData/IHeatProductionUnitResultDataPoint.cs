namespace HeatManager.Core.Models.Schedules;

public interface IHeatProductionUnitResultDataPoint
{
    DateTime TimeFrom { get; }
    DateTime TimeTo { get; }
    double Utilization { get; }
    double HeatProduction { get; }
    decimal Cost { get; }
    double ResourceConsumption { get; }
    double Emissions { get; }
}