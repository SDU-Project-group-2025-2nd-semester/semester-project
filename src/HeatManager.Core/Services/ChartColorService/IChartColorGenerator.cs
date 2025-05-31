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

using SkiaSharp;

namespace HeatManager.Services.ChartColorService;

public interface IChartColorGenerator
{
    /// <summary>
    /// Generates a deterministic, visually distinct color based on the provided string parameter.
    /// </summary>
    public SKColor SetColor(string parameter);
}