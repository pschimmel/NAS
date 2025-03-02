using NAS.ViewModels.Base;

namespace NAS.Views
{
  /// <summary>
  /// Interaction logic for SelectActivityPropertyView.xaml
  /// </summary>
  public partial class SelectActivityPropertyView : IDialogContentView
  {
    public SelectActivityPropertyView()
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
