using System.Globalization;
using System.Windows.Data;
using NAS.Model.Enums;

namespace NAS.View.Converters
{
  [ValueConversion(typeof(ConstraintType), typeof(string))]
  public class ConstraintTypeConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return value is ConstraintType item ? ConstraintTypeHelper.GetNameOfConstraintType(item) : null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      string strValue = value as string;
      return ConstraintTypeHelper.GetConstraintTypeByName(strValue);
    }
  }
}
