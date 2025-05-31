using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace HeatManager.DataConverter;

public class EnumToInverseBoolConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null || parameter == null) return true;
        return value.ToString() != parameter.ToString();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

