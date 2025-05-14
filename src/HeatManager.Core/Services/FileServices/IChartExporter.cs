

namespace HeatManager.Services.FileServices;

public interface IChartExporter
{
    public Task Export(object chartParam);
}