using System;

namespace HeatManager.Core.Services.ScheduleExporter;

public interface IScheduleExporter
{
    public void ExportScheduleData<T>(string filePath, IEnumerable<T> records)
    {

    }

    public void ExportScheduleData(string filePath, double[] records)
    {
        
    }
}
