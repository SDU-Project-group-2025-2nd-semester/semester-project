using System.Collections.Immutable;

namespace HeatManager.Core.Models.SourceData;

public class SourceDataCollection
{
    public IImmutableList<SourceDataPoint> DataPoints { get; set; } = [];

    // Used for serialization
    public SourceDataCollection() { }

    public SourceDataCollection(IEnumerable<SourceDataPoint> dataPoints)
    {
        DataPoints = dataPoints.ToImmutableList();
    }
}