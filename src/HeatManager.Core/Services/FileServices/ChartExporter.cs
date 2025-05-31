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
                var CartesianskChart = new SKCartesianChart
                {
                    Width = (int)cartesianChart.Bounds.Width,
                    Height = (int)cartesianChart.Bounds.Height,
                    Series = series?.ToArray() ?? cartesianChart.Series,
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
                skChart = CartesianskChart;

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
                await ShowStatusNotification("Chart exported successfully", false);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error exporting SKChart: {ex.Message}");
            await ShowStatusNotification("Error exporting Chart", true);
        }
    }

    /// <summary>
    /// Displays a temporary notification popup with a status message.
    /// </summary>
    /// <param name="message">The message to display in the notification.</param>
    /// <param name="isError">Whether the notification represents an error state (true) or success state (false).</param>
    /// <remarks>
    /// The notification appears near the bottom center of the main application window for 1.5 seconds
    /// before automatically closing. The notification styling (colors, borders) changes based on whether
    /// it represents an error or success state.
    /// </remarks>
    private async Task ShowStatusNotification(string message, bool isError)
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainWindow = desktop.MainWindow;
            if (mainWindow != null)
            {
                //Custom text 
                var TextBlock = new Avalonia.Controls.TextBlock
                {
                    Text = message,
                    FontSize = 14,
                    Foreground = isError ? Avalonia.Media.Brushes.DarkRed : Avalonia.Media.Brushes.DarkGreen,
                    TextAlignment = Avalonia.Media.TextAlignment.Center,
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                    Margin = new Thickness(10)
                };

                //notification popup
                var popup = new Avalonia.Controls.Window
                {
                    Title = isError ? "Error" : "Success",
                    Content = TextBlock,
                    Width = 400,
                    Height = 50,
                    WindowStartupLocation = Avalonia.Controls.WindowStartupLocation.CenterOwner,
                    Background = isError ? Avalonia.Media.Brushes.LightPink : Avalonia.Media.Brushes.LightGreen,
                    SystemDecorations = Avalonia.Controls.SystemDecorations.BorderOnly,
                    CanResize = false,
                    ShowInTaskbar = false,
                    Topmost = true
                };

                popup.Show(mainWindow);
                popup.Position = new PixelPoint((int)(mainWindow.Position.X + mainWindow.Width / 2), (int)(mainWindow.Position.Y + mainWindow.Height));

                // Auto-close
                await Task.Delay(1500);
                popup.Close();
            }
        }
    }
}
