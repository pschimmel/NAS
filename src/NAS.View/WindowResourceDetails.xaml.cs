using System.Windows;
using System.Windows.Media.Imaging;
using ES.Tools.Core.MVVM;
using NAS.Model.Entities;
using NAS.Resources;
using NAS.ViewModel;

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
        if ((value as ResourceDetailsViewModel).CurrentResource is WorkResource)
        {
          Title = NASResources.WorkResource;
          Icon = new BitmapImage(new Uri("pack://application:,,,/NAS.View;component/Images/Resources.png"));
        }
        else if ((value as ResourceDetailsViewModel).CurrentResource is CalendarResource)
        {
          Title = NASResources.CalendarResource;
          Icon = new BitmapImage(new Uri("pack://application:,,,/NAS.View;component/Images/Calendar.png"));
        }
        else
        { // Material Resource
          Title = Globalization.NASResources.MaterialResource;
          Icon = new BitmapImage(new Uri("pack://application:,,,/NAS.View;component/Images/MaterialResource.png"));
        }
      }
    }

    private void buttonOK_Click(object sender, RoutedEventArgs e)
    {
      if ((DataContext as ResourceDetailsViewModel).Validate())
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
