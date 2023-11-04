using System.Globalization;
using System.Windows.Data;
using NAS.Model.Enums;

namespace NAS.View.Converters
{
  [ValueConversion(typeof(RelationshipType), typeof(string))]
  public class RelationshipTypeConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return value is RelationshipType item ? RelationshipTypeHelper.GetNameOfRelationshipType(item) : null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      string strValue = value as string;
      return RelationshipTypeHelper.GetRelationshipTypeByName(strValue);
    }
  }
}
