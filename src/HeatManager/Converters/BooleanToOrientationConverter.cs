using Avalonia.Data.Converters;
using Avalonia.Layout;
using System;
using System.Globalization;

namespace HeatManager.Converters
{
    // Used in MainWindow.axaml
    // Determines whether the Save and Export buttons should be next to each other or below each other
    public class BooleanToOrientationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var parameters = (parameter as string)?.Split(',');
            if (parameters?.Length == 2 && value is bool b)
                return b ? Enum.Parse(typeof(Orientation), parameters[0]) : Enum.Parse(typeof(Orientation), parameters[1]);
            return Orientation.Horizontal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}