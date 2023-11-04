using System.Windows;
using ES.Tools.Core.MVVM;

namespace NAS.ReportViewer
{
  /// <summary>
  /// Interaction logic for WindowPageSettings.xaml
  /// </summary>
  public partial class WindowPageSettings : IView
  {
    public WindowPageSettings()
    {
      InitializeComponent();
    }

    public IViewModel ViewModel
    {
      get => DataContext as IViewModel;
      set => DataContext = value;
    }

    private void buttonCancel_Click(object sender, RoutedEventArgs e)
    {
      DialogResult = false;
    }

    private void buttonOK_Click(object sender, RoutedEventArgs e)
    {
      DialogResult = true;
    }
  }
}
