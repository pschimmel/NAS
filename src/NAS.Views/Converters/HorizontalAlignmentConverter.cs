using System.Globalization;
using System.Windows;
using System.Windows.Data;
using NAS.Resources;

namespace NAS.Views.Converters
{
  [ValueConversion(typeof(Models.Enums.HorizontalAlignment), typeof(string))]
  public class HorizontalAlignmentConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null)
      {
        return DependencyProperty.UnsetValue;
      }

      var alignment = (Models.Enums.HorizontalAlignment)value;
      return alignment switch
      {
        Models.Enums.HorizontalAlignment.Left => NASResources.Left,
        Models.Enums.HorizontalAlignment.Center => NASResources.Center,
        Models.Enums.HorizontalAlignment.Right => NASResources.Right,
        _ => DependencyProperty.UnsetValue,
      };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return DependencyProperty.UnsetValue;
    }
  }
}
