using System.Collections.ObjectModel;
using System.Diagnostics;
using NAS.Models.Enums;
using NAS.Resources;

namespace NAS.Models.Entities
{
  [DebuggerDisplay("{Number} {Name}")]
  public class Activity : NASObject
  {
    #region Fields

    private string _number;
    private string _name;
    private DateTime _earlyStartDate;
    private DateTime _earlyFinishDate;
    private DateTime _lateStartDate;
    private DateTime _lateFinishDate;
    private DateTime? _actualStartDate;
    private DateTime? _actualFinishDate;
    private int _originalDuration;
    private int _remainingDuration;
    protected double _percentComplete;
    private int _totalFloat;
    private int _freeFloat;
    private ConstraintType _constraintType;
    private DateTime? _constraintDate;
    private Calendar _calendar;
    private CustomAttribute _customAttribute1;
    private CustomAttribute _customAttribute2;
    private CustomAttribute _customAttribute3;
    private Fragnet _fragnet;
    private WBSItem _wbsItem;

    #endregion

    #region Constructors

    public Activity(bool isFixed = false)
    {
      IsFixed = isFixed;
      _constraintType = ConstraintType.None;
      _name = NASResources.NewActivity;
      _originalDuration = 5;
      Distortions = new ObservableCollection<Distortion>();
    }

    public Activity(Activity other)
    {
      _number = other._number;
      _name = other._name;
      _earlyStartDate = other._earlyStartDate;
      _earlyFinishDate = other._earlyFinishDate;
      _lateStartDate = other._lateStartDate;
      _lateFinishDate = other._lateFinishDate;
      _actualStartDate = other._actualStartDate;
      _actualFinishDate = other._actualFinishDate;
      _originalDuration = other._originalDuration;
      _remainingDuration = other._remainingDuration;
      _percentComplete = other._percentComplete;
      _totalFloat = other._totalFloat;
      _freeFloat = other._freeFloat;
      _constraintType = other._constraintType;
      _constraintDate = other._constraintDate;
      _calendar = other._calendar;
      _customAttribute1 = other._customAttribute1;
      _customAttribute2 = other._customAttribute2;
      _customAttribute3 = other._customAttribute3;
      _fragnet = other._fragnet;
      _wbsItem = other._wbsItem;
      IsFixed = other.IsFixed;
      Distortions = new ObservableCollection<Distortion>(other.Distortions.Select(x => x.Clone()));
    }

    #endregion

    #region Properties

