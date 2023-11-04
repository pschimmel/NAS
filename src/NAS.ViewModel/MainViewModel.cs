using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ES.Tools.Core.Infrastructure;
using ES.Tools.Core.MVVM;
using Microsoft.Win32;
using NAS.Model;
using NAS.Model.Base;
using NAS.Model.Controllers;
using NAS.Model.Entities;
using NAS.Model.Enums;
using NAS.Model.ImportExport;
using NAS.Resources;
using NAS.ViewModel.Base;
using NAS.ViewModel.Helpers;
using NAS.ViewModel.Printing;
using NAS.Model.Settings;

namespace NAS.ViewModel
{
  public class MainViewModel : ViewModelBase
  {
    #region Events

    public event EventHandler<ProgressEventArgs> DownloadProgress;
    public event EventHandler<ItemEventArgs<Themes>> RequestThemeChange;
    public event EventHandler<RequestItemEventArgs<LayoutType, IPrintableCanvas>> GetCanvas;
    public event EventHandler<ItemEventArgs<(Schedule Schedule, string FileName, CultureInfo Language)>> ShowFastReport;
    public event EventHandler<ItemEventArgs<(Schedule Schedule, string FileName, CultureInfo Language)>> EditFastReport;

    #endregion

    #region Fields

    private int _busyCount = 0;
    private bool _backstageOpen = false;
    private ObservableCollection<ScheduleViewModel> _schedules;
    private ScheduleViewModel _currentSchedule;
    private ReportViewModel _selectedReport;
    private readonly ReportController _reportsController;
    private readonly Lazy<OpenFileDialog> _lazyOpenFileDialog = new(GetOpenFileDialog);
    private readonly Lazy<SaveFileDialog> _lazySaveFileDialog = new(GetSaveFileDialog);
    private readonly Lazy<OpenFileDialog> _lazyImportFileDialog = new(GetImportFileDialog);
    private readonly Lazy<SaveFileDialog> _lazyExportFileDialog = new(GetExportFileDialog);

    #endregion

    #region Constructor

    public MainViewModel()
    {
      _reportsController = new ReportController(SettingsController.Settings.UserReportsFolderPath);
      _reportsController.ReportsChanged += (s, e) => RefreshReports();

      if (SettingsController.Settings.ShowInstantHelpOnStartUp)
      {
        InstantHelpCommandExecute();
      }

      InstantHelpManager.Instance.HelpWindowsChanged += (_, __) => OnPropertyChanged(nameof(InstantHelpVisible));

      NewScheduleCommand = new ActionCommand(NewScheduleCommandExecute, () => NewScheduleCommandCanExecute);
      OpenScheduleCommand = new ActionCommand(param => OpenScheduleCommandExecute(param), param => OpenScheduleCommandCanExecute);
      SaveScheduleCommand = new ActionCommand(SaveScheduleCommandExecute, () => SaveScheduleCommandCanExecute);
      SaveScheduleAsCommand = new ActionCommand(SaveScheduleAsCommandExecute, () => SaveScheduleAsCommandCanExecute);
      ImportScheduleCommand = new ActionCommand(ImportScheduleCommandExecute, () => ImportScheduleCommandCanExecute);
      ExportScheduleCommand = new ActionCommand(ExportScheduleCommandExecute, () => ExportScheduleCommandCanExecute);
      CloseScheduleCommand = new ActionCommand(CloseScheduleCommandExecute, () => CloseScheduleCommandCanExecute);
      PrintCommand = new ActionCommand(PrintCommandExecute, () => PrintCommandCanExecute);
      CloseCommand = new ActionCommand(CloseCommandExecute, () => CloseCommandCanExecute);
      ProgramSettingsCommand = new ActionCommand(ProgramSettingsCommandExecute, () => ProgramSettingsCommandCanExecute);
      DatabaseSettingsCommand = new ActionCommand(DatabaseSettingsCommandExecute, () => DatabaseSettingsCommandCanExecute);
      CheckForUpdatesCommand = new ActionCommand(param => CheckForUpdatesCommandExecute(null), param => CheckForUpdatesCommandCanExecute);
      AboutCommand = new ActionCommand(AboutCommandExecute);
      WebsiteCommand = new ActionCommand(WebsiteCommandExecute);
      InstantHelpCommand = new ActionCommand(InstantHelpCommandExecute);
      ShowReportCommand = new ActionCommand(ShowReportCommandExecute, () => ShowReportCommandCanExecute);
      EditReportCommand = new ActionCommand(EditReportCommandExecute, () => EditReportCommandCanExecute);
      CopyReportCommand = new ActionCommand(CopyReportCommandExecute, () => CopyReportCommandCanExecute);
      DeleteReportCommand = new ActionCommand(DeleteReportCommandExecute, () => DeleteReportCommandCanExecute);
      RenameReportCommand = new ActionCommand(RenameReportCommandExecute, () => RenameReportCommandCanExecute);
      RefreshReports();
    }

