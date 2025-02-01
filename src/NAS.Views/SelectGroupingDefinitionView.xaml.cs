using System.Windows.Controls;
using NAS.ViewModels.Base;

namespace NAS.Views
{
  /// <summary>
  /// Interaction logic for SelectGroupingDefinitionView.xaml
  /// </summary>
  public partial class SelectGroupingDefinitionView : Grid, IDialogContentView
  {
    public SelectGroupingDefinitionView()
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
