﻿using System.IO;
using System.Windows;
using ES.Tools.Core.MVVM;
using NAS.Model.Controllers;
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
      ViewFactory.Instance.Register<DialogViewModel, DialogWindow>();

      // Dialogs
      ViewFactory.Instance.RegisterDialog<AboutViewModel, AboutView>();
      ViewFactory.Instance.RegisterDialog<SettingsViewModel, SettingsView>();
      ViewFactory.Instance.RegisterDialog<NewScheduleViewModel, NewScheduleView>();
      ViewFactory.Instance.RegisterDialog<EditActivityViewModel, EditActivityView>();
      ViewFactory.Instance.RegisterDialog<EditCalendarsViewModel, EditCalendarsView>();
      ViewFactory.Instance.RegisterDialog<EditCalendarViewModel, EditCalendarView>();
      ViewFactory.Instance.RegisterDialog<GetDateViewModel, GetDateView>();
    }

    private static void RegisterNotificationTargets()
    {
      UserNotificationService.Instance.RegisterTarget(new MessageBoxNotificationTarget());
      GlobalDataController.Instance.Error += GlobalDataController_Error;
    }

    private static void GlobalDataController_Error(object sender, ErrorEventArgs e)
    {
      UserNotificationService.Instance.Error(e.GetException().Message);
    }
  }
}
