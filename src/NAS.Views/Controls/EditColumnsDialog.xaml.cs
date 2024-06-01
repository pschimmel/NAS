using ES.Tools.Core.MVVM;

namespace NAS.Views.Controls
{
  /// <summary>
  /// Interaction logic for EditColumnsDialog.xaml
  /// </summary>
  public partial class EditColumnsDialog : IView
  {
    public EditColumnsDialog()
    {
      InitializeComponent();
    }

    public IViewModel ViewModel
    {
      get => DataContext as IViewModel;
      set => DataContext = value;
    }
  }
}
