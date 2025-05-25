using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace HeatManager.Converters
{
    // Used in the MainWindow.axaml
    // If the Menu pane is open, the texts show up on the buttons
    // If it is closed, the icons show up
    public class BooleanToContentConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool isOpen && parameter is string content)
            {
                var contents = content.Split('|');
                return isOpen ? contents[0] : contents[1];
            }
            return string.Empty;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}