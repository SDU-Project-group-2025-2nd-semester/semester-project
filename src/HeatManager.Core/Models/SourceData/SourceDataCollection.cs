using System.Collections.Immutable;

namespace HeatManager.Core.Models.SourceData;

internal class SourceDataCollection : ISourceDataCollection
{
    public string Name { get; } // TODO: Possibly remove

    public IImmutableList<ISourceDataPoint> DataPoints { get; }

    public SourceDataCollection(IEnumerable<ISourceDataPoint> dataPoints)
    {
        DataPoints = dataPoints.ToImmutableList();
    }
}