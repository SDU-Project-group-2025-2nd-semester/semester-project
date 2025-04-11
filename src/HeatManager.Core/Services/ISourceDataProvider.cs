using HeatManager.Core.Models.SourceData;

namespace HeatManager.Core.Services;

public interface ISourceDataProvider
{
    public SourceDataCollection SourceDataCollection { get; set; }

    public void SetDataCollection(SourceDataCollection sourceDataCollection);

    public SourceDataCollection GetDataCollection();
}