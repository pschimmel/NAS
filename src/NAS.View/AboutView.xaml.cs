using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Navigation;
using NAS.ViewModel.Base;

namespace NAS
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
