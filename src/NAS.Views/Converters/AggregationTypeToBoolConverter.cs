using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;
using NAS.Models.Enums;

namespace NAS.Views.Converters
{
  public class AggregationTypeToBoolConverter : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      Debug.Assert(values.Length == 2);
      Debug.Assert(values[0] is TimeAggregateType);
      Debug.Assert(values[1] is TimeAggregateType);

      return (TimeAggregateType)values[0] == (TimeAggregateType)values[1];
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
