using System.Windows.Controls;
using NAS.ViewModel.Base;

namespace NAS
{
  /// <summary>
  /// Interaction logic for EditActivityView.xaml
  /// </summary>
  public partial class EditActivityView : Grid, IDialogContentView
  {
    public EditActivityView()
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
