

using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using LiveChartsCore.SkiaSharpView.SKCharts;

using HeatManager;
using Avalonia;
using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;

namespace HeatManager.Services.FileServices;

public class ChartExporter() : IChartExporter
{
    private string _filename = $"Chart-{DateTime.Now:MMdd_HHmmss}.png";

    public async Task Export(object chartParam, ISeries[] chartSeries, ICartesianAxis[] scrollableAxes, ICartesianAxis[] YAxes)
    {
        try
        {
            var chartControl = chartParam as LiveChartsCore.SkiaSharpView.Avalonia.CartesianChart;

            if (chartControl == null)
            {
                Console.WriteLine("Chart control not found");
                return;
            }

            // Create a SkiaSharp version of the chart
            var skChart = new SKCartesianChart()
            {
                Width = (int)chartControl.Bounds.Width,
                Height = (int)chartControl.Bounds.Height,
                Series = chartSeries,
                XAxes = scrollableAxes,
                YAxes = YAxes
            };

            // Generate temp file
            var tempDirectory = Path.GetTempPath();
            var tempFilename = Path.Combine(tempDirectory, _filename);
            skChart.SaveImage(tempFilename);

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
            Console.WriteLine($"Error creating chart image: {ex.Message}");
        }
    }

    public Task Export(object chartParam)
    {
        throw new NotImplementedException();
    }
}
