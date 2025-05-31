using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace HeatManager.Converters
{
    // Used in the MainWindow.axaml
    // Shows tooltip only when pane is closed (hovering over the icons)
    public class ContentToTooltipConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool isOpen && parameter is string tooltipText)
            {
                return !isOpen ? tooltipText : null;
            }
            return null;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}