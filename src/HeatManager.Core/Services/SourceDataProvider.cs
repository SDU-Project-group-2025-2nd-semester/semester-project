using HeatManager.Core.Models.SourceData;

namespace HeatManager.Core.Services;

public class SourceDataProvider : ISourceDataProvider
{
    public ISourceDataCollection SourceDataCollection { get; set; }


    public SourceDataProvider(IEnumerable<ISourceDataPoint> dataPoints)
    {
        SourceDataCollection = new SourceDataCollection(dataPoints);; 
    }
    
    public void SetDataCollection(ISourceDataCollection collection)
    {
        SourceDataCollection = collection;
    }
    
    public ISourceDataCollection GetDataCollection()
    {
        return SourceDataCollection;
    }
}