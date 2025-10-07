using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using ES.Tools.Core.Infrastructure;
using ES.Tools.Core.MVVM;
using ES.Tools.UI;
using Microsoft.Win32;
using NAS.Models;
using NAS.Models.Base;
using NAS.Models.Controllers;
using NAS.Models.Entities;
using NAS.Models.Enums;
using NAS.Models.ImportExport;
using NAS.Models.Settings;
using NAS.Resources;
using NAS.ViewModels.Base;
using NAS.ViewModels.Helpers;
using NAS.ViewModels.Printing;

namespace NAS.ViewModels
{
  public class MainViewModel : ViewModelBase
  {
    #region Events

    public event EventHandler<ItemEventArgs<Theme>> OnThemeChangeRequested;
    public event EventHandler<RequestItemEventArgs<LayoutType, IPrintableCanvas>> GetCanvas;
    public event EventHandler ActivateRibbonStartTab;

    #endregion

    #region Fields

    private int _busyCount = 0;
    private readonly Lock _busyLock = new();
    private bool _backstageOpen = false;
    private readonly CommandLineSettings _startupSettings;
    private ObservableCollection<ScheduleViewModel> _schedules;
    private ScheduleViewModel _currentSchedule;
    private ReportViewModel _selectedReport;
    private readonly Lazy<OpenFileDialog> _lazyOpenFileDialog = new(GetOpenFileDialog);
    private readonly Lazy<SaveFileDialog> _lazySaveFileDialog = new(GetSaveFileDialog);
    private readonly Lazy<OpenFileDialog> _lazyImportFileDialog = new(GetImportFileDialog);
    private readonly Lazy<SaveFileDialog> _lazyExportFileDialog = new(GetExportFileDialog);
    private ActionCommand _createNewScheduleCommand;
    private ActionCommand<string> _openScheduleCommand;
    private ActionCommand _closeScheduleCommand;
    private ActionCommand _saveScheduleCommand;
    private ActionCommand _saveScheduleAsCommand;
    private ActionCommand _importScheduleCommand;
    private ActionCommand _exportScheduleAsCommand;
    private ActionCommand _printCommand;
    private ActionCommand _closeCommand;
    private ActionCommand _openProgramSettingsCommand;
    private ActionCommand _aboutCommand;
    private ActionCommand _websiteCommand;
    private ActionCommand _instantHelpCommand;
    private ActionCommand _showReportCommand;
    private ActionCommand _editReportCommand;
    private ActionCommand _copyReportCommand;
    private ActionCommand _deleteReportCommand;
    private ActionCommand _renameReportCommand;

    #endregion

    #region Constructor

    public MainViewModel()
    {
      _startupSettings = CommandLineParser.GetStartupSettings();

      if (SettingsController.Settings.ShowInstantHelpOnStartUp)
      {
        InstantHelp();
      }

      InstantHelpManager.Instance.HelpWindowsChanged += (_, __) => OnPropertyChanged(nameof(InstantHelpVisible));

      ReportsController.LoadReports().ForEach(x => Reports.Add(new ReportViewModel(x)));

      if (_startupSettings.Get(CommandLineSettings.CommandLineSettingsType.OpenFile, out string path))
      {
        DispatcherWrapper.Default.BeginInvokeIfRequired(() =>
        {
          OpenScheduleCommand.Execute(path);
        });
      }
    }

    #endregion

    #region Properties

    public bool IsBusy { get; private set; } = false;

    public string ProgressMessage { get; private set; } = NASResources.PleaseWait;

    public bool BackstageOpen
    {
      get => _backstageOpen;
      set
      {
        if (_backstageOpen != value)
        {
          _backstageOpen = value;
          OnPropertyChanged(nameof(BackstageOpen));
        }
      }
    }

    public string Version => Globals.Version.ToString(3);

    /// <summary>
    /// Gets the window title.
    /// </summary>
    public string WindowTitle
    {
      get
      {
        string s = Globals.ApplicationName + " " + Globals.Version.ToString(3);
        if (IsProjectLoaded && CurrentSchedule != null && CurrentSchedule.Schedule != null)
        {
          s += " [" + CurrentSchedule.Schedule.Name + "]";
        }

        return s;
      }
    }

    public bool IsProjectLoaded => CurrentSchedule?.Schedule != null;