    public string Number
    {
      get => _number;
      set
      {
        if (_number != value)
        {
          _number = value;
          OnPropertyChanged(nameof(Number));
        }
      }
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
        }
      }
    }

    public virtual ActivityType ActivityType => ActivityType.Activity;

    public bool IsFixed { get; }

    /// <summary>
    /// Is the activity started?
    /// </summary>
    public bool IsStarted => ActualStartDate.HasValue;

    /// <summary>
    /// Is the activity ended?
    /// </summary>
    public bool IsFinished => ActualFinishDate.HasValue;

    /// <summary>
    /// Is activity part of the critical path?
    /// </summary>
    public bool IsCritical { get; set; }

    #endregion

    #region Dates

    public DateTime EarlyStartDate
    {
      get => _earlyStartDate;
      set
      {
        if (_earlyStartDate != value)
        {
          _earlyStartDate = value;
          OnPropertyChanged(nameof(EarlyStartDate));
          if (Calendar != null)
          {
            EarlyFinishDate = Calendar.GetEndDate(EarlyStartDate, RemainingDuration);
          }
          OnPropertyChanged(nameof(StartDate));
        }
      }
    }

    public DateTime EarlyFinishDate
    {
      get => _earlyFinishDate;
      set
      {
        if (_earlyFinishDate != value)
        {
          _earlyFinishDate = value;
          OnPropertyChanged(nameof(EarlyFinishDate));
          if (Calendar != null)
          {
            EarlyStartDate = Calendar.GetStartDate(EarlyFinishDate, RemainingDuration);
          }
          OnPropertyChanged(nameof(FinishDate));
        }
      }
    }

    public DateTime LateStartDate
    {
      get => _lateStartDate;
      set
      {
        if (_lateStartDate != value)
        {
          _lateStartDate = value;
          OnPropertyChanged(nameof(LateStartDate));
          if (Calendar != null)
          {
            LateFinishDate = Calendar.GetEndDate(LateStartDate, RemainingDuration);
          }
        }
      }
    }

    public DateTime LateFinishDate
    {
      get => _lateFinishDate;
      set
      {
        if (_lateFinishDate != value)
        {
          _lateFinishDate = value;
          OnPropertyChanged(nameof(LateFinishDate));
          if (Calendar != null)
          {
            LateStartDate = Calendar.GetStartDate(LateFinishDate, RemainingDuration);
          }
        }
      }
    }

    public DateTime? ActualStartDate
    {
      get => _actualStartDate;
      set
      {
        if (_actualStartDate != value)
        {
          _actualStartDate = value;
          OnPropertyChanged(nameof(ActualStartDate));
          OnPropertyChanged(nameof(StartDate));
          OnPropertyChanged(nameof(ActualDuration));
        }
      }
    }

    public DateTime? ActualFinishDate
    {
      get => _actualFinishDate;
      set
      {
        if (_actualFinishDate != value)
        {
          _actualFinishDate = value;
          OnPropertyChanged(nameof(ActualFinishDate));
          OnPropertyChanged(nameof(FinishDate));
          OnPropertyChanged(nameof(ActualDuration));
        }
      }
    }

    public DateTime StartDate => ActualStartDate ?? EarlyStartDate;

    public DateTime FinishDate => ActualFinishDate ?? EarlyFinishDate;

    #endregion

    #region Durations

    public virtual int OriginalDuration
    {
      get => _originalDuration;
      set
      {
        if (_originalDuration != value)
        {
          _originalDuration = value;
          OnPropertyChanged(nameof(OriginalDuration));
          OnPropertyChanged(nameof(DelayedDuration));
          OnPropertyChanged(nameof(RemainingDuration));
          OnPropertyChanged(nameof(AtCompletionDuration));
          RemainingDuration = Convert.ToInt32(Convert.ToDouble(DelayedDuration) * (100d - PercentComplete) / 100d);
          if (Calendar != null)
          {
            EarlyFinishDate = Calendar.GetEndDate(EarlyStartDate, RemainingDuration);
          }
        }
      }
    }

    /// <summary>
    /// The duration until the activity ends
    /// </summary>
    public virtual int RemainingDuration
    {
      get => _remainingDuration;
      set
      {
        if (_remainingDuration != value)
        {
          _remainingDuration = value;
          PercentComplete = DelayedDuration != 0
            ? (Convert.ToDouble(DelayedDuration) - Convert.ToDouble(RemainingDuration)) * 100d / Convert.ToDouble(DelayedDuration)
            : 0;

          OnPropertyChanged(nameof(RemainingDuration));
          OnPropertyChanged(nameof(AtCompletionDuration));
        }
      }
    }

    /// <summary>
    /// The estimated duration until the activity ends
    /// </summary>
    public int AtCompletionDuration => ActualDuration + RemainingDuration;

    public virtual int DelayedDuration
    {
      get
      {
        int result = OriginalDuration;
        foreach (var d in Distortions)
        {
          if (d.Fragnet == null || d.Fragnet.IsVisible)
          {
            if (d is Delay && (d as Delay).Days.HasValue)
            {
              result += (d as Delay).Days.Value;
            }
            else if (d is Interruption && (d as Interruption).Days.HasValue)
            {
              result += (d as Interruption).Days.Value;
            }
            else if (d is Inhibition && (d as Inhibition).Percent.HasValue)
            {
              result += Convert.ToInt32(Math.Round(OriginalDuration * (d as Inhibition).Percent.Value / 100));
            }
            else if (d is Extension && (d as Extension).Days.HasValue)
            {
              result += (d as Extension).Days.Value;
            }
            else if (d is Reduction && (d as Reduction).Days.HasValue)
            {
              result -= (d as Reduction).Days.Value;
            }
          }
        }
        if (result < 1)
        {
          result = 1;
        }

        return result;
      }
    }

    public virtual int ActualDuration
    {
      get => ActualStartDate.HasValue
            ? ActualStartDate.HasValue && ActualFinishDate.HasValue && Calendar != null
              ? Calendar.GetWorkDays(ActualStartDate.Value, ActualFinishDate.Value, false)
              : DelayedDuration
            : 0;
    }

    #endregion

    #region Percentages

    public virtual double PercentComplete
    {
      get => _percentComplete;
      set
      {
        if (_percentComplete != value)
        {
          if (value < 0)
          {
            _percentComplete = 0;
          }
          else if (value > 100)
          {
            _percentComplete = 100;
          }
          else 
          {
            _percentComplete = value;
            if (_percentComplete > 0 && !ActualStartDate.HasValue)
            {
              ActualStartDate = EarlyStartDate;
            }

            if (_percentComplete == 100 && !ActualFinishDate.HasValue)
            {
              ActualFinishDate = EarlyFinishDate;
            }
          }

          OnPropertyChanged(nameof(PercentComplete));
        }
      }
    }

    #endregion

    #region Float

    public int TotalFloat
    {
      get => _totalFloat;
      set
      {
        if (_totalFloat != value)
        {
          _totalFloat = value;
          OnPropertyChanged(nameof(TotalFloat));
        }
      }
    }

    public int FreeFloat
    {
      get => _freeFloat;
      set
      {
        if (_freeFloat != value)
        {
          _freeFloat = value;
          OnPropertyChanged(nameof(FreeFloat));
        }
      }
    }

    #endregion

    #region Constraints

    public ConstraintType Constraint
    {
      get => _constraintType;
      set
      {
        if (_constraintType != value)
        {
          _constraintType = value;
          OnPropertyChanged(nameof(Constraint));
          if (ConstraintDate == null)
          {
            if (value is ConstraintType.StartOn or ConstraintType.StartOnOrLater)
            {
              ConstraintDate = EarlyStartDate;
            }
            else if (value is ConstraintType.EndOn or ConstraintType.EndOnOrEarlier)
            {
              ConstraintDate = EarlyFinishDate;
            }
          }
          if (value == ConstraintType.None)
          {
            ConstraintDate = null;
          }
        }
      }
    }

    public DateTime? ConstraintDate
    {
      get => _constraintDate;
      set
      {
        if (_constraintDate != value)
        {
          _constraintDate = value;
          OnPropertyChanged(nameof(ConstraintDate));
          if (ConstraintDate != null && Constraint == ConstraintType.None)
          {
            Constraint = ConstraintType.StartOnOrLater;
          }
        }
      }
    }

    #endregion

    #region Costs

    /// <summary>
    /// Gets the total _budget.
    /// </summary>
    public decimal TotalBudget { get; set; }

    /// <summary>
    /// Gets the total planned costs.
    /// </summary>
    public decimal TotalPlannedCosts { get; set; }

    /// <summary>
    /// Gets the total actual costs.
    /// </summary>
    public decimal TotalActualCosts { get; set; }

    #endregion

    #region Distortions

    public void RefreshDistortions(IEnumerable<Distortion> distortions)
    {
      if (distortions == null)
      {
        throw new ArgumentNullException(nameof(distortions), "Argument can't be null");
      }

      Distortions.Clear();

      foreach (var distortion in distortions)
      {
        Distortions.Add(distortion);
      }
    }

    #endregion

    #region Navigation Properties

    public ICollection<Distortion> Distortions { get; }

    public Calendar Calendar
    {
      get => _calendar;
      set
      {
        if (_calendar != value)
        {
          _calendar = value;
          OnPropertyChanged(nameof(Calendar));
        }
      }
    }

    public CustomAttribute CustomAttribute1
    {
      get => _customAttribute1;
      set
      {
        if (_customAttribute1 != value)
        {
          _customAttribute1 = value;
          OnPropertyChanged(nameof(CustomAttribute1));
        }
      }
    }

    public CustomAttribute CustomAttribute2
    {
      get => _customAttribute2;
      set
      {
        if (_customAttribute2 != value)
        {
          _customAttribute2 = value;
          OnPropertyChanged(nameof(CustomAttribute2));
        }
      }
    }

    public CustomAttribute CustomAttribute3
    {
      get => _customAttribute3;
      set
      {
        if (_customAttribute3 != value)
        {
          _customAttribute3 = value;
          OnPropertyChanged(nameof(CustomAttribute3));
        }
      }
    }

    public Fragnet Fragnet
    {
      get => _fragnet;
      set
      {
        if (_fragnet != value)
        {
          _fragnet = value;
          OnPropertyChanged(nameof(Fragnet));
        }
      }
    }

    public WBSItem WBSItem
    {
      get => _wbsItem;
      set
      {
        if (_wbsItem != value)
        {
          _wbsItem = value;
          OnPropertyChanged(nameof(WBSItem));
        }
      }
    }

    #endregion

    #region Public Overridden Methods

    /// <summary>
    /// Returns the Description of the activity
    /// </summary>
    public override string ToString()
    {
      return Name;
    }

    /// <summary>
    /// Checks if two activities are equal
    /// </summary>
    public override bool Equals(object obj)
    {
      if (obj is not Activity)
      {
        return false;
      }

      var a = (Activity)obj;
      int hash1 = a.GetHashCode();
      int hash2 = GetHashCode();
      return hash1.Equals(hash2);
    }

    /// <summary>
    /// Returns the hash code of an activity
    /// </summary>
    public override int GetHashCode()
    {
      return new { ID, Number, Name }.GetHashCode();
    }

    #endregion

    #region ICloneable

    public virtual Activity Clone()
    {
      return new Activity(this);
    }

    #endregion
  }
}
