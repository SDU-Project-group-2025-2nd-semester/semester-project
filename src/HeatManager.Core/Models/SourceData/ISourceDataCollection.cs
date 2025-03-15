namespace HeatManager.Core.Models.SourceData;

public interface ISourceDataCollection
{
    string Name { get; }
    List<ISourceDataPoint> DataPoints { get; }

}