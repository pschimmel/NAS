using System.Windows.Controls;
using NAS.ViewModel.Base;

namespace NAS.View
{
  /// <summary>
  /// Interaction logic for EditCalendarsView.xaml
  /// </summary>
  public partial class EditCalendarsView : Grid, IDialogContentView
  {
    public EditCalendarsView()
    {
      InitializeComponent();
    }

    public IDialogContentViewModel ViewModel
    {
      get => DataContext as IDialogContentViewModel;
      set => DataContext = value;
    }
  }
}
