using Avalonia.Platform.Storage;
using CsvHelper;
using CsvHelper.Configuration;
using HeatManager.Core.Models.SourceData;
using HeatManager.Core.Services.SourceDataProviders;
using System.Globalization;

namespace HeatManager.Core.DataLoader;

public class CsvDataLoader : IDataLoader
{
    private readonly ISourceDataProvider _sourceDataProvider;
    /// <summary>
    /// Initializes a new instance of the <see cref="CsvDataLoader"/> class.
    /// </summary>
    /// <param name="sourceDataProvider">The source data provider to populate with loaded data.</param>
    public CsvDataLoader(ISourceDataProvider sourceDataProvider)
    {
        _sourceDataProvider = sourceDataProvider;
    }
    /// <summary>
    /// Loads data from the specified CSV file and populates the source data collection.
    /// </summary>
    /// <param name="csvFilePath">The file path of the CSV file to load data from.</param>
    public void LoadData(string csvFilePath)
    {
        // Configuration for the CSV reader, specifying culture and header handling.
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true // Ensures headers are read properly
        };

        // Reads the CSV file and maps its content to SourceDataPoint objects.
        using StreamReader reader = new(csvFilePath);

        LoadData(reader);

    }

    private void LoadData(StreamReader reader)
    {
        using CsvReader csv = new(reader, CultureInfo.InvariantCulture);
        
        // Registers the mapping configuration for SourceDataPoint.
        csv.Context.RegisterClassMap<SourceDataPointMap>();

        // Reads and converts the CSV records into a list of SourceDataPoint objects.
        var records = csv.GetRecords<SourceDataPoint>().ToList();
        
        // Populates the source data collection in the source data provider.
        _sourceDataProvider.SourceDataCollection = new SourceDataCollection(records);
    }

    public async Task LoadData(IStorageFile csvFilePath)
    {
        await using var stream = await csvFilePath.OpenReadAsync();
        using var reader = new StreamReader(stream);
        LoadData(reader);
    }

    /// <summary>
    /// A private class that defines the mapping between CSV columns and the <see cref="SourceDataPoint"/> properties.
    /// </summary>
    private sealed class SourceDataPointMap : ClassMap<SourceDataPoint>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SourceDataPointMap"/> class.
        /// Configures the mapping for each property of <see cref="SourceDataPoint"/>.
        /// </summary>
        public SourceDataPointMap()
        {
            Map(m => m.ElectricityPrice);
            Map(m => m.HeatDemand);
            Map(m => m.TimeFrom).TypeConverterOption.Format("M/d/yyyy H:mm");
            Map(m => m.TimeTo).TypeConverterOption.Format("M/d/yyyy H:mm");
        }
    }

}