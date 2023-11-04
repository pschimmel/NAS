using System.Windows.Controls;
using ES.Tools.Core.MVVM;

namespace NAS.View
{
  /// <summary>
  /// Interaction logic for SchedulingSettingsDialog.xaml
  /// </summary>
  public partial class SchedulingSettingsDialog : UserControl, IDialog
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
