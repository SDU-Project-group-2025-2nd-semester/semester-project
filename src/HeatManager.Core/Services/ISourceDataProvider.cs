using HeatManager.Core.Models.SourceData;

namespace HeatManager.Core.Services;

public interface ISourceDataProvider
{
    ISourceDataCollection SourceDataCollection { get; set; }
    void SetDataCollection(ISourceDataCollection sourceDataCollection);
    ISourceDataCollection GetDataCollection();
}