    public ObservableCollection<ScheduleViewModel> Schedules
    {
      get
      {
        if (_schedules == null)
        {
          _schedules = [];
          _schedules.CollectionChanged += (sender, e) =>
          {
            if (e.Action is NotifyCollectionChangedAction.Add or NotifyCollectionChangedAction.Replace)
            {
              foreach (ScheduleViewModel schedule in e.NewItems)
              {
                schedule.CalculationProgress += Schedule_CalculationProgress;
              }
            }
            if (e.Action is NotifyCollectionChangedAction.Remove or NotifyCollectionChangedAction.Replace)
            {
              foreach (ScheduleViewModel schedule in e.OldItems)
              {
                schedule.CalculationProgress -= Schedule_CalculationProgress;
              }
            }
          };
        }
        return _schedules;
      }
    }

    public ScheduleViewModel CurrentSchedule
    {
      get => _currentSchedule;
      set
      {
        if (_currentSchedule != value)
        {
          _currentSchedule = value;
          OnPropertyChanged(nameof(CurrentSchedule));
          OnPropertyChanged(nameof(IsProjectLoaded));
          OnPropertyChanged(nameof(WindowTitle));
          if (IsProjectLoaded)
          {
            ActivateRibbonStartTab?.Invoke(this, EventArgs.Empty);
          }
        }
      }
    }

    public bool InstantHelpVisible
    {
      get => SettingsController.Settings.ShowInstantHelpOnStartUp;
      set
      {
        if (SettingsController.Settings.ShowInstantHelpOnStartUp != value)
        {
          SettingsController.Settings.ShowInstantHelpOnStartUp = value;
          OnPropertyChanged(nameof(InstantHelpVisible));
        }
      }
    }

    public ObservableCollection<ReportViewModel> Reports { get; } = [];

    public ReportViewModel SelectedReport
    {
      get => _selectedReport;
      set
      {
        if (_selectedReport != value)
        {
          _selectedReport = value;
          OnPropertyChanged(nameof(SelectedReport));
        }
      }
    }

    #endregion

    #region New Schedule

    public ICommand CreateNewScheduleCommand => _createNewScheduleCommand ??= new ActionCommand(CreateNewSchedule, CanCreateNewSchedule);

    private void CreateNewSchedule()
    {
      using var vm = new NewScheduleViewModel();
      if (ViewFactory.Instance.ShowDialog(vm) == true)
      {
        var newSchedule = new Schedule();
        var newVM = new ScheduleViewModel(newSchedule);
        Schedules.Add(newVM);
        CurrentSchedule = newVM;
      }
    }

    private bool CanCreateNewSchedule()
    {
      return !IsBusy;
    }

    #endregion

    #region Open Schedule

    public ICommand OpenScheduleCommand => _openScheduleCommand ??= new ActionCommand<string>(OpenSchedule, CanOpenSchedule);

    private void OpenSchedule(string param)
    {
      string fileName = null;

      if (!string.IsNullOrWhiteSpace(param))
      {
        if (File.Exists(param))
        {
          fileName = param;
        }
        else
        {
          UserNotificationService.Instance.Error(NASResources.MessageScheduleNotFound);
          return;
        }
      }

      if (string.IsNullOrWhiteSpace(fileName))
      {
        var openFileDialog = _lazyOpenFileDialog.Value;

        if (openFileDialog.ShowDialog() == true)
        {
          fileName = openFileDialog.FileName;
        }
        else
        {
          return;
        }
      }

      if (!File.Exists(fileName))
      {
        UserNotificationService.Instance.Error(string.Format(NASResources.MessageFileNotFound, fileName));
        return;
      }

      try
      {
        string ext = Path.GetExtension(fileName);
        var filter = new NASFilter();

        if (string.Equals(filter.FileExtension, ext, StringComparison.InvariantCultureIgnoreCase))
        {
          var schedule = Persistency.Load(fileName);

          if (!string.IsNullOrWhiteSpace(filter.Output))
          {
            UserNotificationService.Instance.Information(filter.Output);
          }

          ScheduleController.AddMinimumData(schedule);
          ScheduleController.CheckValues(schedule);
          var newVM = new ScheduleViewModel(schedule);
          Schedules.Add(newVM);
          CurrentSchedule = newVM;
        }
      }
      catch (Exception ex)
      {
        UserNotificationService.Instance.Error(ex.Message);
      }
    }

    public bool CanOpenSchedule(string _)
    {
      return !IsBusy;
    }

    #endregion

