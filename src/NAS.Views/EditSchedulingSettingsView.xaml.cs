using System.Windows.Controls;
using NAS.ViewModels.Base;

namespace NAS.Views
{
  /// <summary>
  /// Interaction logic for EditSchedulingSettingsView.xaml
  /// </summary>
  public partial class EditSchedulingSettingsView:IDialogContentView
  {
    public EditSchedulingSettingsView()
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
