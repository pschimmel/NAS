using System.Windows;
using System.Windows.Controls;
using NAS.ViewModels.Base;

namespace NAS.Views
{
  /// <summary>
  /// Interaction logic for EditPrintLayoutView.xaml
  /// </summary>
  public partial class EditPrintLayoutView : Grid, IDialogContentView
  {
    public EditPrintLayoutView()
    {
      InitializeComponent();
    }

    public IDialogContentViewModel ViewModel
    {
      get => DataContext as IDialogContentViewModel;
      set => DataContext = value;
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