    #region Close Schedule

    public ICommand CloseScheduleCommand => _closeScheduleCommand ??= new ActionCommand(CloseSchedule, CanCloseSchedule);

    public void CloseSchedule()
    {
      RemoveScheduleViewModel(CurrentSchedule);
    }

    private bool CanCloseSchedule()
    {
      return CurrentSchedule != null && !IsBusy;
    }

    #endregion

    #region Save Schedule

    public ICommand SaveScheduleCommand => _saveScheduleCommand ??= new ActionCommand(SaveSchedule, CanSaveSchedule);

    private void SaveSchedule()
    {
      string fileName = null;

      if (string.IsNullOrWhiteSpace(CurrentSchedule.Schedule.FileName) && CanSaveScheduleAs())
      {
        SaveScheduleAs();
      }

      try
      {
        string ext = Path.GetExtension(fileName).ToLower();
        var filter = new NASFilter();

        if ("." + filter.FileExtension.ToLower() == ext)
        {
          Persistency.Save(CurrentSchedule.Schedule, fileName);
        }
      }
      catch (Exception ex)
      {
        UserNotificationService.Instance.Error(ex.Message);
      }
    }

    public bool CanSaveSchedule()
    {
      return CurrentSchedule != null && !IsBusy;
    }

    #endregion

    #region Save Schedule As

    public ICommand SaveScheduleAsCommand => _saveScheduleAsCommand ??= new ActionCommand(SaveScheduleAs, CanSaveScheduleAs);

    private void SaveScheduleAs()
    {
      var saveFileDialog = _lazySaveFileDialog.Value;

      if (saveFileDialog.ShowDialog() == true)
      {
        CurrentSchedule.Schedule.FileName = saveFileDialog.FileName;
        SaveSchedule();
      }
    }

    public bool CanSaveScheduleAs()
    {
      return CurrentSchedule != null && !IsBusy;
    }

    #endregion

    #region Import Schedule

    public ICommand ImportScheduleCommand => _importScheduleCommand ??= new ActionCommand(ImportSchedule, CanImportSchedule);

    private void ImportSchedule()
    {
      var openFileDialog = _lazyImportFileDialog.Value;

      string fileName;
      if (openFileDialog.ShowDialog() == true)
      {
        fileName = openFileDialog.FileName;
      }
      else
      {
        return;
      }

      if (!File.Exists(fileName))
      {
        UserNotificationService.Instance.Error(string.Format(NASResources.MessageFileNotFound, fileName));
        return;
      }
      try
      {
        string ext = Path.GetExtension(fileName).ToLower();
        foreach (var filter in FilterFactory.ImportFilters)
        {
          if ("." + filter.FileExtension.ToLower() == ext)
          {
            var schedule = filter.Import(fileName);
            schedule.CreatedDate = DateTime.Now;
            schedule.CreatedBy = Globals.UserName;

            if (!string.IsNullOrWhiteSpace(filter.Output))
            {
              UserNotificationService.Instance.Information(filter.Output);
            }

            ScheduleController.AddMinimumData(schedule);
            ScheduleController.CheckValues(schedule);
            var newVM = new ScheduleViewModel(schedule);
            Schedules.Add(newVM);
            CurrentSchedule = newVM;
          }
        }
      }
      catch (Exception ex)
      {
        UserNotificationService.Instance.Error(ex.Message);
      }
    }

    private bool CanImportSchedule()
    {
      return CurrentSchedule != null && !IsBusy;
    }

    #endregion

    #region Export Schedule

    public ICommand ExportScheduleCommand => _exportScheduleAsCommand ??= new ActionCommand(ExportSchedule, CanExportSchedule);

    private void ExportSchedule()
    {
      var saveFileDialog = _lazyExportFileDialog.Value;

      if (saveFileDialog.ShowDialog() == true)
      {
        string fileName = saveFileDialog.FileName;
        try
        {
          string ext = Path.GetExtension(fileName).ToLower();
          foreach (var filter in FilterFactory.ExportFilters)
          {
            if ("." + filter.FileExtension.ToLower() == ext)
            {
              filter.Export(CurrentSchedule.Schedule, fileName);

              if (!string.IsNullOrWhiteSpace(filter.Output))
              {
                UserNotificationService.Instance.Information(filter.Output);
              }
            }
          }
        }
        catch (Exception ex)
        {
          UserNotificationService.Instance.Error(ex.Message);
        }
      }
    }

