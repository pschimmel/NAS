using System.Windows.Controls;
using NAS.ViewModels.Base;

namespace NAS.Views
{
  /// <summary>
  /// Interaction logic for SelectFilterDefinitionView.xaml
  /// </summary>
  public partial class SelectFilterDefinitionView : Grid, IDialogContentView
  {
    public SelectFilterDefinitionView()
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
