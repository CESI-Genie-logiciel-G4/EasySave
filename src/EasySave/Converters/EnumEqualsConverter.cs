using System.Globalization;
using Avalonia.Data.Converters;

namespace EasySave.Converters;

public class EnumEqualsConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var parameters = parameter?.ToString()?.Split('|');

        if (value == null || parameters == null)
            return false;

        foreach (var parameterString in parameters)
        {
            if (!Enum.TryParse(value.GetType(), parameterString, out var enumValue)) continue;

            if (enumValue.Equals(value))
                return true;
        }

        return false;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value;
    }
}