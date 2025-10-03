using ES.Tools.Core.MVVM;
using NAS.ViewModels.Base;

namespace NAS.Views.Controls
{
  /// <summary>
  /// Interaction logic for EditColumnsView.xaml
  /// </summary>
  public partial class EditColumnsView : IDialogContentView
  {
    public EditColumnsView()
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
