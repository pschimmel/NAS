﻿using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using AvalonDock;
using Fluent;
using NAS.Models.Enums;
using NAS.Models.Settings;
using NAS.Resources;
using NAS.ViewModels;
using NAS.Views.Controls;

namespace NAS.Views
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow
  {
    #region Constructor

    public MainWindow()
    {
      Loaded += MainWindow_Loaded;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
      // WPF Bug Workaround: while we have no WPF window open we can`t show MessageBox.
      var dummyWindow = new Window { AllowsTransparency = true, Background = Brushes.Transparent, WindowStyle = WindowStyle.None, Width = 1, Height = 1, ShowInTaskbar = false, WindowStartupLocation = WindowStartupLocation.CenterScreen };
      dummyWindow.Show();

      MainViewModel viewModel = null;
      try
      {
        viewModel = new MainViewModel();
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message, NASResources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
        Application.Current.Shutdown();
      }

      DataContext = viewModel;
      viewModel.OnThemeChangeRequested += (s, args) => Helpers.ThemeManager.SetTheme(args.Item, dockingManager);
      viewModel.ActivateRibbonStartTab += (_, __) => Ribbon.SelectedTabItem = StartTab;

      viewModel.GetCanvas += (s, args) =>
      {
        args.ReturnedItem = args.Item switch
        {
          LayoutType.Gantt => new StandaloneGanttCanvas(),
          LayoutType.PERT => new StandalonePERTCanvas(),
          _ => throw new ArgumentException($"Unknown layout type {args.Item}"),
        };
      };
      InitializeComponent();
      Helpers.ThemeManager.SetTheme(SettingsController.Settings.Theme, dockingManager);

      Dispatcher.BeginInvoke(new Action(() =>
      {
        ActivityContextMenu.IsVisibleChanged += TabGroup_IsVisibleChanged;
        RelationshipContextMenu.IsVisibleChanged += TabGroup_IsVisibleChanged;
      }), DispatcherPriority.Normal);

      dummyWindow.Close();
    }

    #endregion

    #region Startup / Shutdown

    private void RibbonWindow_Closing(object sender, CancelEventArgs e)
    {
      if (DataContext is MainViewModel viewModel)
      {
        foreach (var scheduleViewModel in viewModel.Schedules)
        {
          //scheduleViewModel.UnlockSchedule();
          //scheduleViewModel.SaveChanges();
        }
      }

      (DataContext as MainViewModel)?.Dispose();
    }

    #endregion

    #region Commands

    public RoutedCommand CalculateCommand { get; }

    #endregion

    #region Ribbon Menu Tabs

    private void TabGroup_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if (sender is RibbonTabItem && (sender as RibbonTabItem).Group != null && (bool)e.NewValue == true)
      {
        (sender as RibbonTabItem).IsSelected = true;
      }
    }

    #endregion

    #region Avalon Dock

    private void DockManager_DocumentClosed(object sender, DocumentClosedEventArgs e)
    {
      var scheduleViewModel = e.Document.Content as ScheduleViewModel;
      (DataContext as MainViewModel).Schedules.Remove(scheduleViewModel);
    }

    #endregion
  }
}
