using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace NAS.Views.Converters
{
  [ValueConversion(typeof(decimal), typeof(string))]
  public class CurrencyConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null)
      {
        return null;
      }

      decimal d = (decimal)value;
      return d.ToString("N") + " €";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      string s = value as string;
      if (s.EndsWith("€"))
      {
        s = s.Remove(s.Length - 1);
      }

      if (s.StartsWith("€"))
      {
        s = s.Remove(0, 1);
      }

      s = s.Trim();

      return decimal.TryParse(s, out decimal result) ? result : DependencyProperty.UnsetValue;
    }
  }
}
