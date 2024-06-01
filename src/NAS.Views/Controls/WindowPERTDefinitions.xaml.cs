using System.Windows;
using ES.Tools.Core.MVVM;

namespace NAS.Views.Controls
{
  /// <summary>
  /// Interaction logic for WindowPERTDefinitions.xaml
  /// </summary>
  public partial class WindowPERTDefinitions : IView
  {
    public WindowPERTDefinitions()
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
  }
}
