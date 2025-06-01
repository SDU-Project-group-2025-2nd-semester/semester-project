namespace HeatManager.Core.Services.ScheduleExporter;

public interface IScheduleExporter
{
    public void ExportScheduleData<T>(string filePath, IEnumerable<T> records)
    {

    }
}