using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Navigation;
using NAS.ViewModels.Base;

namespace NAS.Views
{
  /// <summary>
  /// Interaction logic for AboutView.xaml
  /// </summary>
  public partial class AboutView : Grid, IDialogContentView
  {
    public AboutView()
    {
      InitializeComponent();
    }

    public IDialogContentViewModel ViewModel
    {
      get => DataContext as IDialogContentViewModel;
      set => DataContext = value;
    }

    private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
      Process.Start(e.Uri.ToString());
    }
  }
}
