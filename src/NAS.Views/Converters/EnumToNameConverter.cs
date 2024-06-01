using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;
using ES.Tools.Core.Infrastructure;
using NAS.Resources;

namespace NAS.Views.Converters
{
  public class EnumToNameConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      Debug.Assert(value.GetType().IsEnum);
      string enumValue = value.ToString();
      var manager = NASResources.ResourceManager;
      string translated =  manager.GetString(enumValue.ToString());
      if (string.IsNullOrWhiteSpace(translated))
      {
        if (!Utilities.IsDesignTime)
        {
          Debug.Fail($"Enum value {enumValue} cannot be found in resources.");
        }

        return enumValue.ToString();
      }
      return translated;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
