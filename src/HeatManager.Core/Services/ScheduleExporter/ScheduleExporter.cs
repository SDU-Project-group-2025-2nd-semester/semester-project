using CsvHelper;
using HeatManager.Core.Models.Schedules;
using System;
using System.Globalization;
using System.Linq;

namespace HeatManager.Core.Services.ScheduleExporter;

public class ScheduleExporter
{
    public void ExportScheduleData(string filePath, Schedule schedule)
    {


        using (var writer = new StreamWriter(filePath))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(schedule.HeatProductionUnitSchedules);
            //csv.WriteRecords(schedule.ElectricityProductionUnitSchedules);
            
        }
    }
}
