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
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.Defaults;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.VisualElements;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

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

    public async Task ExportControl<T>(T control, ISeries[]? series = null, object? xAxes = null, object? yAxes = null, string filenamePrefix = "Chart", string title = "") where T : class
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
                    YAxes = yAxes as ICartesianAxis[] ?? cartesianChart.YAxes as ICartesianAxis[],
                    Title = new LabelVisual
                    {
                        Text = title,
                        TextSize = 20,
                        Padding = new LiveChartsCore.Drawing.Padding(15),
                        Paint = new SolidColorPaint(SKColors.Black)
                    }
                };

            }
            else if (control is LiveChartsCore.SkiaSharpView.Avalonia.PieChart pieChart)
            {
                var skPieChart = new SKPieChart
                {
                    Width = (int)pieChart.Bounds.Width,
                    Height = (int)pieChart.Bounds.Height,
                    Series = series?.ToArray() ?? pieChart.Series,

                    InitialRotation = pieChart.InitialRotation,
                    MaxValue = pieChart.MaxValue,
                    MinValue = pieChart.MinValue,
                    Title = new LabelVisual
                    {
                        Text = title,
                        TextSize = 20,
                        Padding = new LiveChartsCore.Drawing.Padding(15),
                        Paint = new SolidColorPaint(SKColors.Black)
                    }
                };

                // Ensure UI styling is applied
                foreach (var serie in skPieChart.Series)
                {
                    if (serie is PieSeries<ObservableValue> pieSeries)
                    {
                        // Make sure data labels are visible
                        pieSeries.DataLabelsPosition = PolarLabelsPosition.End;
                        pieSeries.DataLabelsSize = 11;
                    }
                }
                skChart = skPieChart;

            }
            else if (control is LiveChartsCore.SkiaSharpView.Avalonia.PolarChart polarChart)
            {
                skChart = new SKPolarChart
                {
                    Width = (int)polarChart.Bounds.Width,
                    Height = (int)polarChart.Bounds.Height,
                    Series = series ?? polarChart.Series,
                    AngleAxes = polarChart.AngleAxes,
                    RadiusAxes = polarChart.RadiusAxes,
                    Title = new LabelVisual
                    {
                        Text = title,
                        TextSize = 20,
                        Padding = new LiveChartsCore.Drawing.Padding(15),
                        Paint = new SolidColorPaint(SKColors.Black)
                    }
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
