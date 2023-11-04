using System.Globalization;
using System.Windows.Data;
using NAS.Resources;

namespace NAS.View.Converters
{
  [ValueConversion(typeof(object), typeof(string))]
  public class EnumValueConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null)
      {
        return null;
      }

      var manager = NASResources.ResourceManager;
      return manager.GetString(value.ToString());
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
