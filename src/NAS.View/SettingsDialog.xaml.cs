using ES.Tools.Core.MVVM;

namespace NAS.View
{
  /// <summary>
  /// Interaction logic for SettingsDialog.xaml
  /// </summary>
  public partial class SettingsDialog : IView
  {
    public SettingsDialog()
    {
      InitializeComponent();
    }

    public IViewModel ViewModel
    {
      get => DataContext as IViewModel;
      set => DataContext = value;
    }
  }
}
