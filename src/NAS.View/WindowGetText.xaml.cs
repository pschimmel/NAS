using System.Windows;
using ES.Tools.Core.MVVM;

namespace NAS
{
  /// <summary>
  /// Interaction logic for WindowGetText.xaml
  /// </summary>
  public partial class WindowGetText : IView
  {
    public WindowGetText()
    {
      InitializeComponent();
      textBoxText.Focus();
    }

    public IViewModel ViewModel
    {
      get => DataContext as IViewModel;
      set => DataContext = value;
    }

    private void buttonOK_Click(object sender, RoutedEventArgs e)
    {
      if ((ViewModel as IValidatingViewModel).Validate())
      {
        DialogResult = true;
      }
    }

    private void ButtonCancel_Click(object sender, RoutedEventArgs e)
    {
      DialogResult = false;
    }
  }
}
