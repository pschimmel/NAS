using System.Windows.Controls;
using NAS.ViewModels.Base;

namespace NAS.Views
{
  /// <summary>
  /// Interaction logic for SelectSortingDefinitionView.xaml
  /// </summary>
  public partial class SelectSortingDefinitionView:IDialogContentView
  {
    public SelectSortingDefinitionView()
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
