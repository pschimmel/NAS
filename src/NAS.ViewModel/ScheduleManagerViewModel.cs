//using System;
//using System.Collections.ObjectModel;
//using System.IO;
//using System.Linq;
//using System.Windows.Input;
//using ES.Tools.Core.MVVM;
//using Microsoft.Win32;
//using NAS.Model.Base;
//using NAS.Model.ImportExport;
//using NAS.Resources;
//using NAS.ViewModel.Base;
//using NAS.ViewModel.Helpers;
//using NAS.ViewModel.Settings;

//namespace NAS.ViewModel
//{
//  public class ScheduleManagerViewModel : ViewModelBase
//  {
//    #region Events

//    public event EventHandler RequestSelectFirstTab;
//    public event EventHandler CloseBackstage;
//    public event EventHandler<ItemEventArgs<Guid>> OpenSchedule;

//    #endregion

//    #region Fields

//    private ScheduleInfoVM _currentSchedule;
//    private bool _isBusy = false;

//    #endregion

//    #region Constructor

//    public ScheduleManagerViewModel()
//      : base()
//    {
//      Schedules = new ObservableCollection<ScheduleInfoVM>(Controller.GetSchedules().ConvertAll(x => new ScheduleInfoVM(x)));
//      NewScheduleCommand = new ActionCommand(NewScheduleCommandExecute, () => NewScheduleCommandCanExecute);
//      RemoveScheduleCommand = new ActionCommand(RemoveScheduleCommandExecute, () => RemoveScheduleCommandCanExecute);
//      OpenScheduleCommand = new ActionCommand(param => OpenScheduleCommandExecute(param), param => OpenScheduleCommandCanExecute(param));
//      ImportScheduleCommand = new ActionCommand(param => ImportScheduleCommandExecute(null), param => ImportScheduleCommandCanExecute);
//      ExportScheduleCommand = new ActionCommand(ExportScheduleCommandExecute, () => ExportScheduleCommandCanExecute);
//    }

//    #endregion

//    #region Public Properties

//    public ObservableCollection<ScheduleInfoVM> Schedules { get; }

//    public ObservableCollection<RecentSchedule> RecentSchedules => ProgramSettings.Settings.RecentlyOpenedSchedules;

//    public ScheduleInfoVM CurrentSchedule
//    {
//      get => _currentSchedule;
//      set
//      {
//        if (_currentSchedule != value)
//        {
//          _currentSchedule = value;
//          OnPropertyChanged(nameof(CurrentSchedule));
//        }
//      }
//    }

//    #endregion

//    #region New Schedule

//    public ICommand NewScheduleCommand { get; }

//    private void NewScheduleCommandExecute()
//    {
//      using var vm = new NewScheduleViewModel();
//      if (ViewFactory.Instance.ShowDialog(vm) == true)
//      {
//        vm.SaveChanges();
//        var newSchedule = Controller.GetSchedule(vm.Schedule.Guid);

//        if (newSchedule != null)
//        {
//          CurrentInfoHolder.Settings.AddRecentlyOpenedFile(vm.Schedule.Guid, vm.Schedule.Name);
//          var newVM = new ScheduleInfoVM(newSchedule);
//          Schedules.Add(newVM);
//          CurrentSchedule = newVM;
//        }
//        else
//        {
//          NotificationService.Instance.Broadcast(NASResources.MessageScheduleNotFound, MessageCategory.Error);
//        }
//      }
//    }

//    private bool NewScheduleCommandCanExecute => !_isBusy;

//    #endregion

//    #region Open Schedule

//    public ICommand OpenScheduleCommand { get; }

//    private void OpenScheduleCommandExecute(object param)
//    {
//      var schedule = CurrentSchedule;

//      if (param != null && param is Guid guid)
//      {
//        schedule = Schedules.FirstOrDefault(x => x.Guid == guid);
//        if (schedule == null)
//        {
//          NotificationService.Instance.Broadcast(NASResources.MessageScheduleNotFound, MessageCategory.Error);
//          CurrentInfoHolder.Settings.RemoveRecentlyOpenedFile((Guid)param);
//          return;
//        }
//      }

//      CurrentSchedule = schedule;
//      OpenSchedule?.Invoke(this, new ItemEventArgs<Guid>(schedule.Guid));

//      CurrentInfoHolder.Settings.AddRecentlyOpenedFile(schedule.Guid, schedule.Name);
//      using (var uc = new UsersController())
//      {
//        uc.UpdateUserSettings(CurrentInfoHolder.CurrentUser);
//      }

//      CloseBackstage?.Invoke(this, EventArgs.Empty);
//      RequestSelectFirstTab?.Invoke(this, EventArgs.Empty);
//    }

//    private bool OpenScheduleCommandCanExecute(object param)
//    {
//      return !_isBusy && (param is Guid || CurrentSchedule != null);
//    }

