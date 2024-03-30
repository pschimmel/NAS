using System.Collections.ObjectModel;
using System.Windows.Input;
using ES.Tools.Core.MVVM;
using NAS.Model.Entities;
using NAS.Resources;
using NAS.ViewModel.Base;
using NAS.ViewModel.Helpers;

namespace NAS.ViewModel
{
  public class EditCalendarViewModel : DialogContentViewModel
  {
    #region Fields

    private readonly Calendar _calendar;
    private Holiday _currentHoliday;

    #endregion

    #region Constructor

    public EditCalendarViewModel(Calendar calendar)
      : base()
    {
      _calendar = calendar;
      Holidays = new ObservableCollection<Holiday>(calendar.Holidays);
      Name = calendar.Name;
      //GlobalCalendars = Controller.GetGlobalCalendars();
      AddHolidayCommand = new ActionCommand(AddHolidayCommandExecute);
      RemoveHolidayCommand = new ActionCommand(RemoveHolidayCommandExecute, () => RemoveHolidayCommandCanExecute);
    }

    #endregion

    #region Overwritten Members

    public override string Title => NASResources.Calendar;

    public override string Icon => "Calendar";

    public override DialogSize DialogSize => DialogSize.Fixed(450, 325);

    #endregion

    #region Public Members

    public ObservableCollection<Holiday> Holidays;

    public bool IsGlobal => _calendar.IsGlobal;

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

    #endregion

    #region Add Holiday

    public ICommand AddHolidayCommand { get; }

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

    #region Remove Holiday

    public ICommand RemoveHolidayCommand { get; }

    private void RemoveHolidayCommandExecute()
    {
      _ = Holidays.Remove(CurrentHoliday);
    }

    private bool RemoveHolidayCommandCanExecute => CurrentHoliday != null;

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
      }
    }

    #endregion
  }
}
