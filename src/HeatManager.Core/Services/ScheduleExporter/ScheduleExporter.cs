using CsvHelper;
using HeatManager.Core.Models.Schedules;
using System;
using System.Globalization;
using System.Linq;

namespace HeatManager.Core.Services.ScheduleExporter;

public class ScheduleExporter
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
