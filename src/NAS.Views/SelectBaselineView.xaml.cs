using System.Windows.Controls;
using NAS.ViewModels.Base;

namespace NAS.Views
{
  /// <summary>
  /// Interaction logic for SelectBaselineView.xaml
  /// </summary>
  public partial class SelectBaselineView : Grid, IDialogContentView
  {
    public SelectBaselineView()
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
