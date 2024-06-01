using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace NAS.Views.Converters
{
  [ValueConversion(typeof(double), typeof(bool))]
  public class ZoomToBoolConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value != null && parameter != null)
      {
        var zoom = (double)value;
        if (double.TryParse(parameter.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out double p))
        {
          return Math.Abs(zoom - p) < 0.00001;
        }
      }

      // Something went wrong
      return new ApplicationException("ZoomToBool Converter not correctly configured.");
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return DependencyProperty.UnsetValue;
    }
  }
}
