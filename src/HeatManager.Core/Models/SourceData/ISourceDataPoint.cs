namespace HeatManager.Core.Models.SourceData;

public interface ISourceDataPoint
{
    /// <summary>
    /// DKK/MWh(el)
    /// </summary>
    decimal ElectricityPrice { get; init; }

    /// <summary>
    /// MWh(th)
    /// </summary>
    double HeatDemand { get; init; }

    DateTime TimeForm { get; init; }

    DateTime TimeTo { get; init; }
}