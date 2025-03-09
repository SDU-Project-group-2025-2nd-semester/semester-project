namespace HeatManager.Core.Models.SourceData;

public class SourceDataPoint : ISourceDataPoint
{
    // TODO: Possibly change to record

    public required DateTime TimeForm { get; init; }

    public required DateTime TimeTo { get; init; }

    public required double HeatDemand { get; init; }

    public required decimal ElectricityPrice { get; init; }
}