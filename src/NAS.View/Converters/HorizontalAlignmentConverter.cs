using System.Globalization;
using System.Windows;
using System.Windows.Data;
using NAS.Resources;

namespace NAS.View.Converters
{
  [ValueConversion(typeof(Model.Enums.HorizontalAlignment), typeof(string))]
  public class HorizontalAlignmentConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null)
      {
        return DependencyProperty.UnsetValue;
      }

      var alignment = (Model.Enums.HorizontalAlignment)value;
      return alignment switch
      {
        Model.Enums.HorizontalAlignment.Left => NASResources.Left,
        Model.Enums.HorizontalAlignment.Center => NASResources.Center,
        Model.Enums.HorizontalAlignment.Right => NASResources.Right,
        _ => DependencyProperty.UnsetValue,
      };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return DependencyProperty.UnsetValue;
    }
  }
}
