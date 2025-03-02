using NAS.ViewModels.Base;

namespace NAS.Views
{
  /// <summary>
  /// Interaction logic for GetColorView.xaml
  /// </summary>
  public partial class GetColorView : IDialogContentView
  {
    public GetColorView()
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
