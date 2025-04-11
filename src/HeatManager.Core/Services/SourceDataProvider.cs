using HeatManager.Core.Models.SourceData;

namespace HeatManager.Core.Services;

internal class SourceDataProvider : ISourceDataProvider
{
    public SourceDataCollection SourceDataCollection { get; set; }


    public SourceDataProvider(IEnumerable<SourceDataPoint> dataPoints)
    {
        SourceDataCollection = new SourceDataCollection(dataPoints); 
    }
    
    public void SetDataCollection(SourceDataCollection collection)
    {
        SourceDataCollection = collection;
    }
    
    public SourceDataCollection GetDataCollection()
    {
        return SourceDataCollection;
    }
}