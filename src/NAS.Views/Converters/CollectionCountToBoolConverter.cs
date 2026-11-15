using System;
using System.Collections;
using System.Globalization;
using System.Windows.Data;

namespace NAS.Views.Converters
{
  [ValueConversion(typeof(object), typeof(bool))]
  public class CollectionCountToBoolConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null)
      {
        return false;
      }

      if (value is ICollection collection)
      {
        return collection.Count > 0;
      }

      return value is string s && s.Length > 0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return Binding.DoNothing;
    }
  }
}
