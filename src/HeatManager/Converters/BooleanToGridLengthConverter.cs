using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace HeatManager.Converters
{
    public class BooleanToGridLengthConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool isOpen && parameter is string sizes)
            {
                var sizeArray = sizes.Split(',');
                if (sizeArray.Length == 2 && double.TryParse(sizeArray[0], out var openSize) && double.TryParse(sizeArray[1], out var closedSize))
                {
                    // Return GridLength for Width binding
                    return new GridLength(isOpen ? openSize : closedSize, GridUnitType.Pixel);
                }
            }

            // Default to 0 pixels
            return new GridLength(0, GridUnitType.Pixel);
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}