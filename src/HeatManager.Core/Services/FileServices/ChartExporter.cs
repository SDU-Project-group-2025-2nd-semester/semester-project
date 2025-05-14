using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView.SKCharts;

using System;
using System.IO;
using System.Threading.Tasks;
using HeatManager;

namespace HeatManager.Services.FileServices;

public class ChartExporter() : IChartExporter
{
    private string _filename = "";

    public async Task Export<TChart>(TChart chart, string FilenamePrefix = "Chart") where TChart : InMemorySkiaSharpChart
    {
        try
        {

            _filename = $"{FilenamePrefix}-{DateTime.Now:MMdd_HHmmss}.png";

            // Generate temp file
            var tempDirectory = Path.GetTempPath();
            var tempFilename = Path.Combine(tempDirectory, _filename);
            chart.SaveImage(tempFilename);

            var topLevel = TopLevel.GetTopLevel((Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow);

            // File Dialog
            var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Save As Image",
                SuggestedFileName = _filename,
                DefaultExtension = "png",
                FileTypeChoices = new[]
                {
                new FilePickerFileType("PNG Image") { Patterns = new[] { "*.png" } },
                new FilePickerFileType("JPEG Image") { Patterns = new[] { "*.jpg", "*.jpeg" } }
            }
            });

            if (file is not null)
            {
                // Copy the temporary image to selected location
                await using var sourceStream = File.OpenRead(tempFilename);
                await using var destinationStream = await file.OpenWriteAsync();
                await sourceStream.CopyToAsync(destinationStream);

                Console.WriteLine($"Chart image saved successfully to {file.Path}");

            }
            else
            {
                Console.WriteLine("Save As was canceled by user");
            }

            // Clean up temporary file
            try { File.Delete(tempFilename); } catch { }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while saving chart image: {ex.Message}");
        }
    }

    public async Task ExportControl<T>(T control, ISeries[]? series = null, object? xAxes = null, object? yAxes = null, string filenamePrefix = "Chart") where T : class
    {
        try
        {

            InMemorySkiaSharpChart skChart = null;

            if (control is LiveChartsCore.SkiaSharpView.Avalonia.CartesianChart cartesianChart)
            {
                skChart = new SKCartesianChart
                {
                    Width = (int)cartesianChart.Bounds.Width,
                    Height = (int)cartesianChart.Bounds.Height,
                    Series = series ?? cartesianChart.Series,
                    XAxes = xAxes as ICartesianAxis[] ?? cartesianChart.XAxes as ICartesianAxis[],
                    YAxes = yAxes as ICartesianAxis[] ?? cartesianChart.YAxes as ICartesianAxis[]
                };

            }
            else if (control is LiveChartsCore.SkiaSharpView.Avalonia.PieChart pieChart)
            {
                skChart = new SKPieChart
                {
                    Width = (int)pieChart.Bounds.Width,
                    Height = (int)pieChart.Bounds.Height,
                    Series = series ?? pieChart.Series
                };

            }
            else if (control is LiveChartsCore.SkiaSharpView.Avalonia.PolarChart polarChart)
            {
                skChart = new SKPolarChart
                {
                    Width = (int)polarChart.Bounds.Width,
                    Height = (int)polarChart.Bounds.Height,
                    Series = series ?? polarChart.Series,
                    AngleAxes = polarChart.AngleAxes,
                    RadiusAxes = polarChart.RadiusAxes
                };

            }
            else
            {
                throw new ArgumentException($"Unsupported chart type: {control?.GetType().Name}");
            }
            if (skChart != null)
            {
                await Export(skChart, filenamePrefix);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error exporting SKChart: {ex.Message}");
        }
    }
}
