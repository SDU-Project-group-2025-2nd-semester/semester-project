namespace HeatManager.Core.Models.SourceData;

public interface ISourceDataCollection
{
    string Name { get; }
    IEnumerable<ISourceDataPoint> DataPoints { get; }

}