using NAS.ViewModels.Base;

namespace NAS.Views.ReportViewer
{
  /// <summary>
  /// Interaction logic for PageSettingsView.xaml
  /// </summary>
  public partial class PageSettingsView : IDialogContentView
  {
    public PageSettingsView()
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
