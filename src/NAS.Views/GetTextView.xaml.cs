using NAS.ViewModels.Base;

namespace NAS.Views
{
  /// <summary>
  /// Interaction logic for GetTextView.xaml
  /// </summary>
  public partial class GetTextView : IDialogContentView
  {
    public GetTextView()
    {
      InitializeComponent();
      Loaded += GetTextView_Loaded;
    }

    private void GetTextView_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
      Loaded -= GetTextView_Loaded;
      textBoxText.Focus();
    }

    public IDialogContentViewModel ViewModel
    {
      get => DataContext as IDialogContentViewModel;
      set => DataContext = value;
    }
  }
}
