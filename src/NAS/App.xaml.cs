using System.Windows;
using System.Windows.Navigation;
using NAS.View.Helpers;
using NAS.ViewModel.Helpers;

namespace NAS
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    protected override void OnLoadCompleted(NavigationEventArgs e)
    {
      base.OnLoadCompleted(e);

      UserNotificationService.Instance.RegisterTarget(new MessageBoxNotificationTarget());
    }
  }
}
