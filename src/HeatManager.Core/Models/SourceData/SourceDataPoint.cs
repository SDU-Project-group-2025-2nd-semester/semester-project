namespace HeatManager.Core.Models.SourceData;

/// <summary>
///
/// Used for parsing of source csv to our data
/// </summary>
public class SourceDataPoint
{
    // TODO: Possibly change to record

    public required DateTime TimeFrom { get; init; }

    public required DateTime TimeTo { get; init; }

    /// <summary>
    /// MWh(th)
    /// </summary>
    public required double HeatDemand { get; init; }

    /// <summary>
    /// DKK/MWh(el)
    /// </summary>
    public required decimal ElectricityPrice { get; init; }
}