    private bool CanExportSchedule()
    {
      return CurrentSchedule != null && !IsBusy;
    }

    #endregion

    #region Printing

    public ICommand PrintCommand => _printCommand ??= new ActionCommand(Print, CanPrint);

    private void Print()
    {
      try
      {
        System.Printing.LocalPrintServer.GetDefaultPrintQueue();
      }
      catch
      {
        UserNotificationService.Instance.Error(NASResources.MessageNoDefaultPrinter);
        return;
      }

      try
      {
        var viewModel = CurrentSchedule;

        var args = new RequestItemEventArgs<LayoutType, IPrintableCanvas>(viewModel.ActiveLayout.LayoutType);
        GetCanvas?.Invoke(this, args);
        var canvas = args.ReturnedItem ?? throw new ApplicationException("Canvas not defined");
        canvas.DataContext = viewModel;

        using var vm = new PrintPreviewViewModel(CurrentSchedule, canvas);
        ViewFactory.Instance.ShowDialog(vm);
      }
      catch (Exception ex)
      {
        string message = ex.Message;
        if (ex.InnerException != null)
        {
          message += Environment.NewLine + ex.InnerException.Message;
        }

        message += Environment.NewLine + ex.StackTrace;
        UserNotificationService.Instance.Error(message);
      }
    }

    private bool CanPrint()
    {
      return CurrentSchedule != null && !IsBusy;
    }

    #endregion

    #region Close

    public ICommand CloseCommand => _closeCommand ??= new ActionCommand(Close, CanClose);

    private void Close()
    {
      UserNotificationService.Instance.Question(NASResources.MessageClose, () =>
      {
        InstantHelpManager.Instance.HideHelpWindow();
        Application.Current.Shutdown();
      });
    }

    private bool CanClose()
    {
      return !IsBusy;
    }

    #endregion

    #region Program Settings

    public ICommand OpenProgramSettingsCommand => _openProgramSettingsCommand ??= new ActionCommand(OpenProgramSettings, CanOpenProgramSettings);

    private void OpenProgramSettings()
    {
      var oldTheme = SettingsController.Settings.Theme;

      var vm = new SettingsViewModel(() => OnThemeChangeRequested?.Invoke(this, new ItemEventArgs<Theme>(oldTheme)));
      vm.PropertyChanged += (sender, args) =>
      {
        if (args.PropertyName == nameof(SettingsViewModel.SelectedTheme))
        {
          OnThemeChangeRequested?.Invoke(this, new ItemEventArgs<Theme>(vm.SelectedTheme));
        }
      };

      ViewFactory.Instance.ShowDialog(vm);
    }

    private bool CanOpenProgramSettings()
    {
      return !IsBusy;
    }

    #endregion

    #region Reports

    #region Show Report

    public ICommand ShowReportCommand => _showReportCommand ??= new ActionCommand(ShowReport, CanShowReport);

    private void ShowReport()
    {
      try
      {
        ReportManager.ShowReport(SelectedReport.Report, CurrentSchedule.Schedule);
      }
      catch (Exception ex)
      {
        UserNotificationService.Instance.Error(ex.Message);
      }
    }

    private bool CanShowReport()
    { 
      return !IsBusy && CurrentSchedule != null && SelectedReport != null;
    }

    #endregion

    #region Edit Report

    public ICommand EditReportCommand => _editReportCommand ??= new ActionCommand(EditReport, CanEditReport);

    private void EditReport()
    {
      ReportManager.EditReport(SelectedReport.Report, CurrentSchedule.Schedule);
    }

    private bool CanEditReport()
    {
      return !IsBusy && CurrentSchedule != null && SelectedReport != null && SelectedReport.Report.ReportLevel == ReportLevel.User;
    }

    #endregion

    #region Copy Report

    public ICommand CopyReportCommand => _copyReportCommand ??= new ActionCommand(CopyReport, CanCopyReport);

    private void CopyReport()
    {
      try
      {
        var vm = new GetTextViewModel(NASResources.CopyReport, NASResources.ReportName, SelectedReport.Report.Name);
        if (ViewFactory.Instance.ShowDialog(vm) == true)
        {
          // Duplicate names are not allowed on the same level
          if (Reports.Any(x => x.Report.Name.Equals(vm.Text, StringComparison.OrdinalIgnoreCase)))
          {
            UserNotificationService.Instance.Error(NASResources.MessageReportExists);
            return;
          }

          var newReport = ReportsController.CopyReport(SelectedReport.Report, vm.Text);

          if (newReport == null)
            return;

          var newVM = new ReportViewModel(newReport);
          Reports.Add(newVM);
        }
      }
      catch (Exception ex)
      {
        UserNotificationService.Instance.Error(ex.Message);
      }
    }

