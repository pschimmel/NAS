using System.Windows.Controls;
using NAS.ViewModels.Base;

namespace NAS.Views
{
  /// <summary>
  /// Interaction logic for EditWBSView.xaml
  /// </summary>
  public partial class EditWBSView : Grid, IDialogContentView
  {
    public EditWBSView()
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
