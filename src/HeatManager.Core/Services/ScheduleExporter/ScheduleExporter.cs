using CsvHelper;
using System.Globalization;

namespace HeatManager.Core.Services.ScheduleExporter;

public class ScheduleExporter : IScheduleExporter
{
    public void ExportScheduleData<T>(string filePath, IEnumerable<T> records)
    {

        using (var writer = new StreamWriter(filePath))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(records);
        }
    }
}