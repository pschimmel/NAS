using System.Windows.Controls;
using NAS.ViewModel.Base;

namespace NAS.View
{
  /// <summary>
  /// Interaction logic for SettingsView.xaml
  /// </summary>
  public partial class SettingsView : Grid, IDialogContentView
  {
    public SettingsView()
    {
      InitializeComponent();
    }

    public IDialogContentViewModel ViewModel
    {
      get => DataContext as IDialogContentViewModel;
      set => DataContext = value;
    }
  }
}
