using System.Windows;
using System.Windows.Controls;
using ES.Tools.Core.MVVM;

namespace NAS
{
  /// <summary>
  /// Interaction logic for WindowLayoutTemplate.xaml
  /// </summary>
  public partial class WindowPrintLayout : IView
  {
    public WindowPrintLayout()
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

    private void MenuItem_Click(object sender, RoutedEventArgs e)
    {
      if (sender != null && sender is MenuItem && (sender as MenuItem).Parent != null && (sender as MenuItem).Parent is ContextMenu)
      {
        var menu = (sender as MenuItem).Parent as ContextMenu;
        if (menu.PlacementTarget is Xceed.Wpf.Toolkit.RichTextBox textBox)
        {
          textBox.Selection.Text = (sender as MenuItem).Header.ToString();
        }
      }
    }
  }
}
