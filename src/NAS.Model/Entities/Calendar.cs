using System.Collections.ObjectModel;
using NAS.Resources;

namespace NAS.Model.Entities
{
  public class Calendar : NASObject
  {
    private string _name;
    private bool _monday;
    private bool _tuesday;
    private bool _wednesday;
    private bool _thursday;
    private bool _friday;
    private bool _saturday;
    private bool _sunday;
    private bool _isStandard;
    private Calendar _baseCalendar;

    public Calendar()
      : this(false)
    { }

    public Calendar(bool isBase)
    {
      Holidays = [];
      _name = NASResources.NewCalendar;
      _monday = true;
      _tuesday = true;
      _wednesday = true;
      _thursday = true;
      _friday = true;
      _saturday = false;
      _sunday = false;
      IsGlobal = isBase;
    }

    public Calendar(Calendar calendar)
      : this()
    {
      CopyCalendar(this, calendar);
    }

    public string Name
    {
      get => _name;
      set
      {
        if (_name != value)
        {
          _name = value;
          OnPropertyChanged(nameof(Name));
          OnPropertyChanged(nameof(FullName));
        }
      }
    }

    public bool Monday
    {
      get => _monday;
      set
      {
        if (_monday != value)
        {
          _monday = value;
          OnPropertyChanged(nameof(Monday));
        }
      }
    }

    public bool Tuesday
    {
      get => _tuesday;
      set
      {
        if (_tuesday != value)
        {
          _tuesday = value;
          OnPropertyChanged(nameof(Tuesday));
        }
      }
    }

    public bool Wednesday
    {
      get => _wednesday;
      set
      {
        if (_wednesday != value)
        {
          _wednesday = value;
          OnPropertyChanged(nameof(Wednesday));
        }
      }
    }

    public bool Thursday
    {
      get => _thursday;
      set
      {
        if (_thursday != value)
        {
          _thursday = value;
          OnPropertyChanged(nameof(Thursday));
        }
      }
    }

    public bool Friday
    {
      get => _friday;
      set
      {
        if (_friday != value)
        {
          _friday = value;
          OnPropertyChanged(nameof(Friday));
        }
      }
    }

    public bool Saturday
    {
      get => _saturday;
      set
      {
        if (_saturday != value)
        {
          _saturday = value;
          OnPropertyChanged(nameof(Saturday));
        }
      }
    }

    public bool Sunday
    {
      get => _sunday;
      set
      {
        if (_sunday != value)
        {
          _sunday = value;
          OnPropertyChanged(nameof(Sunday));
        }
      }
    }

    public bool IsStandard
    {
      get => _isStandard;
      set
      {
        if (_isStandard != value)
        {
          _isStandard = value;
          OnPropertyChanged(nameof(IsStandard));
          OnPropertyChanged(nameof(FullName));
        }
      }
    }

    public string FullName => IsStandard ? Name + " (" + NASResources.Standard + ")" : Name;

    public ObservableCollection<Holiday> Holidays { get; }

    public bool IsGlobal { get; internal set; }

    public Calendar BaseCalendar
    {
      get => _baseCalendar;
      set
      {
        if (_baseCalendar != value)
        {
          _baseCalendar = value;
          OnPropertyChanged(nameof(BaseCalendar));
        }
      }
    }

    public void ReplaceHolidays(IEnumerable<Holiday> newHolidays)
    {
      if (newHolidays == null)
      {
        throw new ArgumentNullException(nameof(newHolidays), "Argument can't be null");
      }

      Holidays.Clear();

      foreach (var holiday in newHolidays)
      {
        Holidays.Add(holiday);
      }
    }

    /// <summary>
    /// Returns if the given day is activity work day
    /// </summary>
    /// <param _number="day">The day which shall be checked.</param>
    /// <returns>True if day is workday</returns>
    public bool IsWorkDay(DateTime day)
    {
      return IsWorkDayInternal(day) && !IsHoliday(day);
    }

    /// <summary>
    /// Returns the end day of activity work period
    /// </summary>
    /// <param _number="startDate">Start day of the period</param>
    /// <param _number="duration">The _number of work duration between _start and end.</param>
    /// <returns>End day</returns>
    public DateTime GetEndDate(DateTime startDate, int duration)
    {
      if (startDate == DateTime.MaxValue)
      {
        return startDate;
      }

      int sign = 1;
      if (duration < 0)
      {
        sign = -1;
        duration = -duration;
      }
      duration--;
      var result = startDate;
      if (duration > 0 && GetWorkingDaysInWeek() > 0)
      {
        int days = (int)Math.Floor(duration / (double)GetWorkingDaysInWeek()) * 7;
        result = result.AddDays(sign * days);
        duration -= (int)Math.Floor(duration / (double)GetWorkingDaysInWeek()) * GetWorkingDaysInWeek();
      }
      var holidayList = Holidays.ToList().Select(x => x.Date).ToList();
      if (BaseCalendar != null)
      {
        holidayList.AddRange(BaseCalendar.Holidays.Where(x => !holidayList.Contains(x.Date)).Select(x => x.Date));
      }

      foreach (var d in holidayList)
      {
        if (IsWorkDayInternal(d))
        {
          if (sign == 1 && d >= startDate && d <= result || sign == -1 && d >= result && d <= startDate)
          {
            result = result.AddDays(sign);
            if (d != result && !IsWorkDay(result))
            {
              result = result.AddDays(sign);
            }
          }
        }
      }
      while (duration > 0)
      {
        if (IsWorkDay(result))
        {
          duration--;
        }

        if (result != DateTime.MinValue && result != DateTime.MaxValue)
        {
          result = result.AddDays(sign);
        }
      }
      // If result is Holiday go to next working day
      while (!IsWorkDay(result))
      {
        result = result.AddDays(sign);
      }

      return result;
    }

