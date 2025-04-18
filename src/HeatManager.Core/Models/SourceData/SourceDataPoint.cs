namespace HeatManager.Core.Models.SourceData;

/// <summary>
///
/// Used for parsing of source csv to our data
/// </summary>
public class SourceDataPoint : ISourceDataPoint
{
    // TODO: Possibly change to record

    public DateTime TimeFrom { get; set; }

    public DateTime TimeTo { get; set; }

    /// <summary>
    /// MWh(th)
    /// </summary>
    public double HeatDemand { get; set; }

    /// <summary>
    /// DKK/MWh(el)
    /// </summary>
    public decimal ElectricityPrice { get; set; }
}