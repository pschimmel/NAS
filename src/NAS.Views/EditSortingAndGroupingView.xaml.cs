using System.Windows.Controls;
using NAS.ViewModels.Base;

namespace NAS.Views
{
  /// <summary>
  /// Interaction logic for EditSortingAndGroupingView.xaml
  /// </summary>
  public partial class EditSortingAndGroupingView:IDialogContentView
  {
    public EditSortingAndGroupingView()
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
