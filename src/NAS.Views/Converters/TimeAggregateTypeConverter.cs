using System.Globalization;
using System.Windows.Data;
using NAS.Models.Enums;

namespace NAS.Views.Converters
{
  [ValueConversion(typeof(TimeAggregateType), typeof(string))]
  public class TimeAggregateTypeConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return value is TimeAggregateType item ? TimeAggregateTypeHelper.GetNameOfConstraintType(item) : null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      string strValue = value as string;
      return ConstraintTypeHelper.GetConstraintTypeByName(strValue);
    }
  }
}
