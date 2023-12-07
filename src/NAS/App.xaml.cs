using System.Windows;
using System.Windows.Navigation;
using NAS.View;
using NAS.View.Helpers;
using NAS.ViewModel.Helpers;

namespace NAS
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    protected override void OnStartup(StartupEventArgs e)
    {
      base.OnStartup(e);

      var splashScreen = new SplashScreen("Images/Splash.png");
      splashScreen.Show(true);

      var mainWindow  = new MainWindow();
      mainWindow.Show();
    }

    protected override void OnLoadCompleted(NavigationEventArgs e)
    {
      base.OnLoadCompleted(e);

      UserNotificationService.Instance.RegisterTarget(new MessageBoxNotificationTarget());
    }
  }
}
