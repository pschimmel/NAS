using System.Collections.ObjectModel;
using System.Windows.Input;
using ES.Tools.Core.MVVM;
using NAS.Models.Entities;
using NAS.Resources;
using NAS.ViewModels.Base;
using NAS.ViewModels.Helpers;

namespace NAS.ViewModels
{
  public class EditCalendarViewModel : DialogContentViewModel
  {
    #region Fields

    private readonly Calendar _calendar;
    private Holiday _currentHoliday;
    private ActionCommand _addHolidayCommand;
    private ActionCommand _editHolidayCommand;
    private ActionCommand _removeHolidayCommand;

    #endregion

    #region Constructor

    public EditCalendarViewModel(Calendar calendar, IEnumerable<Calendar> globalCalendars)
      : base()
    {
      _calendar = calendar;
      Holidays = new ObservableCollection<Holiday>(calendar.Holidays);
      Name = calendar.Name;
      GlobalCalendars = new List<Calendar>(globalCalendars);

      if (calendar.BaseCalendar != null && !GlobalCalendars.Contains(calendar.BaseCalendar))
      {
        GlobalCalendars.Add(calendar.BaseCalendar);
        BaseCalendar = calendar.BaseCalendar;
      }
    }

    #endregion

    #region Overwritten Members

    public override string Title => NASResources.Calendar;

    public override string Icon => "Calendar";

    public override DialogSize DialogSize => DialogSize.Fixed(450, 325);

    #endregion

    #region Properties

    public bool IsGlobal => _calendar.IsGlobal;

    public ObservableCollection<Holiday> Holidays { get; }

    public List<Calendar> GlobalCalendars { get; }

    public Holiday CurrentHoliday
    {
      get => _currentHoliday;
      set
      {
        if (_currentHoliday != value)
        {
          _currentHoliday = value;
          OnPropertyChanged(nameof(CurrentHoliday));
        }
      }
    }

    public string Name { get; set; }

    public bool Monday { get; set; }

    public bool Tuesday { get; set; }

    public bool Wednesday { get; set; }

    public bool Thursday { get; set; }

    public bool Friday { get; set; }

    public bool Saturday { get; set; }

    public bool Sunday { get; set; }

    public Calendar BaseCalendar { get; set; }

    #endregion

    #region Add Holiday

    public ICommand AddHolidayCommand => _addHolidayCommand ??= new ActionCommand(AddHolidayCommandExecute);

    private void AddHolidayCommandExecute()
    {
      using var vm = new GetDateViewModel(NASResources.PleaseEnterDate, DateTime.Today);

      if (ViewFactory.Instance.ShowDialog(vm) == true)
      {
        var newHoliday = new Holiday() { Date = vm.Date };
        Holidays.Add(newHoliday);
        CurrentHoliday = newHoliday;
      }
    }

    #endregion

    #region Edit Holiday

    public ICommand EditHolidayCommand => _editHolidayCommand ??= new ActionCommand(EditHoliday, CanEditHoliday);

    private void EditHoliday()
    {
      using var vm = new GetDateViewModel(NASResources.PleaseEnterDate, CurrentHoliday.Date);

      if (ViewFactory.Instance.ShowDialog(vm) == true)
      {
        CurrentHoliday.Date = vm.Date;
        // To update view remove and re-insert date
        var idx = Holidays.IndexOf(CurrentHoliday);
        Holidays.Remove(CurrentHoliday);
        // Execute on UI thread
        ES.Tools.UI.DispatcherWrapper.Default.BeginInvokeIfRequired(() =>
        {
          Holidays.Insert(idx, CurrentHoliday);
        });
      }
    }

    private bool CanEditHoliday()
    {
      return CurrentHoliday != null;
    }

    #endregion

    #region Remove Holiday

    public ICommand RemoveHolidayCommand => _removeHolidayCommand ??= new ActionCommand(RemoveHoliday, CanRemoveHoliday);

    private void RemoveHoliday()
    {
      Holidays.Remove(CurrentHoliday);
    }

    private bool CanRemoveHoliday()
    {
      return CurrentHoliday != null;
    }

    #endregion

    #region Validation

    protected override ValidationResult OnValidating()
    {
      return string.IsNullOrWhiteSpace(Name)
             ? ValidationResult.Error(NASResources.PleaseEnterName)
             : ValidationResult.OK();
    }

    #endregion

    #region Apply

    protected override void OnApply()
    {
      if (Validate().IsOK)
      {
        _calendar.Name = Name;
        _calendar.ReplaceHolidays(Holidays);
        _calendar.Monday = Monday;
        _calendar.Tuesday = Tuesday;
        _calendar.Wednesday = Wednesday;
        _calendar.Thursday = Thursday;
        _calendar.Friday = Friday;
        _calendar.Saturday = Saturday;
        _calendar.Sunday = Sunday;

        if (!IsGlobal)
        {
          _calendar.BaseCalendar = BaseCalendar;
        }
      }
    }

    #endregion
  }
}
