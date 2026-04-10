using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace METARLookupWPF.Converters;

public class FlightCategoryBrushConverter : IValueConverter
{
    public static readonly FlightCategoryBrushConverter Instance = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value?.ToString() switch
        {
            "VFR" => new SolidColorBrush(Color.FromRgb(0x22, 0xBB, 0x45)),
            "MVFR" => new SolidColorBrush(Color.FromRgb(0x22, 0x88, 0xFF)),
            "IFR" => new SolidColorBrush(Color.FromRgb(0xEE, 0x44, 0x33)),
            "LIFR" => new SolidColorBrush(Color.FromRgb(0xAA, 0x22, 0xAA)),
            _ => new SolidColorBrush(Colors.Gray),
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}

public class FlightCategoryBackgroundConverter : IValueConverter
{
    public static readonly FlightCategoryBackgroundConverter Instance = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value?.ToString() switch
        {
            "VFR" => new SolidColorBrush(Color.FromArgb(0x30, 0x22, 0xBB, 0x45)),
            "MVFR" => new SolidColorBrush(Color.FromArgb(0x30, 0x22, 0x88, 0xFF)),
            "IFR" => new SolidColorBrush(Color.FromArgb(0x30, 0xEE, 0x44, 0x33)),
            "LIFR" => new SolidColorBrush(Color.FromArgb(0x30, 0xAA, 0x22, 0xAA)),
            _ => new SolidColorBrush(Color.FromArgb(0x10, 0x80, 0x80, 0x80)),
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}

public class BoolToVisibilityConverter : IValueConverter
{
    public static readonly BoolToVisibilityConverter Instance = new();
    public bool Invert { get; set; }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool v = value is bool b && b;
        if (Invert) v = !v;
        return v ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}

public class InverseBoolToVisibilityConverter : IValueConverter
{
    public static readonly InverseBoolToVisibilityConverter Instance = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool v = value is bool b && b;
        return v ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}

public class SigmetBadgeConverter : IValueConverter
{
    public static readonly SigmetBadgeConverter Instance = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int count && count > 0)
            return System.Windows.Visibility.Visible;
        return System.Windows.Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}
