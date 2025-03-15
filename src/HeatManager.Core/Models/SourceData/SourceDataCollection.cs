namespace HeatManager.Core.Models.SourceData;

    public class SourceDataCollection : ISourceDataCollection
    {
        public string Name { get; } // TODO: Possibly remove

        public List<ISourceDataPoint> DataPoints { get; }
        public SourceDataCollection(IEnumerable<ISourceDataPoint> dataPoints)
        {
            DataPoints = new List<ISourceDataPoint>(dataPoints); 
        }
    }