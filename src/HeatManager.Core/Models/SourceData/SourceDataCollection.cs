using System.Collections.Immutable;

namespace HeatManager.Core.Models.SourceData;

public class SourceDataCollection(IEnumerable<SourceDataPoint> dataPoints)
{
    public IImmutableList<SourceDataPoint> DataPoints { get; } = dataPoints.ToImmutableList();
}