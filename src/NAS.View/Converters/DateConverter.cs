using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace NAS.View.Converters
{
  [ValueConversion(typeof(DateTime), typeof(string))]
  public class DateConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (string.IsNullOrWhiteSpace(value?.ToString()))
      {
        return null;
      }

      var date = (DateTime)value;
      return parameter != null ? date.ToString(parameter.ToString()) : date.ToShortDateString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value != null)
      {
        string strValue = value.ToString();
        if (DateTime.TryParse(strValue, out var resultDateTime))
        {
          return resultDateTime;
        }
      }
      return DependencyProperty.UnsetValue;
    }
  }
}
