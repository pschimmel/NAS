using NAS.ViewModels.Base;

namespace NAS.Views
{
  /// <summary>
  /// Interaction logic for EditDistortionView.xaml
  /// </summary>
  public partial class EditDistortionView : IDialogContentView
  {
    public EditDistortionView()
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
