using System.Globalization;
using System.Windows;
using System.Windows.Data;
using NAS.Resources;

namespace NAS.View.Converters
{
  [ValueConversion(typeof(Model.Enums.HorizontalAlignment), typeof(string))]
  public class VerticalAlignmentConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null)
      {
        return DependencyProperty.UnsetValue;
      }

      var alignment = (Model.Enums.VerticalAlignment)value;
      return alignment switch
      {
        Model.Enums.VerticalAlignment.Top => NASResources.Top,
        Model.Enums.VerticalAlignment.Center => NASResources.Center,
        Model.Enums.VerticalAlignment.Bottom => NASResources.Bottom,
        _ => DependencyProperty.UnsetValue,
      };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return DependencyProperty.UnsetValue;
    }
  }
}
