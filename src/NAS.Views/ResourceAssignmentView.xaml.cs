using System.Windows.Controls;
using NAS.ViewModels.Base;

namespace NAS.Views
{
  /// <summary>
  /// Interaction logic for ResourceAssignmentView.xaml
  /// </summary>
  public partial class ResourceAssignmentView:IDialogContentView
  {
    public ResourceAssignmentView()
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