    private bool CanCopyReport()
    {
      return !IsBusy && SelectedReport != null;
    }

    #endregion

    #region Delete Report

    public ICommand DeleteReportCommand => _deleteReportCommand ??= new ActionCommand(DeleteReport, CanDeleteReport);

    private void DeleteReport()
    {
      UserNotificationService.Instance.Question(string.Format(NASResources.MessageDeleteReport, SelectedReport.Report.Name), () =>
      {
        ReportsController.DeleteReport(SelectedReport.Report);
      });
    }

    private bool CanDeleteReport()
    {
      return !IsBusy && SelectedReport != null && SelectedReport.Report.ReportLevel != ReportLevel.Integrated;
    }

    #endregion

    #region Rename Report

    public ICommand RenameReportCommand => _renameReportCommand ??= new ActionCommand(RenameReport, CanRenameReport);

    private void RenameReport()
    {
      var vm = new GetTextViewModel(NASResources.RenameReport, NASResources.ReportName, SelectedReport.Report.Name);
      if (ViewFactory.Instance.ShowDialog(vm) == true)
      {
        SelectedReport.Report.Name = vm.Text;
      }
    }

    private bool CanRenameReport()
    {
      return !IsBusy && SelectedReport != null && SelectedReport.Report.ReportLevel != ReportLevel.Integrated;
    }

    #endregion

    #endregion

    #region Help

    #region About

    public ICommand AboutCommand => _aboutCommand ??= new ActionCommand(About);

    private static void About()
    {
      ViewFactory.Instance.ShowDialog(new AboutViewModel());
    }

    #endregion

    #region Website

    public ICommand WebsiteCommand => _websiteCommand ??= new ActionCommand(Website);

    public static void Website()
    {
      string url = Globals.Website;

      try
      {
        Process.Start(url);
      }
      catch
      {
        // Hack because of this: https://github.com/dotnet/corefx/issues/10361
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
          url = url.Replace("&", "^&");
          Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }
      }
    }

    #endregion

    #region Instant Help

    public ICommand InstantHelpCommand => _instantHelpCommand ??= new ActionCommand(InstantHelp);

    public void InstantHelp()
    {
      if (InstantHelpVisible)
      {
        InstantHelpManager.Instance.ShowHelpWindow();
      }
      else
      {
        InstantHelpManager.Instance.HideHelpWindow();
      }
    }

    #endregion

    #endregion

    #region Private Methods

    private void Schedule_CalculationProgress(object sender, ProgressEventArgs e)
    {
      if (e == ProgressEventArgs.Starting)
      {
        SetIsBusy(true);
      }
      else if (e == ProgressEventArgs.Finished)
      {
        SetIsBusy(false);
        SetProgressMessage(NASResources.PleaseWait);
      }

      SetProgressMessage($"{NASResources.Calculating}: {e.CurrentProgress:#.0} %");
    }

    private static OpenFileDialog GetOpenFileDialog()
    {
      var openFileDialog = new OpenFileDialog();
      var filter = new NASFilter();
      string f = $"{filter.FilterName}|*{filter.FileExtension}";

      openFileDialog.Filter = f;
      openFileDialog.AddExtension = true;
      openFileDialog.CheckFileExists = true;
      return openFileDialog;
    }

    private static OpenFileDialog GetImportFileDialog()
    {
      var openFileDialog = new OpenFileDialog();
      string f = "";

      foreach (IFilter filter in FilterFactory.ImportFilters)
      {
        if (!string.IsNullOrEmpty(f))
        {
          f += "|";
        }

        f += $"{filter.FilterName}|*{filter.FileExtension}";
      }
      openFileDialog.Filter = f;
      openFileDialog.AddExtension = true;
      openFileDialog.CheckFileExists = true;
      return openFileDialog;
    }

    private static SaveFileDialog GetSaveFileDialog()
    {
      var saveFileDialog = new SaveFileDialog();

      var filter = new NASFilter();
      string f = $"{filter.FilterName}|*{filter.FileExtension}";

      saveFileDialog.Filter = f;
      saveFileDialog.AddExtension = true;
      return saveFileDialog;
    }

