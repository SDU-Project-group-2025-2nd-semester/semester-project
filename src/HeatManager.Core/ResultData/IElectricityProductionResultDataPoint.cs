namespace HeatManager.Core.ResultData;

public interface IElectricityProductionResultDataPoint
{
    DateTime TimeFrom { get; }
    DateTime TimeTo { get; }
    decimal ElectricityPrice { get; }
    double ElectricityProduction { get; }
}