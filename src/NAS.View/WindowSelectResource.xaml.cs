using System.Windows;
using System.Windows.Input;
using ES.Tools.Core.MVVM;
using NAS.ViewModel;

namespace NAS
{
  /// <summary>
  /// Interaction logic for WindowSelectResource.xaml
  /// </summary>
  public partial class WindowSelectResource : IView
  {
    public WindowSelectResource()
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

    private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      if (e.ClickCount == 2)
      {
        if ((DataContext as SelectResourceViewModel).SelectedResource != null)
        {
          DialogResult = true;
        }
      }
    }
  }
}
