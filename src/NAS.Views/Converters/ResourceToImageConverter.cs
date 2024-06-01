using System.Globalization;
using System.Windows;
using System.Windows.Data;
using NAS.Models.Entities;

namespace NAS.Views.Converters
{
  [ValueConversion(typeof(Resource), typeof(Uri))]
  public class ResourceToImageConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is Resource item)
      {
        switch (item)
        {
          case MaterialResource _:
            return new Uri("pack://application:,,,/NAS.Views;component/Images/MaterialResource.png", UriKind.Absolute);
          case WorkResource _:
            return new Uri("pack://application:,,,/NAS.Views;component/Images/Resources.png", UriKind.Absolute);
          case CalendarResource _:
            return new Uri("pack://application:,,,/NAS.Views;component/Images/Calendar.png", UriKind.Absolute);
        }
      }

      return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return DependencyProperty.UnsetValue;
    }
  }
}
