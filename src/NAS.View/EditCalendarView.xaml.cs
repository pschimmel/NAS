using System.Windows.Controls;
using NAS.ViewModel.Base;

namespace NAS.View
{
  /// <summary>
  /// Interaction logic for EditCalendarView.xaml
  /// </summary>
  public partial class EditCalendarView : Grid, IDialogContentView
  {
    public EditCalendarView()
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
