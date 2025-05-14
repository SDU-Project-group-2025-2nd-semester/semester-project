using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using LiveChartsCore.SkiaSharpView.SKCharts;
using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia;


namespace HeatManager.Services.FileServices;

public interface IChartExporter
{
    Task Export<TChart>(TChart chart, string FilenamePrefix = "Chart") where TChart : InMemorySkiaSharpChart;
    Task ExportControl<T>(T control, ISeries[]? series = null, object? xAxes = null, object? yAxes = null, string filenamePrefix = "Chart") where T : class;
}