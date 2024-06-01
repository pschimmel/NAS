using System.Globalization;
using System.Windows.Data;
using NAS.Models.Enums;

namespace NAS.Views.Converters
{
  [ValueConversion(typeof(FilterRelation), typeof(string))]
  public class FilterRelationConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return value is FilterRelation item ? FilterRelationHelper.GetNameOfFilterRelation(item) : null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      string strValue = value as string;
      return FilterRelationHelper.GetFilterRelationByName(strValue);
    }
  }
}
