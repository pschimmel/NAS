using System.Windows;
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

    public IDialogContentViewModel ViewModel
    {
      get => DataContext as IDialogContentViewModel;
      set => DataContext = value;
    }

    private void GetTextView_Loaded(object sender, RoutedEventArgs e)
    {
      Loaded -= GetTextView_Loaded;
      textBoxText.Focus();
      // Move caret to end of text
      textBoxText.CaretIndex = textBoxText.Text.Length;
    }
  }
}