    private static SaveFileDialog GetExportFileDialog()
    {
      var saveFileDialog = new SaveFileDialog();
      string f = "";

      foreach (IFilter filter in FilterFactory.ExportFilters)
      {
        if (!string.IsNullOrWhiteSpace(f))
        {
          f += "|";
        }

        f += $"{filter.FilterName}|*{filter.FileExtension}";
      }
      saveFileDialog.Filter = f;
      saveFileDialog.AddExtension = true;
      return saveFileDialog;
    }

    private void RefreshCommands()
    {
      _createNewScheduleCommand.RaiseCanExecuteChanged();
      _openScheduleCommand.RaiseCanExecuteChanged();
      _closeScheduleCommand.RaiseCanExecuteChanged();
      _saveScheduleCommand.RaiseCanExecuteChanged();
      _saveScheduleAsCommand.RaiseCanExecuteChanged();
      _importScheduleCommand.RaiseCanExecuteChanged();
      _exportScheduleAsCommand.RaiseCanExecuteChanged();
      _printCommand.RaiseCanExecuteChanged();
      _closeCommand.RaiseCanExecuteChanged();
      _openProgramSettingsCommand.RaiseCanExecuteChanged();
      _aboutCommand.RaiseCanExecuteChanged();
      _websiteCommand.RaiseCanExecuteChanged();
      _instantHelpCommand.RaiseCanExecuteChanged();
      _showReportCommand.RaiseCanExecuteChanged();
      _editReportCommand.RaiseCanExecuteChanged();
      _copyReportCommand.RaiseCanExecuteChanged();
      _deleteReportCommand.RaiseCanExecuteChanged();
      _renameReportCommand.RaiseCanExecuteChanged();
    }

    private void SetIsBusy(bool busy)
    {
      bool stateChanged = false;
      lock (_busyLock)
      {
        if (busy)
        {
          _busyCount++;
        }
        else
        {
          _busyCount--;
          _busyCount = Math.Max(_busyCount, 0);
        }

        bool newIsBusy = _busyCount > 0;
        stateChanged = newIsBusy != IsBusy;
        IsBusy = newIsBusy;
      }

      if (stateChanged)
      {
        UITools.Instance.BeginInvokeIfRequired(() =>
        {
          OnPropertyChanged(nameof(IsBusy));
          RefreshCommands();
        });
      }
    }

    private void SetProgressMessage(string message)
    {
      ProgressMessage = message;
      OnPropertyChanged(nameof(ProgressMessage));
    }

    private void RemoveScheduleViewModel(ScheduleViewModel viewModel)
    {
      viewModel.LayoutTypeChanged -= ViewModel_LayoutTypeChanged;
      Schedules.Remove(viewModel);
    }

    private void ViewModel_LayoutTypeChanged(object sender, EventArgs e)
    {
      if (sender is ScheduleViewModel oldVM)
      {
        var schedule = oldVM.Schedule;
        var viewModel = new ScheduleViewModel(schedule);
        var buffer = new List<ScheduleViewModel>(Schedules);
        Schedules.Add(viewModel);
        var view = Schedules.GetView();
        view.Refresh();
        OnPropertyChanged(nameof(Schedules));
        CurrentSchedule = viewModel;
        viewModel.LayoutTypeChanged += ViewModel_LayoutTypeChanged;
        RemoveScheduleViewModel(sender as ScheduleViewModel);
        CurrentSchedule = viewModel;
      }
    }

    private static void Configure()
    {
      string fileName = Path.Combine(ApplicationHelper.StartupPath, "NAS.Config.exe");
      if (File.Exists(fileName))
      {
        Process.Start(fileName);
      }
      else
      {
        UserNotificationService.Instance.Error(string.Format(NASResources.MessageFileNotFound, fileName));
      }
    }

    private static void SetCultureOfCurrentThread(CultureInfo currentCulture)
    {
      if (currentCulture != null)
      {
        Thread.CurrentThread.CurrentCulture = currentCulture;
        Thread.CurrentThread.CurrentUICulture = currentCulture;
      }
    }

    #endregion

    #region IDisposable

    protected override void Dispose(bool disposing)
    {
      foreach (var vm in Schedules)
      {
        vm.Dispose();
      }
      base.Dispose(disposing);
    }

    #endregion
  }
}
