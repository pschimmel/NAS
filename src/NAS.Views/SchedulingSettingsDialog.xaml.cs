using ES.Tools.Core.MVVM;

namespace NAS.Views
{
  /// <summary>
  /// Interaction logic for SchedulingSettingsDialog.xaml
  /// </summary>
  public partial class SchedulingSettingsDialog : IView
  {
    public SchedulingSettingsDialog()
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
