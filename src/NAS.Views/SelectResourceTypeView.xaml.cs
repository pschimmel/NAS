using System.Windows.Controls;
using NAS.ViewModels.Base;

namespace NAS.Views
{
  /// <summary>
  /// Interaction logic for SelectResourceTypeView.xaml
  /// </summary>
  public partial class SelectResourceTypeView:IDialogContentView
  {
    public SelectResourceTypeView()
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
