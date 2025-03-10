﻿using System.IO;
using System.Windows;
using ES.Tools.Core.MVVM;
using NAS.Models.Controllers;
using NAS.ViewModels;
using NAS.ViewModels.Base;
using NAS.ViewModels.Helpers;
using NAS.Views;
using NAS.Views.Helpers;

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

      var mainWindow = new MainWindow();
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
      ViewFactory.Instance.RegisterDialog<EditPropertiesViewModel, EditPropertiesView>();
      ViewFactory.Instance.RegisterDialog<GetDateViewModel, GetDateView>();
      ViewFactory.Instance.RegisterDialog<GetTextViewModel, GetTextView>();
      ViewFactory.Instance.RegisterDialog<GetColorViewModel, GetColorView>();
      ViewFactory.Instance.RegisterDialog<SelectResourceTypeViewModel, SelectResourceTypeView>();
      ViewFactory.Instance.RegisterDialog<SelectResourceViewModel, SelectResourceView>();
      ViewFactory.Instance.RegisterDialog<SelectActivityPropertyViewModel, SelectActivityPropertyView>();
      ViewFactory.Instance.RegisterDialog<SelectActivityViewModel, SelectActivityView>();
      ViewFactory.Instance.RegisterDialog<NewScheduleViewModel, NewScheduleView>();
      ViewFactory.Instance.RegisterDialog<EditWBSViewModel, EditWBSView>();
      ViewFactory.Instance.RegisterDialog<EditWBSItemViewModel, EditWBSItemView>();
      ViewFactory.Instance.RegisterDialog<ShowWBSSummaryViewModel, ShowWBSSummaryView>();
      ViewFactory.Instance.RegisterDialog<EditSchedulingSettingsViewModel, EditSchedulingSettingsView>();
      ViewFactory.Instance.RegisterDialog<EditActivityViewModel, EditActivityView>();
      ViewFactory.Instance.RegisterDialog<EditRelationshipViewModel, EditRelationshipView>();
      ViewFactory.Instance.RegisterDialog<EditCalendarsViewModel, EditCalendarsView>();
      ViewFactory.Instance.RegisterDialog<EditCalendarViewModel, EditCalendarView>();
      ViewFactory.Instance.RegisterDialog<EditResourcesViewModel, EditResourcesView>();
      ViewFactory.Instance.RegisterDialog<EditResourceViewModel, EditResourceView>();
      ViewFactory.Instance.RegisterDialog<EditCustomAttributesViewModel, EditCustomAttributesView>();
      ViewFactory.Instance.RegisterDialog<ResourceAssignmentViewModel, ResourceAssignmentView>();
      ViewFactory.Instance.RegisterDialog<EditLayoutViewModel, EditLayoutView>();
      ViewFactory.Instance.RegisterDialog<SelectBaselineViewModel, SelectBaselineView>();
      ViewFactory.Instance.RegisterDialog<EditPrintLayoutViewModel, EditPrintLayoutView>();
      ViewFactory.Instance.RegisterDialog<EditSortingAndGroupingViewModel, EditSortingAndGroupingView>();
      ViewFactory.Instance.RegisterDialog<SelectSortingDefinitionViewModel, SelectSortingDefinitionView>();
      ViewFactory.Instance.RegisterDialog<SelectGroupingDefinitionViewModel, SelectGroupingDefinitionView>();
      ViewFactory.Instance.RegisterDialog<EditFiltersViewModel, EditFiltersView>();
      ViewFactory.Instance.RegisterDialog<SelectFilterDefinitionViewModel, SelectFilterDefinitionView>();
      ViewFactory.Instance.RegisterDialog<EditBaselinesViewModel, EditBaselinesView>();
      ViewFactory.Instance.RegisterDialog<EditFragnetsViewModel, EditFragnetsView>();
      ViewFactory.Instance.RegisterDialog<EditFragnetViewModel, EditFragnetView>();
      ViewFactory.Instance.RegisterDialog<ShowFragnetViewModel, ShowFragnetView>();
      ViewFactory.Instance.RegisterDialog<EditDistortionsViewModel, EditDistortionsView>();
      ViewFactory.Instance.RegisterDialog<EditDistortionViewModel, EditDistortionView>();
      ViewFactory.Instance.RegisterDialog<AddDistortionViewModel, AddDistortionView>();
      ViewFactory.Instance.RegisterDialog<CompareResultsViewModel, CompareResultsView>();
      ViewFactory.Instance.RegisterDialog<CompareSchedulesViewModel, CompareSchedulesView>();
      ViewFactory.Instance.RegisterDialog<EditLogicViewModel, EditLogicView>();
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
