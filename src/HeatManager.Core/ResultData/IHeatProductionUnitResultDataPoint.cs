namespace HeatManager.Core.ResultData;

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