using System.Globalization;
using System.Windows;
using System.Windows.Data;
using NAS.Resources;

namespace NAS.Views.Converters
{
  [ValueConversion(typeof(Models.Enums.HorizontalAlignment), typeof(string))]
  public class VerticalAlignmentConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null)
      {
        return DependencyProperty.UnsetValue;
      }

      var alignment = (Models.Enums.VerticalAlignment)value;
      return alignment switch
      {
        Models.Enums.VerticalAlignment.Top => NASResources.Top,
        Models.Enums.VerticalAlignment.Center => NASResources.Center,
        Models.Enums.VerticalAlignment.Bottom => NASResources.Bottom,
        _ => DependencyProperty.UnsetValue,
      };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return DependencyProperty.UnsetValue;
    }
  }
}
