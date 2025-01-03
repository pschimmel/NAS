using NAS.ViewModels.Base;

namespace NAS.Views
{
  /// <summary>
  /// Interaction logic for ShowWBSSummaryView.xaml
  /// </summary>
  public partial class ShowWBSSummaryView : IDialogContentView
  {
    public ShowWBSSummaryView()
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
