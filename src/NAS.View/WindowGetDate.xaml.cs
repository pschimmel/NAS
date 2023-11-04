using System.Windows;
using System.Windows.Input;
using ES.Tools.Core.MVVM;

namespace NAS
{
  /// <summary>
  /// Interaction logic for WindowGetDate.xaml
  /// </summary>
  public partial class WindowGetDate : IView
  {
    public WindowGetDate()
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

    private void calendar_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      DialogResult = true;
    }
  }
}
