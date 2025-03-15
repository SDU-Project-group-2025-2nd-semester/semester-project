namespace HeatManager.Core.Models.SourceData;

public class SourceDataCollection : ISourceDataCollection
{
    public string Name { get; } // TODO: Possibly remove

    public IEnumerable<ISourceDataPoint> DataPoints { get; }

    public SourceDataCollection(IEnumerable<ISourceDataPoint> dataPoints)
    {
        DataPoints = dataPoints;
    }
}