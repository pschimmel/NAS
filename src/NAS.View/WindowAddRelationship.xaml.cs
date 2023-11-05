using System.Windows;
using ES.Tools.Core.MVVM;
using NAS.ViewModel.Base;

namespace NAS
{
  /// <summary>
  /// Interaction logic for WindowAddRelationship.xaml
  /// </summary>
  public partial class WindowAddRelationship : IView
  {
    public WindowAddRelationship()
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
      if ((ViewModel as IValidatable).Validate())
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
