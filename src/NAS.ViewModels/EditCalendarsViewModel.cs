using System.Collections.ObjectModel;
using System.Windows.Input;
using ES.Tools.Core.MVVM;
using NAS.Models.Controllers;
using NAS.Models.Entities;
using NAS.Resources;
using NAS.ViewModels.Base;
using NAS.ViewModels.Helpers;

namespace NAS.ViewModels
{
  public class EditCalendarsViewModel : DialogContentViewModel
  {
    #region Fields

    private readonly Schedule _schedule;
    private Calendar _currentCalendar;
    private Calendar _currentGlobalCalendar;
    private ActionCommand _addGlobalCalendarCommand;
    private ActionCommand _removeGlobalCalendarCommand;
    private ActionCommand _editGlobalCalendarCommand;
    private ActionCommand _addCalendarCommand;
    private ActionCommand _removeCalendarCommand;
    private ActionCommand _copyCalendarCommand;
    private ActionCommand _setStandardCalendarCommand;
    private ActionCommand _editCalendarCommand;

    #endregion

    #region Constructor

    public EditCalendarsViewModel(Schedule schedule)
      : base()
    {
      _schedule = schedule;
      Calendars = new ObservableCollection<Calendar>(schedule.Calendars);
      GlobalCalendars = new ObservableCollection<Calendar>(GlobalDataController.Calendars);
    }

    #endregion

    #region Overwritten Members

    public override string Title => NASResources.EditCalendars;

    public override string Icon => "Calendar";

    public override DialogSize DialogSize => DialogSize.Fixed(300, 300);

    public override HelpTopic HelpTopicKey => HelpTopic.Calendars;

    #endregion

    #region Public Members

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

    public ObservableCollection<Calendar> GlobalCalendars { get; }

    public Calendar CurrentGlobalCalendar
    {
      get => _currentGlobalCalendar;
      set
      {
        if (_currentGlobalCalendar != value)
        {
          _currentGlobalCalendar = value;
          OnPropertyChanged(nameof(CurrentGlobalCalendar));
        }
      }
    }

    #endregion

    #region Add Global Calendar

    public ICommand AddGlobalCalendarCommand => _addGlobalCalendarCommand ??= new ActionCommand(AddGlobalCalendar, CanAddGlobalCalendar);

    private void AddGlobalCalendar()
    {
      var calendar = new Calendar(true);
      using var vm = new EditCalendarViewModel(calendar, GlobalCalendars);
      if (ViewFactory.Instance.ShowDialog(vm) == true)
      {
        GlobalCalendars.Add(calendar);
        CurrentGlobalCalendar = calendar;
      }
    }

    private bool CanAddGlobalCalendar()
    {
      return true;
    }

    #endregion

    #region Remove Global Calendar

    public ICommand RemoveGlobalCalendarCommand => _removeGlobalCalendarCommand ??= new ActionCommand(RemoveGlobalCalendar, CanRemoveGlobalCalendar);

    private void RemoveGlobalCalendar()
    {
      UserNotificationService.Instance.Question(NASResources.MessageDeleteCalendar, () =>
      {
        GlobalCalendars.Remove(CurrentGlobalCalendar);
        CurrentGlobalCalendar = null;
      });
    }

    private bool CanRemoveGlobalCalendar()
    {
      return CurrentGlobalCalendar != null;
    }

    #endregion

    #region Edit Global Calendar

    public ICommand EditGlobalCalendarCommand => _editGlobalCalendarCommand ??= new ActionCommand(EditGlobalCalendar, CanEditGlobalCalendar);

    private void EditGlobalCalendar()
    {
      using var vm = new EditCalendarViewModel(CurrentGlobalCalendar, GlobalCalendars);
      ViewFactory.Instance.ShowDialog(vm);
    }

    private bool CanEditGlobalCalendar()
    {
      return CurrentGlobalCalendar != null;
    }

    #endregion

    #region Add Calendar

    public ICommand AddCalendarCommand => _addCalendarCommand ??= new ActionCommand(AddCalendar);

    private void AddCalendar()
    {
      var newCalendar = new Calendar();
      using var vm = new EditCalendarViewModel(newCalendar, GlobalCalendars);
      if (ViewFactory.Instance.ShowDialog(vm) == true)
      {
        Calendars.Add(newCalendar);
        CurrentCalendar = newCalendar;
      }
    }

    #endregion

    #region Remove Calendar

    public ICommand RemoveCalendarCommand => _removeCalendarCommand ??= new ActionCommand(RemoveCalendar, CanRemoveCalendar);

    private void RemoveCalendar()
    {
      if (!_schedule.CanRemoveCalendar(CurrentCalendar))
      {
        UserNotificationService.Instance.Error(NASResources.MessageCannotRemoveCalendar);
        return;
      }

      UserNotificationService.Instance.Question(NASResources.MessageDeleteCalendar, () =>
      {
        Calendars.Remove(CurrentCalendar);
        CurrentCalendar = null;
        if (!_schedule.Calendars.Any(x => x.IsStandard))
        {
          _schedule.Calendars.First().IsStandard = true;
        }
      });
    }

    private bool CanRemoveCalendar()
    {
      return CurrentCalendar != null && Calendars.Count > 1 && !CurrentCalendar.IsStandard;
    }

    #endregion

    #region Edit Calendar

    public ICommand EditCalendarCommand => _editCalendarCommand ??= new ActionCommand(EditCalendar, CanEditCalendar);

    private void EditCalendar()
    {
      using var vm = new EditCalendarViewModel(CurrentCalendar, GlobalCalendars);
      ViewFactory.Instance.ShowDialog(vm);
    }

    private bool CanEditCalendar()
    {
      return CurrentCalendar != null;
    }

    #endregion

    #region Copy Calendar

    public ICommand CopyCalendarCommand => _copyCalendarCommand ??= new ActionCommand(CopyCalendar, CanCopyCalendar);

    private void CopyCalendar()
    {
      var newCalendar = new Calendar(CurrentCalendar);
      _schedule.Calendars.Add(newCalendar);
      CurrentCalendar = newCalendar;
    }

    private bool CanCopyCalendar()
    {
      return CurrentCalendar != null;
    }

    #endregion

    #region Set Standard Calendar

    public ICommand SetStandardCalendarCommand => _setStandardCalendarCommand ??= new ActionCommand(SetStandardCalendar, CanSetStandardCalendar);

    private void SetStandardCalendar()
    {
      _schedule.StandardCalendar = CurrentCalendar;
    }

    private bool CanSetStandardCalendar()
    {
      return CurrentCalendar != null && !CurrentCalendar.IsStandard;
    }

    #endregion

    #region Apply

    protected override void OnApply()
    {
      base.OnApply();
      GlobalDataController.Calendars.Clear();
      GlobalDataController.Calendars.AddRange(GlobalCalendars);
      GlobalDataController.SaveCalendars();

      _schedule.Calendars.Clear();

      foreach (var calendar in Calendars)
      {
        _schedule.Calendars.Add(calendar);
      }

      if (!_schedule.Calendars.Any(x => x.IsStandard))
      {
        _schedule.Calendars.First().IsStandard = true;
      }
    }

    #endregion
  }
}
