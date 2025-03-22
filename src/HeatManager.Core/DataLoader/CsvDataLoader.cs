using CsvHelper;
using CsvHelper.Configuration;
using HeatManager.Core.Models.SourceData;
using HeatManager.Core.Services;
using System.Globalization;

namespace HeatManager.Core.DataLoader;

public class CsvDataLoader : IDataLoader
{
    private readonly ISourceDataProvider _sourceDataProvider;
    
    public CsvDataLoader(ISourceDataProvider sourceDataProvider)
    {
        _sourceDataProvider = sourceDataProvider;
    }
    
    public void LoadData(string csvFilePath)
    {
        
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true // Ensures headers are read properly
        };
        
        
        using StreamReader reader = new(csvFilePath);
        using CsvReader csv = new(reader, CultureInfo.InvariantCulture);
        
        csv.Context.RegisterClassMap<SourceDataPointMap>();


        var records = csv.GetRecords<SourceDataPoint>().ToList();
        _sourceDataProvider.SetDataCollection(new SourceDataCollection(records));
    }
    
    private sealed class SourceDataPointMap : ClassMap<SourceDataPoint>
    {
        public SourceDataPointMap()
        {
            Map(m => m.ElectricityPrice);
            Map(m => m.HeatDemand);
            Map(m => m.TimeFrom).TypeConverterOption.Format("M/d/yyyy H:mm");
            Map(m => m.TimeTo).TypeConverterOption.Format("M/d/yyyy H:mm");
        }
    }

}