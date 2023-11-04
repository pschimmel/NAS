using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using AvalonDock;
using Fluent;
using NAS.Resources;
using NAS.View.Controls;
using NAS.ViewModel;
using NAS.ViewModel.Helpers;

namespace NAS.View
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow
  {
    #region Fields

    private WindowProgress windowProgress;
    private StartupSettings _startupSettings;

    #endregion

    #region Constructor

    public MainWindow()
    {
      Loaded += MainWindow_Loaded;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
      // WPF Bug Workaround: while we have no WPF window open we can`t show MessageBox.
      var dummyWindow = new Window { AllowsTransparency = true, Background = Brushes.Transparent, WindowStyle = WindowStyle.None, Width = 1, Height = 1, ShowInTaskbar = false, WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen };
      dummyWindow.Show();

      MainViewModel viewModel = null;
      try
      {
        viewModel = new MainViewModel();
#if !DEBUG
        if (CurrentInfoHolder.Settings.AutoCheckForUpdates)
        {
          viewModel.CheckForUpdatesCommand.Execute(false);
        }
#endif
        viewModel.TestConnection();
        if (MainViewModel.ShutdownRequested)
        {
          return;
        }

        _startupSettings = CommandLineParser.GetStartupSettings();
        viewModel.CheckReportsImport();

        if (!string.IsNullOrWhiteSpace(_startupSettings.UserName) && !string.IsNullOrWhiteSpace(_startupSettings.Password))
        {
          viewModel.Login(_startupSettings.UserName, _startupSettings.Password);
        }
        else
        {
          viewModel.Login();
        }
      }
      catch (Exception ex)
      {
        ES.WPF.Toolkit.MessageBox.Show(ex.Message, NASResources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
        Application.Current.Shutdown();
      }

      if (viewModel != null && !MainViewModel.ShutdownRequested)
      {
        DataContext = viewModel;
        viewModel.DownloadProgress += ViewModel_DownloadProgress;
        viewModel.RequestThemeChange += (s, args) => Helpers.ThemeManager.SetTheme(args.Item, dockingManager);
        viewModel.RequestSelectFirstTab += (s, args) => Ribbon.Tabs.First().IsSelected = true;
        viewModel.GetCanvas += (s, args) =>
        {
          args.Item = args.Info switch
          {
            LayoutType.Gantt => new StandaloneGanttCanvas(),
            LayoutType.PERT => new StandalonePERTCanvas(),
            _ => throw new ArgumentException($"Unknown layout type {args.Info}"),
          };
        };
        InitializeComponent();
        Helpers.ThemeManager.SetTheme(CurrentInfoHolder.ProgramSettings.Theme, dockingManager);

        Dispatcher.BeginInvoke(new Action(() =>
        {
          ActivityContextMenu.IsVisibleChanged += tabGroup_IsVisibleChanged;
          RelationshipContextMenu.IsVisibleChanged += tabGroup_IsVisibleChanged;
          Ribbon.Tabs.First().IsSelected = true;
        }), DispatcherPriority.Normal);

        if (!string.IsNullOrWhiteSpace(_startupSettings.ScheduleToOpen))
        {
          viewModel.OpenSchedule(_startupSettings.ScheduleToOpen, true);
        }
        else if (_startupSettings.ImportFileName?.Exists ?? false && viewModel.ScheduleManagerViewModel.ImportScheduleCommand.CanExecute(_startupSettings?.ImportFileName.FullName))
        {
          viewModel.ScheduleManagerViewModel.ImportScheduleCommand.Execute(_startupSettings?.ImportFileName.FullName);
        }

        Ribbon.AreTabHeadersVisible = true;
      }
      else
      {
        MainViewModel.ShutdownRequested = true;
        Application.Current.Shutdown();
      }

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
          scheduleViewModel.UnlockSchedule();
          scheduleViewModel.SaveChanges();
        }
      }

      (DataContext as MainViewModel)?.Dispose();
    }

    #endregion

    #region Commands

    public RoutedCommand CalculateCommand { get; }

    #endregion

    #region Ribbon Menu Tabs

    private void tabGroup_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if (sender is RibbonTabItem && (sender as RibbonTabItem).Group != null && (bool)e.NewValue == true)
      {
        (sender as RibbonTabItem).IsSelected = true;
      }
    }

    #endregion

    #region Avalondock

    private void dockManager_DocumentClosed(object sender, DocumentClosedEventArgs e)
    {
      var scheduleViewModel = e.Document.Content as ScheduleViewModel;
      (DataContext as MainViewModel).Schedules.Remove(scheduleViewModel);
    }

    private void dockingManager_DocumentClosing(object sender, DocumentClosingEventArgs e)
    {
      var scheduleViewModel = e.Document.Content as ScheduleViewModel;
      scheduleViewModel.UnlockSchedule();
      scheduleViewModel.SaveChanges();
    }

    #endregion

    #region Progress

    private void ViewModel_DownloadProgress(object sender, ProgressEventArgs e)
    {
      if (e == ProgressEventArgs.Starting)
      {
        windowProgress = new WindowProgress(0, NASResources.Downloading + "...");
        windowProgress.Show();
      }
      else if (e == ProgressEventArgs.Finished)
      {
        windowProgress.Close();
        windowProgress = null;
      }
      else
      {
        if (windowProgress != null)
        {
          windowProgress.SetProgress(e.CurrentProgress);
        }
      }
    }

    #endregion
  }
}
