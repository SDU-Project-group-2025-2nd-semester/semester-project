using LiveChartsCore.SkiaSharpView.SKCharts;
using LiveChartsCore;

namespace HeatManager.Services.FileServices;

public interface IChartExporter
{
    Task Export<TChart>(TChart chart, string FilenamePrefix = "Chart") where TChart : InMemorySkiaSharpChart;
    Task ExportControl<T>(T control, ISeries[]? series = null, object? xAxes = null, object? yAxes = null, string filenamePrefix = "Chart", string title = "") where T : class;
}