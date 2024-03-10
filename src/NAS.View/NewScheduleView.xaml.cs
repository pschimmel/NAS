using System.Windows;
using System.Windows.Controls;
using NAS.ViewModel;
using NAS.ViewModel.Base;
using Xceed.Wpf.Toolkit;

namespace NAS.View
{
  /// <summary>
  /// Interaction logic for NewScheduleView.xaml
  /// </summary>
  public partial class NewScheduleView : Grid, IDialogContentView
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
