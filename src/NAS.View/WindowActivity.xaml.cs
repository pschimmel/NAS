using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ES.Tools.Core.MVVM;
using NAS.ViewModel.Base;

namespace NAS
{
  /// <summary>
  /// Interaction logic for WindowActivity.xaml
  /// </summary>
  public partial class WindowActivity : IView
  {
    public WindowActivity()
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
      if ((DataContext as IValidatable).Validate())
      {
        DialogResult = true;
      }
    }

    private void buttonCancel_Click(object sender, RoutedEventArgs e)
    {
      DialogResult = false;
    }

    private void comboBox_KeyDown(object sender, KeyEventArgs e)
    {
      if (sender is ComboBox && e.Key == Key.Delete)
      {
        (sender as ComboBox).SelectedItem = null;
      }
    }
  }
}
