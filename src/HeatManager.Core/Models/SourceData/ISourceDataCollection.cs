using System.Collections.Immutable;

namespace HeatManager.Core.Models.SourceData;

public interface ISourceDataCollection
{
    string Name { get; }
    IImmutableList<ISourceDataPoint> DataPoints { get; }

}