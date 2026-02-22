using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PcOptimizationTool.Helpers
{
    /// <summary>
    /// Returns Visibility.Collapsed when the bound bool is true,
    /// and Visibility.Visible when false.
    /// </summary>
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class InverseBoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is true ? Visibility.Collapsed : Visibility.Visible;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => value is Visibility.Collapsed;
    }
}
