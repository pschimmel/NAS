using System.Globalization;
using System.Windows.Data;

namespace NAS.Views.Converters
{
  [ValueConversion(typeof(string), typeof(DateTime))]
  public class StringToDateConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return string.IsNullOrWhiteSpace(value?.ToString())
        ? null
        : DateTime.TryParse(value.ToString(), out var date) ? date.Date : null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return string.IsNullOrWhiteSpace(value?.ToString()) ? null : ((DateTime)value).ToString("d", culture);
    }
  }
}