    /// <summary>
    /// Returns the end day of activity work period
    /// </summary>
    /// <param _number="endDate">End day of the period</param>
    /// <param _number="duration">The _number of work duration between _start and end.</param>
    /// <returns>End day</returns>
    public DateTime GetStartDate(DateTime endDate, int duration)
    {
      return endDate == DateTime.MinValue ? endDate : GetEndDate(endDate, -duration);
    }

    /// <summary>
    /// Returns the duration (work duration) between two dates
    /// </summary>
    /// <param _number="startDate">Start day of the period</param>
    /// <param _number="endDate">End day of the period</param>
    /// <returns>Work duration.</returns>
    public int GetWorkDays(DateTime startDate, DateTime endDate, bool includeEndDate)
    {
      int sign = 1;
      if (endDate < startDate)
      {
        (endDate, startDate) = (startDate, endDate);
        sign = -1;
      }
      int days = 0;
      days = (int)(Math.Floor((endDate - startDate).TotalDays / 7) * GetWorkingDaysInWeek());
      var holidayList = Holidays.ToList().Select(x => x.Date).ToList();
      if (BaseCalendar != null)
      {
        holidayList.AddRange(BaseCalendar.Holidays.Where(x => !holidayList.Contains(x.Date)).Select(x => x.Date));
      }

      foreach (var d in holidayList)
      {
        if (d >= startDate && d <= endDate)
        {
          days--;
        }
      }
      startDate = startDate.AddDays((int)(days * (double)7 / GetWorkingDaysInWeek()));
      while (startDate < endDate)
      {
        if (IsWorkDay(startDate))
        {
          days++;
        }

        startDate = startDate.AddDays(1);
      }
      if (includeEndDate)
      {
        while (startDate <= endDate)
        {
          if (IsWorkDay(endDate))
          {
            days++;
          }

          startDate = startDate.AddDays(1);
        }
      }
      return days * sign;
    }

    /// <summary>
    /// Returns activity <see cref="string"/> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="string"/> that represents this instance.
    /// </returns>
    public override string ToString()
    {
      return Name;
    }

    #region Private Members

    private bool IsHoliday(DateTime day)
    {
      return BaseCalendar != null && BaseCalendar.Holidays.Any(x => x.Date == day.Date)
            || Holidays != null && Holidays.Any(x => x.Date == day.Date);
    }

    public static void CopyCalendar(Calendar newCalendar, Calendar oldCalendar)
    {
      if (newCalendar.IsGlobal != oldCalendar.IsGlobal)
      {
        throw new ArgumentException("The calendars have different levels.");
      }

      newCalendar.Name = oldCalendar.Name + " (" + NASResources.Copy + ")";
      newCalendar.Monday = oldCalendar.Monday;
      newCalendar.Tuesday = oldCalendar.Tuesday;
      newCalendar.Wednesday = oldCalendar.Wednesday;
      newCalendar.Thursday = oldCalendar.Thursday;
      newCalendar.Friday = oldCalendar.Friday;
      newCalendar.Saturday = oldCalendar.Saturday;
      newCalendar.Sunday = oldCalendar.Sunday;

      if (oldCalendar.Holidays != null)
      {
        foreach (var h in oldCalendar.Holidays)
        {
          newCalendar.Holidays.Add(new Holiday() { Date = h.Date });
        }
      }
    }

    private bool IsWorkDayInternal(DateTime day)
    {
      return day.DayOfWeek switch
      {
        DayOfWeek.Sunday => Sunday,
        DayOfWeek.Monday => Monday,
        DayOfWeek.Tuesday => Tuesday,
        DayOfWeek.Wednesday => Wednesday,
        DayOfWeek.Thursday => Thursday,
        DayOfWeek.Friday => Friday,
        DayOfWeek.Saturday => Saturday,
        _ => true,
      };
    }

    private int GetWorkingDaysInWeek()
    {
      int result = 0;
      if (Monday)
      {
        result++;
      }

      if (Tuesday)
      {
        result++;
      }

      if (Wednesday)
      {
        result++;
      }

      if (Thursday)
      {
        result++;
      }

      if (Friday)
      {
        result++;
      }

      if (Saturday)
      {
        result++;
      }

      if (Sunday)
      {
        result++;
      }

      return result;
    }

    #endregion
  }
}
