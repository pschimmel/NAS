using System.Windows;

namespace NAS.View.Controls
{
  /// <summary>
  /// Interaction logic for WindowPERTGridSize.xaml
  /// </summary>
  public partial class WindowPERTGridSize : IView
  {
    public WindowPERTGridSize()
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
  }
}
