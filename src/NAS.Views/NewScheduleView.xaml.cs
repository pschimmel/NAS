using System.Windows;
using System.Windows.Controls;
using NAS.ViewModels;
using NAS.ViewModels.Base;
using Xceed.Wpf.Toolkit;

namespace NAS.Views
{
  /// <summary>
  /// Interaction logic for NewScheduleView.xaml
  /// </summary>
  public partial class NewScheduleView:IDialogContentView
  {
    public NewScheduleView()
    {
      InitializeComponent();
    }

    public IDialogContentViewModel ViewModel
    {
      get => DataContext as IDialogContentViewModel;
      set => DataContext = value;
    }

    private void Wizard_PageChanged(object sender, RoutedEventArgs e)
    {
      if ((e.OriginalSource as Wizard).CurrentPage == LastPage)
      {
        if (DataContext is NewScheduleViewModel vm)
        {
          vm.ValidateScheduleData();
        }
      }
    }
  }
}
