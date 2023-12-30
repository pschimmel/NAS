using System.Windows;
using ES.Tools.Core.MVVM;
using NAS.View;
using NAS.View.Helpers;
using NAS.ViewModel;
using NAS.ViewModel.Base;
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
      RegisterViews();
      RegisterNotificationTargets();
      mainWindow.Show();
    }

    private static void RegisterViews()
    {
      // Windows
      ViewFactory.Instance.Register<AboutViewModel, AboutWindow>();
      ViewFactory.Instance.Register<DialogViewModel, DialogWindow>();

      // Dialogs
      ViewFactory.Instance.RegisterDialog<SettingsViewModel, SettingsView>();
    }

    private static void RegisterNotificationTargets()
    {
      UserNotificationService.Instance.RegisterTarget(new MessageBoxNotificationTarget());
    }
  }
}