//    #endregion

//    #region Remove Schedule

//    public ICommand RemoveScheduleCommand { get; }

//    private void RemoveScheduleCommandExecute()
//    {
//      UserNotificationService.Instance.Question(NASResources.MessageDeleteSchedule, () =>
//      {
//        Controller.DeleteSchedule(CurrentSchedule.Schedule);
//        Schedules.Remove(CurrentSchedule);
//        CurrentSchedule = null;
//        Controller.SaveChanges();
//      });
//    }

//    private bool RemoveScheduleCommandCanExecute => !_isBusy && CurrentSchedule != null;

//    #endregion

//    #region Import Schedule

//    public ICommand ImportScheduleCommand { get; }

//    internal void ImportScheduleCommandExecute(object param)
//    {
//      var filters = FilterFactory.Filters;
//      string fileName = null;
//      if (param == null)
//      {
//        var openFileDialog = new OpenFileDialog();
//        string f = "";
//        foreach (IFilter filter in filters.OfType<IImportFilter>())
//        {
//          if (!string.IsNullOrEmpty(f))
//          {
//            f += "|";
//          }

//          f += filter.FilterName + "|*." + filter.FileExtension;
//        }
//        openFileDialog.Filter = f;
//        openFileDialog.AddExtension = true;
//        openFileDialog.CheckFileExists = true;
//        if (openFileDialog.ShowDialog() == true)
//        {
//          fileName = openFileDialog.FileName;
//        }
//        else
//        {
//          return;
//        }
//      }
//      else
//      {
//        fileName = param.ToString();
//      }

//      if (!File.Exists(fileName))
//      {
//        UserNotificationService.Instance.Error(string.Format(NASResources.MessageFileNotFound, fileName));
//        return;
//      }
//      try
//      {
//        string ext = Path.GetExtension(fileName).ToLower();
//        foreach (var filter in filters.OfType<IImportFilter>())
//        {
//          if ("." + filter.FileExtension.ToLower() == ext)
//          {
//            Controller.ImportMessages += Controller_ImportMessages;
//            var schedule = Controller.ImportSchedule(filter, fileName);
//            Controller.SaveChanges();
//            var newVM = new ScheduleInfoVM(schedule);
//            Schedules.Add(newVM);
//            CurrentSchedule = newVM;
//            CurrentInfoHolder.Settings.AddRecentlyOpenedFile(schedule.Guid, schedule.Name);
//            Controller.ImportMessages -= Controller_ImportMessages;
//            return;
//          }
//        }
//      }
//      catch (Exception ex)
//      {
//        UserNotificationService.Instance.Error(ex.Message);
//      }
//    }

//    private void Controller_ImportMessages(object sender, ItemEventArgs<string> e)
//    {
//      UserNotificationService.Instance.Error(e.Item);
//    }

//    private bool ImportScheduleCommandCanExecute => !_isBusy;

//    #endregion

//    #region Export Schedule

//    public ICommand ExportScheduleCommand { get; }

//    private void ExportScheduleCommandExecute()
//    {
//      var filters = FilterFactory.Filters.OfType<IExportFilter>();
//      var saveFileDialog = new SaveFileDialog();
//      string f = "";
//      foreach (IFilter filter in filters)
//      {
//        if (!string.IsNullOrWhiteSpace(f))
//        {
//          f += "|";
//        }

//        f += filter.FilterName + "|*." + filter.FileExtension;
//      }
//      saveFileDialog.Filter = f;
//      saveFileDialog.AddExtension = true;
//      if (saveFileDialog.ShowDialog() == true)
//      {
//        string fileName = saveFileDialog.FileName;
//        try
//        {
//          string ext = Path.GetExtension(fileName).ToLower();
//          foreach (var filter in filters)
//          {
//            if ("." + filter.FileExtension.ToLower() == ext)
//            {
//              NASController.ExportSchedule(CurrentSchedule.Schedule, filter, fileName);
//            }
//          }
//        }
//        catch (Exception ex)
//        {
//          UserNotificationService.Instance.Error(ex.Message);
//        }
//      }
//    }

//    private bool ExportScheduleCommandCanExecute => !_isBusy && CurrentSchedule != null;

//    #endregion

//    #region Internal Methods

//    internal void RefreshCommands(bool isBusy)
//    {
//      _isBusy = isBusy;
//      (NewScheduleCommand as ActionCommand).RaiseCanExecuteChanged();
//      (OpenScheduleCommand as ActionCommand).RaiseCanExecuteChanged();
//      (RemoveScheduleCommand as ActionCommand).RaiseCanExecuteChanged();
//      (ImportScheduleCommand as ActionCommand).RaiseCanExecuteChanged();
//      (ExportScheduleCommand as ActionCommand).RaiseCanExecuteChanged();
//    }

//    #endregion
//  }
//}
