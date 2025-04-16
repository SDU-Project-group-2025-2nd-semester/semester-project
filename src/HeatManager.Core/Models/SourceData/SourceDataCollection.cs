using System.Collections.Immutable;

namespace HeatManager.Core.Models.SourceData;

public class SourceDataCollection
{
    public string Name { get; } // TODO: Possibly remove

    public IImmutableList<SourceDataPoint> DataPoints { get; }

    public SourceDataCollection(IEnumerable<SourceDataPoint> dataPoints)
    {
        DataPoints = dataPoints.ToImmutableList();
    }
}