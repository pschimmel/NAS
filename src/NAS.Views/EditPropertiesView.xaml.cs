using System.Windows.Controls;
using NAS.ViewModels.Base;

namespace NAS
{
  /// <summary>
  /// Interaction logic for EditPropertiesView.xaml
  /// </summary>
  public partial class EditPropertiesView : Grid, IDialogContentView
  {
    public EditPropertiesView()
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
