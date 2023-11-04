using System.Globalization;
using System.Windows.Data;
using NAS.Model.Enums;

namespace NAS.View.Converters
{
  [ValueConversion(typeof(ActivityProperty), typeof(string))]
  public class ActivityPropertyConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return value is ActivityProperty item ? ActivityPropertyHelper.GetNameOfActivityProperty(item) : null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      string strValue = value as string;
      return ActivityPropertyHelper.GetActivityPropertyByName(strValue);
    }
  }
}
