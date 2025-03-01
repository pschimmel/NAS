using NAS.ViewModels.Base;

namespace NAS.Views
{
  /// <summary>
  /// Interaction logic for EditFragnetView.xaml
  /// </summary>
  public partial class EditFragnetView : IDialogContentView
  {
    public EditFragnetView()
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
