using System.Globalization;
using System.Windows.Data;
using NAS.Models.Enums;

namespace NAS.Views.Converters
{
  [ValueConversion(typeof(SortDirection), typeof(string))]
  public class SortDirectionConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return value is SortDirection item ? SortDirectionHelper.GetNameOfSortDirection(item) : null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      string strValue = value as string;
      return SortDirectionHelper.GetSortDirectionByName(strValue);
    }
  }
}
