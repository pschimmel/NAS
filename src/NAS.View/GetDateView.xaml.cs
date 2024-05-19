using NAS.ViewModel.Base;

namespace NAS.View
{
  /// <summary>
  /// Interaction logic for GetDateView.xaml
  /// </summary>
  public partial class GetDateView : IDialogContentView
  {
    public GetDateView()
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
