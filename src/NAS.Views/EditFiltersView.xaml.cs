using System.Windows.Controls;
using NAS.ViewModels.Base;

namespace NAS.Views
{
  /// <summary>
  /// Interaction logic for EditFiltersView.xaml
  /// </summary>
  public partial class EditFiltersView : Grid, IDialogContentView
  {
    public EditFiltersView()
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