    #endregion

    #region Public Properties

    public static bool ShutdownRequested { get; set; } = false;

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

#pragma warning disable CA1822 // Mark members as static
    public string Version => Globals.Version.ToString(3);
#pragma warning restore CA1822 // Mark members as static

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
          _schedules = new ObservableCollection<ScheduleViewModel>();
          _schedules.CollectionChanged += (sender, e) =>
          {
            if (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Replace)
            {
              foreach (ScheduleViewModel schedule in e.NewItems)
              {
                schedule.CalculationProgress += Schedule_CalculationProgress;
              }
            }
            if (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Replace)
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

    public ObservableCollection<ReportViewModel> Reports { get; } = new ObservableCollection<ReportViewModel>();

    public void RefreshReports()
    {
      Reports.Clear();
      _reportsController.Reports.ToList().ForEach(x => Reports.Add(new ReportViewModel(x)));
    }

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

    public ICommand NewScheduleCommand { get; }

    private void NewScheduleCommandExecute()
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

    private bool NewScheduleCommandCanExecute => !IsBusy;

    #endregion

    #region Open Schedule

    public ICommand OpenScheduleCommand { get; }

    private void OpenScheduleCommandExecute(object param)
    {
      string fileName = null;

      if (param != null && param is string paramAsString)
      {
        if (File.Exists(paramAsString))
        {
          fileName = paramAsString;
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

        if (!File.Exists(fileName))
        {
          UserNotificationService.Instance.Error(string.Format(NASResources.MessageFileNotFound, fileName));
          return;
        }

        try
        {
          string ext = Path.GetExtension(fileName).ToLower();
          var filter = new NASFilter();

          if ("." + filter.FileExtension.ToLower() == ext)
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
    }

    public bool OpenScheduleCommandCanExecute => !IsBusy;

    #endregion

    #region Save Schedule

    public ICommand SaveScheduleCommand { get; }

    private void SaveScheduleCommandExecute()
    {
      string fileName = null;

      if (string.IsNullOrWhiteSpace(CurrentSchedule.Schedule.FileName) && SaveScheduleAsCommandCanExecute)
      {
        SaveScheduleAsCommandExecute();
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

    public bool SaveScheduleCommandCanExecute => CurrentSchedule != null && !IsBusy;

    #endregion

    #region Save Schedule As

    public ICommand SaveScheduleAsCommand { get; }

    private void SaveScheduleAsCommandExecute()
    {
      var saveFileDialog = _lazySaveFileDialog.Value;

      if (saveFileDialog.ShowDialog() == true)
      {
        CurrentSchedule.Schedule.FileName = saveFileDialog.FileName;
        SaveScheduleCommandExecute();
      }
    }

    public bool SaveScheduleAsCommandCanExecute => CurrentSchedule != null && !IsBusy;

    #endregion

    #region Import Schedule

    public ICommand ImportScheduleCommand { get; }

    internal void ImportScheduleCommandExecute()
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

    private void Controller_ImportMessages(object sender, ItemEventArgs<string> e)
    {
      UserNotificationService.Instance.Error(e.Item);
    }

    private bool ImportScheduleCommandCanExecute => CurrentSchedule != null && !IsBusy;

    #endregion

    #region Export Schedule

    public ICommand ExportScheduleCommand { get; }

    private void ExportScheduleCommandExecute()
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

    private bool ExportScheduleCommandCanExecute => CurrentSchedule != null && !IsBusy;

    #endregion

    #region Close Schedule

    public ICommand CloseScheduleCommand { get; }

    public void CloseScheduleCommandExecute()
    {
      RemoveScheduleViewModel(CurrentSchedule);
    }

    private bool CloseScheduleCommandCanExecute => CurrentSchedule != null && !IsBusy;

    #endregion

    #region Printing

    public ICommand PrintCommand { get; }

    private void PrintCommandExecute()
    {
      try
      {
        System.Printing.LocalPrintServer.GetDefaultPrintQueue();
      }
      catch
      {
        UserNotificationService.Instance.Error(NASResources.MessageNoStandardPrinter);
        return;
      }
      try
      {
        var viewModel = CurrentSchedule;

        var args = new RequestItemEventArgs<LayoutType, IPrintableCanvas>(viewModel.CurrentLayout.LayoutType);
        GetCanvas?.Invoke(this, args);
        var canvas = args.ReturnedItem;
        if (canvas == null)
        {
          throw new ApplicationException("Canvas not defined");
        }

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

    private bool PrintCommandCanExecute => CurrentSchedule != null && !IsBusy;

    #endregion

    #region Close

    public ICommand CloseCommand { get; }

    private void CloseCommandExecute()
    {
      UserNotificationService.Instance.Question(NASResources.MessageClose, () =>
      {
        InstantHelpManager.Instance.HideHelpWindow();
        Application.Current.Shutdown();
      });
    }

    private bool CloseCommandCanExecute => !IsBusy;

    #endregion

    #region Program Settings

    public ICommand ProgramSettingsCommand { get; }

    private void ProgramSettingsCommandExecute()
    {
      var oldTheme = SettingsController.Settings.Theme;

      var vm = new SettingsViewModel(() => RequestThemeChange?.Invoke(this, new ItemEventArgs<Themes>(oldTheme)));
      vm.PropertyChanged += (sender, args) =>
      {
        if (args.PropertyName == nameof(SettingsViewModel.SelectedTheme))
        {
          RequestThemeChange?.Invoke(this, new ItemEventArgs<Themes>(vm.SelectedTheme));
        }
      };

      ViewFactory.Instance.ShowDialog(vm);
    }

    private bool ProgramSettingsCommandCanExecute => !IsBusy;

    #endregion

    #region Database Settings

    public ICommand DatabaseSettingsCommand { get; }

    private void DatabaseSettingsCommandExecute()
    {
      Configure();
    }

    private bool DatabaseSettingsCommandCanExecute => !IsBusy;

    #endregion

    #region Reports

    #region Show Report

    public ICommand ShowReportCommand { get; }

    private void ShowReportCommandExecute()
    {
      try
      {
        ShowFastReport?.Invoke(this, new ItemEventArgs<(Schedule, string, CultureInfo)>((_currentSchedule.Schedule, SelectedReport.Report.FileName, Thread.CurrentThread.CurrentUICulture)));
      }
      catch (Exception ex)
      {
        UserNotificationService.Instance.Error(ex.Message);
      }
    }

    private bool ShowReportCommandCanExecute => CurrentSchedule != null && SelectedReport != null;

    #endregion

    #region Edit Report

    public ICommand EditReportCommand { get; }

    private void EditReportCommandExecute()
    {
      EditFastReport?.Invoke(this, new ItemEventArgs<(Schedule, string, CultureInfo)>((_currentSchedule.Schedule, SelectedReport.Report.FileName, Thread.CurrentThread.CurrentUICulture)));
    }

    private bool EditReportCommandCanExecute => CurrentSchedule != null && SelectedReport != null;

    #endregion

    #region Copy Report

    public ICommand CopyReportCommand { get; }

    private void CopyReportCommandExecute()
    {
      try
      {
        var vm = new GetTextViewModel(NASResources.CopyReport, NASResources.ReportName, SelectedReport.Report.Name);
        if (ViewFactory.Instance.ShowDialog(vm) == true)
        {
          _reportsController.CopyReport(SelectedReport.Report, vm.Text);
        }
      }
      catch (Exception ex)
      {
        UserNotificationService.Instance.Error(ex.Message);
      }
    }

    private bool CopyReportCommandCanExecute => SelectedReport != null;

    #endregion

    #region Delete Report

    public ICommand DeleteReportCommand { get; }

    private void DeleteReportCommandExecute()
    {
      UserNotificationService.Instance.Question(string.Format(NASResources.MessageDeleteReport, SelectedReport.Report.Name), () =>
      {
        _reportsController.DeleteReport(SelectedReport.Report);
      });
    }

    private bool DeleteReportCommandCanExecute => SelectedReport != null && !SelectedReport.Report.IsReadOnly;

    #endregion

    #region Rename Report

    public ICommand RenameReportCommand { get; }

    private void RenameReportCommandExecute()
    {
      var vm = new GetTextViewModel(NASResources.RenameReport, NASResources.ReportName, SelectedReport.Report.Name);
      if (ViewFactory.Instance.ShowDialog(vm) == true)
      {
        SelectedReport.Report.Name = vm.Text;
      }

      RefreshReports();
    }

    private bool RenameReportCommandCanExecute => SelectedReport != null && !SelectedReport.Report.IsReadOnly;

    #endregion

    #endregion

    #region Help

    #region About

    public ICommand AboutCommand { get; }

    private static void AboutCommandExecute()
    {
      ViewFactory.Instance.ShowDialog(new AboutViewModel());
    }

    #endregion

    #region Check for Updates

    public ICommand CheckForUpdatesCommand { get; }

    private async void CheckForUpdatesCommandExecute(object param)
    {
      SetIsBusy(true);
      if (!string.IsNullOrWhiteSpace(SettingsController.Settings.Language))
      {
        SetCultureOfCurrentThread(Cultures.GetCultureInfoFromNativeName(SettingsController.Settings.Language));
      }

      bool showMessages = true;

      if (param != null && (bool)param == false)
      {
        showMessages = false;
      }

      await Task.Run(async () =>
      {
        try
        {
          var service = new UpdateServiceHelper();
          var remoteVersion = await service.GetLatestVersion();
          var installedVersion = Globals.Version;
          if (remoteVersion > installedVersion)
          {
            var changes = await service.GetChanges(installedVersion);
            string message = string.Format(NASResources.MessageNewVersionFound, installedVersion.ToString(3), remoteVersion.ToString(3));
            if (changes.Any())
            {
              message += Environment.NewLine + Environment.NewLine + NASResources.Changes + ":";
              foreach (var change in changes)
              {
                if (!string.IsNullOrWhiteSpace(change.Description))
                {
                  message += Environment.NewLine + change.ToString();
                }
              }
            }

            UserNotificationService.Instance.Question(message, async () =>
            {
              string fileURL = await service.GetDownloadURL();
              fileURL = fileURL.Replace(@"\", "/");

              if (string.IsNullOrEmpty(fileURL))
              {
                UserNotificationService.Instance.Error(string.Format(NASResources.MessageFileNotFound, fileURL));
                return;
              }
              var saveFileDialog = new SaveFileDialog
              {
                FileName = fileURL.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries).Last()
              };
              if (saveFileDialog.ShowDialog() == true)
              {
                await Task.Run(new Action(async () =>
                {
                  using var client = new HttpClient();
                  using var stream = await client.GetStreamAsync(fileURL);
                  using var fs = File.Create(saveFileDialog.FileName);
                  stream.Seek(0, SeekOrigin.Begin);
                  stream.CopyTo(fs);
                }));
              }
            });
          }
          else if (showMessages)
          {
            UserNotificationService.Instance.Information(string.Format(NASResources.MessageNoNewVersion, installedVersion.ToString(3)));
          }
        }
        catch (Exception ex)
        {
          if (showMessages)
          {
            UserNotificationService.Instance.Error(NASResources.MessageErrorCheckingVersion + Environment.NewLine + ex.Message);
          }
        }
        finally
        {
          SetIsBusy(false);
        }
      });
    }

    private void WebClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
    {
      DownloadProgress.Invoke(this, ProgressEventArgs.Finished);

      if (e.Cancelled)
      {
        UserNotificationService.Instance.Warning(NASResources.MessageDownloadCanceled);
        return;
      }

      UserNotificationService.Instance.Question(NASResources.MessageDownloadFinished, () =>
      {
        try
        {
          Process.Start(e.UserState.ToString(), @"/SILENT");
          Environment.Exit(0);
        }
        catch (Exception ex)
        {
          UserNotificationService.Instance.Error(NASResources.MessageUpdateError + Environment.NewLine + ex.Message);
        }
      });
    }

    private bool CheckForUpdatesCommandCanExecute => !IsBusy;

    #endregion

    #region Website

    public ICommand WebsiteCommand { get; }

    public static void WebsiteCommandExecute()
    {
      Process.Start("http://www.engineeringsolutions.de/");
    }

    #endregion

    #region Instant Help

    public ICommand InstantHelpCommand { get; }

    public void InstantHelpCommandExecute()
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

    #region Private Members

    private static OpenFileDialog GetOpenFileDialog()
    {
      var openFileDialog = new OpenFileDialog();
      IFilter filter = new NASFilter();
      string f = $"{filter.FilterName}|*.{filter.FileExtension}";

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

        f += $"{filter.FilterName}|*.{filter.FileExtension}";
      }
      openFileDialog.Filter = f;
      openFileDialog.AddExtension = true;
      openFileDialog.CheckFileExists = true;
      return openFileDialog;
    }

    private static SaveFileDialog GetSaveFileDialog()
    {
      var saveFileDialog = new SaveFileDialog();

      IFilter filter = new NASFilter();
      string f = $"{filter.FilterName}|*.{filter.FileExtension}";

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

        f += $"{filter.FilterName}|*.{filter.FileExtension}";
      }
      saveFileDialog.Filter = f;
      saveFileDialog.AddExtension = true;
      return saveFileDialog;
    }

    private void RefreshCommands()
    {
      (OpenScheduleCommand as ActionCommand).RaiseCanExecuteChanged();
      (CloseScheduleCommand as ActionCommand).RaiseCanExecuteChanged();
      (PrintCommand as ActionCommand).RaiseCanExecuteChanged();
      (CloseCommand as ActionCommand).RaiseCanExecuteChanged();
      (ProgramSettingsCommand as ActionCommand).RaiseCanExecuteChanged();
      (DatabaseSettingsCommand as ActionCommand).RaiseCanExecuteChanged();
      (CheckForUpdatesCommand as ActionCommand).RaiseCanExecuteChanged();
      (AboutCommand as ActionCommand).RaiseCanExecuteChanged();
      (WebsiteCommand as ActionCommand).RaiseCanExecuteChanged();
      (InstantHelpCommand as ActionCommand).RaiseCanExecuteChanged();
      (ShowReportCommand as ActionCommand).RaiseCanExecuteChanged();
      (EditReportCommand as ActionCommand).RaiseCanExecuteChanged();
      (CopyReportCommand as ActionCommand).RaiseCanExecuteChanged();
      (DeleteReportCommand as ActionCommand).RaiseCanExecuteChanged();
      (RenameReportCommand as ActionCommand).RaiseCanExecuteChanged();
    }

    private void SetIsBusy(bool busy)
    {
      if (busy)
      {
        _busyCount++;
      }
      else
      {
        _busyCount--;
        _busyCount = Math.Min(_busyCount, 0);
      }

      UITools.Instance.BeginInvokeIfRequired(() =>
      {
        bool newIsBusy = _busyCount > 0;
        if (newIsBusy != IsBusy)
        {
          IsBusy = newIsBusy;
          OnPropertyChanged(nameof(IsBusy));
          RefreshCommands();
        }
      });
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
        var view = ES.Tools.Core.MVVM.ViewModelExtensions.GetView(Schedules);
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
      SettingsController.Save();
      foreach (var vm in Schedules)
      {
        vm.Dispose();
      }
      base.Dispose(disposing);
    }

    #endregion
  }
}
