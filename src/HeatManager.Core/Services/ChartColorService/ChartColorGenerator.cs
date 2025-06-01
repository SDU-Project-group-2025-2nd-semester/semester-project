using SkiaSharp;

namespace HeatManager.Services.ChartColorService;

public class ChartColorGenerator : IChartColorGenerator
{
    private byte R;
    private byte G;
    private byte B;

    /// <summary>
    /// Converts a string to a consistent RGB color, ensuring adequate brightness for visibility in charts.
    /// </summary>
    public SKColor SetColor(string parameter)
    {
        var rand = new Random();
        int hash = StringToInt(parameter);

        R = (byte)((hash & 0xFF0000) >> 16);
        G = (byte)((hash & 0x00FF00) >> 8);
        B = (byte)((hash & 0x0000FF));

        EnsureProperBrightness(ref R, ref G, ref B);

        return new SKColor(R, G, B);
    }

    /// <summary>
    /// Converts string to integer hash value used for deterministic color generation.
    /// </summary>
    private int StringToInt(string parameter)
    {
        unchecked
        {
            int hash = 100;

            foreach (char item in parameter)
            {
                hash = hash * 31 + hash * item ^ hash;
            }
            return hash;
        }
    }

    /// <summary>
    /// Ensures colors meet minimum brightness threshold by boosting the strongest component if needed.
    /// </summary>
    private void EnsureProperBrightness(ref byte r, ref byte g, ref byte b)
    {
        const byte MIN_BRIGHTNESS = 80;

        if (r < MIN_BRIGHTNESS && g < MIN_BRIGHTNESS && b < MIN_BRIGHTNESS)
        {
            if (r >= g && r >= b)
                r = (byte)Math.Max(r + MIN_BRIGHTNESS, 255);
            else if (g >= r && g >= b)
                g = (byte)Math.Max(g + MIN_BRIGHTNESS, 255);
            else
                b = (byte)Math.Max(b + MIN_BRIGHTNESS, 255);
        }
    }
}