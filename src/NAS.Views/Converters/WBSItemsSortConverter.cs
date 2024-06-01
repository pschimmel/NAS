using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;
using NAS.Models.Entities;
using NAS.ViewModels;

namespace NAS.Views.Converters
{
  [ValueConversion(typeof(IEnumerable<WBSItem>), typeof(IEnumerable<WBSItem>))]
  public class WBSItemsSortConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return ((ObservableCollection<WBSItemViewModel>)value).OrderBy(o => o.Order);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return ((ObservableCollection<WBSItemViewModel>)value).OrderBy(o => o.Order);
    }
  }
}
