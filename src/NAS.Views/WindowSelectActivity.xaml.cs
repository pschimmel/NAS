using System.Windows;
using System.Windows.Input;
using ES.Tools.Core.MVVM;

namespace NAS
{
  /// <summary>
  /// Interaction logic for WindowSelectActivity.xaml
  /// </summary>
  public partial class WindowSelectActivity : IView
  {
    public WindowSelectActivity()
    {
      InitializeComponent();
    }

    public IViewModel ViewModel
    {
      get => DataContext as IViewModel;
      set => DataContext = value;
    }

    private void buttonOK_Click(object sender, RoutedEventArgs e)
    {
      DialogResult = true;
    }

    private void buttonCancel_Click(object sender, RoutedEventArgs e)
    {
      DialogResult = false;
    }

    private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      DialogResult = true;
    }
  }
}
