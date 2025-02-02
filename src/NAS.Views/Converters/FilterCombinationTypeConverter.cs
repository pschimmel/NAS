using System.Globalization;
using System.Windows.Data;
using NAS.Models.Enums;
using NAS.ViewModels.Helpers;

namespace NAS.Views.Converters
{
  [ValueConversion(typeof(FilterCombinationType), typeof(string))]
  public class FilterCombinationTypeConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return value is FilterCombinationType item ? FilterCombinationTypeHelper.GetNameOfFilterCombinationType(item) : null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      string strValue = value as string;
      return FilterCombinationTypeHelper.GetFilterCombinationTypeByName(strValue);
    }
  }
}
