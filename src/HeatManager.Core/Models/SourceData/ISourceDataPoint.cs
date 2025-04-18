namespace HeatManager.Core.Models.SourceData;

public interface ISourceDataPoint
{
    DateTime TimeFrom { get; }
    DateTime TimeTo { get; }
    double HeatDemand { get; }
    decimal ElectricityPrice { get; }
} 