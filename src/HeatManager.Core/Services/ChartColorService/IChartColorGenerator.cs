using SkiaSharp;

namespace HeatManager.Services.ChartColorService;

public interface IChartColorGenerator
{
    /// <summary>
    /// Generates a deterministic, visually distinct color based on the provided string parameter.
    /// </summary>
    public SKColor SetColor(string parameter);
}