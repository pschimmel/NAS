using System.Windows;
using ES.Tools.Core.MVVM;
using NAS.ViewModels.Base;

namespace NAS
{
  /// <summary>
  /// Interaction logic for WindowScheduleProperties.xaml
  /// </summary>
  public partial class WindowScheduleProperties : IView
  {
    public WindowScheduleProperties()
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
      if ((DataContext as IValidatable).Validate().IsOK)
      {
        DialogResult = true;
      }
    }

    private void buttonCancel_Click(object sender, RoutedEventArgs e)
    {
      DialogResult = false;
    }
  }
}
