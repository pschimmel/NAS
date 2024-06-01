using System.Diagnostics;
using System.Windows;
using System.Windows.Media.Imaging;
using ES.Tools.Core.MVVM;
using NAS.Resources;
using NAS.ViewModels;
using NAS.ViewModels.Base;

namespace NAS
{
  /// <summary>
  /// Interaction logic for WindowResourceDetails.xaml
  /// </summary>
  public partial class WindowResourceDetails : IView
  {
    public WindowResourceDetails()
    {
      InitializeComponent();
    }

    public IViewModel ViewModel
    {
      get => DataContext as IViewModel;
      set
      {
        DataContext = value;
        if ((value as ResourceDetailsViewModel).IsWorkResource)
        {
          Title = NASResources.WorkResource;
          Icon = new BitmapImage(new Uri("pack://application:,,,/NAS.Views;component/Images/Resources.png"));
        }
        else if ((value as ResourceDetailsViewModel).IsCalendarResource)
        {
          Title = NASResources.CalendarResource;
          Icon = new BitmapImage(new Uri("pack://application:,,,/NAS.Views;component/Images/Calendar.png"));
        }
        else
        {
          Debug.Assert((value as ResourceDetailsViewModel).IsMaterialResource);
          Title = NASResources.MaterialResource;
          Icon = new BitmapImage(new Uri("pack://application:,,,/NAS.Views;component/Images/MaterialResource.png"));
        }
      }
    }

    private void buttonOK_Click(object sender, RoutedEventArgs e)
    {
      if ((DataContext as IValidatable).Validate().IsOK)
      {
        DialogResult = true;
      }
    }

    private void buttonCancel_Click(object sender, RoutedEventArgs e)
    {
      DialogResult = false;
    }
  }
}
