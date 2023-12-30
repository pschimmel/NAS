using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;
using ES.Tools.Core.MVVM;

namespace NAS
{
  /// <summary>
  /// Interaction logic for AboutWindow.xaml
  /// </summary>
  public partial class AboutWindow : IView
  {
    public AboutWindow()
    {
      InitializeComponent();
    }

    public IViewModel ViewModel
    {
      get => DataContext as IViewModel;
      set => DataContext = value;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      DialogResult = true;
    }

    private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
      Process.Start(e.Uri.ToString());
    }
  }
}
