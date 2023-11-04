using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using ES.Tools.Core.MVVM;
using NAS.Model.Entities;
using NAS.Resources;
using NAS.ViewModel.Base;
using NAS.ViewModel.Helpers;

namespace NAS.ViewModel
{
  public class CalendarsViewModel : ViewModelBase, IApplyable
  {
    #region Fields

    private readonly Schedule _schedule;
    private Calendar _currentCalendar;
    //private BaseCalendar currentGlobalCalendar;

    #endregion

    #region Constructor

    public CalendarsViewModel(Schedule schedule)
      : base()
    {
      _schedule = schedule;
      Calendars = new ObservableCollection<Calendar>(schedule.Calendars);
      //GlobalCalendars = new ObservableCollection<BaseCalendar>(controller.GetGlobalCalendars());
      //AddGlobalCalendarCommand = new ActionCommand(AddGlobalCalendarCommandExecute, () => AddGlobalCalendarCommandCanExecute);
      //RemoveGlobalCalendarCommand = new ActionCommand(RemoveGlobalCalendarCommandExecute, () => RemoveGlobalCalendarCommandCanExecute);
      //EditGlobalCalendarCommand = new ActionCommand(EditGlobalCalendarCommandExecute, () => EditGlobalCalendarCommandCanExecute);
      AddCalendarCommand = new ActionCommand(AddCalendarCommandExecute);
      RemoveCalendarCommand = new ActionCommand(RemoveCalendarCommandExecute, () => RemoveCalendarCommandCanExecute);
      CopyCalendarCommand = new ActionCommand(CopyCalendarCommandExecute, () => CopyCalendarCommandCanExecute);
      SetStandardCalendarCommand = new ActionCommand(SetStandardCalendarCommandExecute, () => SetStandardCalendarCommandCanExecute);
      EditCalendarCommand = new ActionCommand(EditCalendarCommandExecute, () => EditCalendarCommandCanExecute);
    }

    #endregion

    #region Public Members

    public override HelpTopic HelpTopicKey => HelpTopic.Calendars;

    public ObservableCollection<Calendar> Calendars { get; }

    public Calendar CurrentCalendar
    {
      get => _currentCalendar;
      set
      {
        if (_currentCalendar != value)
        {
          _currentCalendar = value;
          OnPropertyChanged(nameof(CurrentCalendar));
        }
      }
    }

    //public ObservableCollection<BaseCalendar> GlobalCalendars { get; }

    //public BaseCalendar CurrentGlobalCalendar
    //{
    //  get => currentGlobalCalendar;
    //  set
    //  {
    //    if (currentGlobalCalendar != value)
    //    {
    //      currentGlobalCalendar = value;
    //      OnPropertyChanged(nameof(CurrentGlobalCalendar));
    //    }
    //  }
    //}

    #endregion

    //#region Add Global Calendar

    //public ICommand AddGlobalCalendarCommand { get; }

    //private void AddGlobalCalendarCommandExecute()
    //{
    //  var item = Controller.AddGlobalCalendar();
    //  var vm = new BaseCalendarViewModel(item);
    //  if (ViewFactory.Instance.ShowDialog(vm) != true)
    //  {
    //    Controller.DeleteBaseCalendar(item);
    //  }
    //  else
    //  {
    //    GlobalCalendars.Add(item);
    //    CurrentGlobalCalendar = item;
    //    SaveChanges();
    //  }
    //}

    //private bool AddGlobalCalendarCommandCanExecute => canEditGlobalCalendars;

    //#endregion

    //#region Remove Global Calendar

    //public ICommand RemoveGlobalCalendarCommand { get; }

    //private void RemoveGlobalCalendarCommandExecute()
    //{
    //  if (!Controller.CanRemoveBaseCalendar(CurrentGlobalCalendar))
    //  {
    //    UserNotificationService.Instance.Error(NASResources.MessageCannotRemoveBaseCalendar);
    //    return;
    //  }

    //  UserNotificationService.Instance.Question(NASResources.MessageDeleteCalendar, () =>
    //  {
    //    Controller.DeleteBaseCalendar(CurrentGlobalCalendar);
    //    GlobalCalendars.Remove(CurrentGlobalCalendar);
    //    Controller.SaveChanges();
    //  });
    //}

    //private bool RemoveGlobalCalendarCommandCanExecute => canEditGlobalCalendars && CurrentGlobalCalendar != null;

    //#endregion

    //#region Edit Global Calendar

    //public ICommand EditGlobalCalendarCommand { get; }

    //private void EditGlobalCalendarCommandExecute()
    //{
    //  var vm = new BaseCalendarViewModel(CurrentGlobalCalendar);
    //  if (ViewFactory.Instance.ShowDialog(vm) == true)
    //  {
    //    SaveChanges();
    //  }
    //  else
    //  {
    //    Controller.RefreshBaseCalendar(CurrentGlobalCalendar);
    //  }
    //}

    //private bool EditGlobalCalendarCommandCanExecute => canEditGlobalCalendars && CurrentGlobalCalendar != null;

    //#endregion

    #region Add Calendar

    public ICommand AddCalendarCommand { get; }

    private void AddCalendarCommandExecute()
    {
      var newCalendar = new Calendar();
      using var vm = new CalendarViewModel(newCalendar);
      if (ViewFactory.Instance.ShowDialog(vm) == true)
      {
        Calendars.Add(newCalendar);
        CurrentCalendar = newCalendar;
      }
    }

    #endregion

    #region Remove Calendar

    public ICommand RemoveCalendarCommand { get; }

    private void RemoveCalendarCommandExecute()
    {
      if (!_schedule.CanRemoveCalendar(CurrentCalendar))
      {
        UserNotificationService.Instance.Error(NASResources.MessageCannotRemoveCalendar);
        return;
      }

      UserNotificationService.Instance.Question(NASResources.MessageDeleteCalendar, () =>
      {
        var calendarToDelete = CurrentCalendar;
        CurrentCalendar = null;
        Calendars.Remove(calendarToDelete);
        if (!_schedule.Calendars.Any(x => x.IsStandard))
        {
          _schedule.Calendars.First().IsStandard = true;
        }
      });
    }

    private bool RemoveCalendarCommandCanExecute => CurrentCalendar != null && Calendars.Count > 1 && !CurrentCalendar.IsStandard;

    #endregion

    #region Copy Calendar

    public ICommand CopyCalendarCommand { get; }

    private void CopyCalendarCommandExecute()
    {
      var newCalendar = new Calendar(CurrentCalendar);
      _schedule.Calendars.Add(newCalendar);
      CurrentCalendar = newCalendar;
    }

    private bool CopyCalendarCommandCanExecute => CurrentCalendar != null;

    #endregion

    #region Set Standard Calendar

    public ICommand SetStandardCalendarCommand { get; }

    private void SetStandardCalendarCommandExecute()
    {
      _schedule.StandardCalendar = CurrentCalendar;
    }

    private bool SetStandardCalendarCommandCanExecute => CurrentCalendar != null && !CurrentCalendar.IsStandard;

    #endregion

    #region Edit Calendar

    public ICommand EditCalendarCommand { get; }

    private void EditCalendarCommandExecute()
    {
      using var vm = new CalendarViewModel(CurrentCalendar);
      if (ViewFactory.Instance.ShowDialog(vm) == true)
      {
        vm.Apply();
      }
    }

    private bool EditCalendarCommandCanExecute => CurrentCalendar != null;

    #endregion

    #region IApplyableViewModel implementation

    public void Apply()
    {
      _schedule.RefreshCalendars(Calendars);
    }

    #endregion
  }
}
