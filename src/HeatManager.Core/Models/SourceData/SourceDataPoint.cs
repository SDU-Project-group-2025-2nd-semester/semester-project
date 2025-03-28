﻿namespace HeatManager.Core.Models.SourceData;

/// <summary>
///
/// Used for parsing of source csv to our data
/// </summary>
public class SourceDataPoint : ISourceDataPoint
{
    // TODO: Possibly change to record

    public required DateTime TimeFrom { get; init; }

    public required DateTime TimeTo { get; init; }

    public required double HeatDemand { get; init; }

    public required decimal ElectricityPrice { get; init; }
}