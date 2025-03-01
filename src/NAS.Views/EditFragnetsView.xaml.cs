using NAS.ViewModels.Base;

namespace NAS.Views
{
  /// <summary>
  /// Interaction logic for EditFragnetsView.xaml
  /// </summary>
  public partial class EditFragnetsView : IDialogContentView
  {
    public EditFragnetsView()
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
