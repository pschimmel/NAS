using System.Windows.Controls;
using NAS.ViewModels.Base;

namespace NAS.Views
{
  /// <summary>
  /// Interaction logic for ChooseResourceTypeView.xaml
  /// </summary>
  public partial class ChooseResourceTypeView : Grid, IDialogContentView
  {
    public ChooseResourceTypeView()
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
