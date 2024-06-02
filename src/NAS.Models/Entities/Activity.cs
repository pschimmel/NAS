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

    private string number;
    private string name;
    private DateTime earlyStartDate;
    private DateTime earlyFinishDate;
    private DateTime lateStartDate;
    private DateTime lateFinishDate;
    private DateTime? actualStartDate;
    private DateTime? actualFinishDate;
    private int _originalDuration;
    private int _remainingDuration;
    private double percentComplete;
    private int totalFloat;
    private int freeFloat;
    private ConstraintType constraint;
    private DateTime? constraintDate;
    private Calendar calendar;
    private CustomAttribute customAttribute1;
    private CustomAttribute customAttribute2;
    private CustomAttribute customAttribute3;
    private Fragnet fragnet;
    private WBSItem wbsItem;

    #endregion

    #region Constructors

    public Activity(Schedule schedule, bool isFixed = false)
    {
      Schedule = schedule;
      IsFixed = isFixed;
      constraint = ConstraintType.None;
      name = NASResources.NewActivity;
      _originalDuration = 5;
      ResourceAssignments = new ObservableCollection<ResourceAssignment>();
      Distortions = new ObservableCollection<Distortion>();
    }

    #endregion

    #region Properties

    public string Number
    {
      get => number;
      set
      {
        if (number != value)
        {
          number = value;
          OnPropertyChanged(nameof(Number));
        }
      }
    }

    public string Name
    {
      get => name;
      set
      {
        if (name != value)
        {
          name = value;
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

    #region Public Methods

    public static Activity NewActivity(Schedule schedule)
    {
      return new Activity(schedule);
    }

    public static Activity NewFixedActivity(Schedule schedule)
    {
      return new Activity(schedule, true);
    }

    public static Activity NewMilestone(Schedule schedule)
    {
      return new Milestone(schedule);
    }

    public static Activity NewFixedMilestone(Schedule schedule)
    {
      return new Milestone(schedule, true);
    }

    public IEnumerable<Activity> GetPredecessors()
    {
      return Schedule.GetPredecessors(this);
    }

    public IEnumerable<Activity> GetSuccessors()
    {
      return Schedule.GetSuccessors(this);
    }

    public IEnumerable<Relationship> GetPreceedingRelationships()
    {
      return Schedule.GetPreceedingRelationships(this);
    }

    public IEnumerable<Relationship> GetSucceedingRelationships()
    {
      return Schedule.GetSucceedingRelationships(this);
    }

    public IEnumerable<Activity> GetVisiblePredecessors()
    {
      return GetPredecessors().Where(x => x.Fragnet == null || x.Fragnet.IsVisible);
    }

    public IEnumerable<Relationship> GetVisiblePreceedingRelationships()
    {
      var predecessors = GetVisiblePredecessors();
      return GetPreceedingRelationships().Where(x => predecessors.Any(y => y.ID == x.Activity1.ID));
    }

    public int GetVisiblePredecessorCount()
    {
      return GetPredecessors().Count(x => x.Fragnet == null || x.Fragnet.IsVisible);
    }

    public IEnumerable<Activity> GetVisibleSuccessors()
    {
      return GetSuccessors().Where(x => x.Fragnet == null || x.Fragnet.IsVisible);
    }

    public IEnumerable<Relationship> GetVisibleSucceedingRelationships()
    {
      var successors = GetVisibleSuccessors();
      return GetSucceedingRelationships().Where(x => successors.Any(y => y.ID == x.Activity2.ID));
    }

    public int GetVisibleSuccessorCount()
    {
      return GetSuccessors().Count(x => x.Fragnet == null || x.Fragnet.IsVisible);
    }

    public PERTActivityData GetActivityData()
    {
      return Schedule.GetOrAddActivityData(this);
    }

    #endregion

    #region Dates

    public DateTime EarlyStartDate
    {
      get => earlyStartDate;
      set
      {
        if (earlyStartDate != value)
        {
          earlyStartDate = value;
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
      get => earlyFinishDate;
      set
      {
        if (earlyFinishDate != value)
        {
          earlyFinishDate = value;
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
      get => lateStartDate;
      set
      {
        if (lateStartDate != value)
        {
          lateStartDate = value;
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
      get => lateFinishDate;
      set
      {
        if (lateFinishDate != value)
        {
          lateFinishDate = value;
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
      get => actualStartDate;
      set
      {
        if (actualStartDate != value)
        {
          actualStartDate = value;
          OnPropertyChanged(nameof(ActualStartDate));
          OnPropertyChanged(nameof(StartDate));
          OnPropertyChanged(nameof(ActualDuration));
        }
      }
    }

    public DateTime? ActualFinishDate
    {
      get => actualFinishDate;
      set
      {
        if (actualFinishDate != value)
        {
          actualFinishDate = value;
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
          OnPropertyChanged(nameof(RetardedDuration));
          OnPropertyChanged(nameof(RemainingDuration));
          OnPropertyChanged(nameof(AtCompletionDuration));
          RemainingDuration = Convert.ToInt32(Convert.ToDouble(RetardedDuration) * (100d - PercentComplete) / 100d);
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
      get => ActivityType == ActivityType.Milestone ? 0 : _remainingDuration;
      set
      {
        if (_remainingDuration != value && ActivityType != ActivityType.Milestone)
        {
          _remainingDuration = value;
          PercentComplete = RetardedDuration != 0
            ? (Convert.ToDouble(RetardedDuration) - Convert.ToDouble(RemainingDuration)) * 100d / Convert.ToDouble(RetardedDuration)
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

    public int RetardedDuration
    {
      get
      {
        if (ActivityType == ActivityType.Milestone)
        {
          return 0;
        }

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
              : RetardedDuration
            : 0;
    }

    #endregion

    #region Percentages

    public double PercentComplete
    {
      get => percentComplete;
      set
      {
        if (percentComplete != value)
        {
          if (value < 0)
          {
            percentComplete = 0;
          }
          else if (value > 100)
          {
            percentComplete = 100;
          }
          else if (ActivityType == ActivityType.Milestone)
          {
            percentComplete = value < 50 ? 0 : 100;
          }
          else
          {
            percentComplete = value;
            if (percentComplete > 0 && !ActualStartDate.HasValue)
            {
              ActualStartDate = EarlyStartDate;
            }

            if (percentComplete == 100 && !ActualFinishDate.HasValue)
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
      get => totalFloat;
      set
      {
        if (totalFloat != value)
        {
          totalFloat = value;
          OnPropertyChanged(nameof(TotalFloat));
        }
      }
    }

    public int FreeFloat
    {
      get => freeFloat;
      set
      {
        if (freeFloat != value)
        {
          freeFloat = value;
          OnPropertyChanged(nameof(FreeFloat));
        }
      }
    }

    #endregion

    #region Constraints

    public ConstraintType Constraint
    {
      get => constraint;
      set
      {
        if (constraint != value)
        {
          constraint = value;
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
      get => constraintDate;
      set
      {
        if (constraintDate != value)
        {
          constraintDate = value;
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
    public decimal TotalBudget => ResourceAssignments == null ? 0 : ResourceAssignments.Sum(x => x.Budget);

    /// <summary>
    /// Gets the total planned costs.
    /// </summary>
    public decimal TotalPlannedCosts => ResourceAssignments == null ? 0 : ResourceAssignments.Sum(x => x.PlannedCosts);

    /// <summary>
    /// Gets the total actual costs.
    /// </summary>
    public decimal TotalActualCosts => ResourceAssignments == null ? 0 : ResourceAssignments.Sum(x => x.ActualCosts);

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

    #region Resource Associations

    public void RefreshResourceAssignments(IEnumerable<ResourceAssignment> resourceAssignments)
    {
      if (resourceAssignments == null)
      {
        throw new ArgumentNullException(nameof(resourceAssignments), "Argument can't be null");
      }

      Distortions.Clear();

      foreach (var resourceAssignment in resourceAssignments)
      {
        ResourceAssignments.Add(resourceAssignment);
      }
    }

    #endregion

    #region General

    #region Splitting

    public bool CanSplit()
    {
      return ActivityType == ActivityType.Activity && OriginalDuration > 1 && !IsStarted;
    }

    /// <summary>
    /// Splits an activity into two parts. Each resulting activity will have half of the duration of the original
    /// activity and all features.
    /// </summary>
    public Activity SplitActivity()
    {
      if (!CanSplit())
      {
        return null;
      }

      var newActivity = new Activity(Schedule, IsFixed);
      newActivity.Name = Name + NASResources.Copy;
      newActivity.OriginalDuration = OriginalDuration / 2;
      newActivity.Calendar = Calendar;
      newActivity.Fragnet = Fragnet;
      newActivity.WBSItem = WBSItem;
      OriginalDuration -= newActivity.OriginalDuration;
      Schedule.AddActivity(newActivity);

      // Move all successors of activity to new activity
      foreach (var r in GetSucceedingRelationships().ToList())
      {
        Schedule.AddRelationship(newActivity, r.Activity2);
        Schedule.RemoveRelationship(r);
      }

      // Add default relationships between split activitiess
      Schedule.AddRelationship(this, newActivity);
      newActivity.EarlyStartDate = Calendar.GetEndDate(EarlyFinishDate, 2);
      return newActivity;
    }

    #endregion

    #region Combining

    /// <summary>
    /// Combines two subsequent activites.
    /// </summary>
    public void CombineActivities()
    {
      var schedule = Schedule;
      var successor = GetSuccessors().SingleOrDefault();
      Debug.Assert(successor != null);

      if (successor == null)
      {
        return;
      }

      foreach (var successorOfSuccessor in successor.GetSuccessors())
      {
        schedule.AddRelationship(this, successorOfSuccessor);
      }

      foreach (var relationship in successor.GetSucceedingRelationships())
      {
        schedule.RemoveRelationship(relationship);
      }

      OriginalDuration += successor.OriginalDuration;
      Schedule.RemoveActivity(successor);
    }

    /// <summary>
    /// Checks if an activity can be combined with its following activity.
    /// </summary>
    public bool CanCombineActivity()
    {
      return ActivityType == ActivityType.Activity && GetSuccessors().Count(x => x.ActivityType == ActivityType.Activity) == 1;
    }

    #endregion

    #endregion

    #region Navigation Properties

    public ICollection<ResourceAssignment> ResourceAssignments { get; }

    public Schedule Schedule { get; }

    public ICollection<Distortion> Distortions { get; }

    public Calendar Calendar
    {
      get => calendar;
      set
      {
        if (calendar != value)
        {
          calendar = value;
          OnPropertyChanged(nameof(Calendar));
        }
      }
    }

    public CustomAttribute CustomAttribute1
    {
      get => customAttribute1;
      set
      {
        if (customAttribute1 != value)
        {
          customAttribute1 = value;
          OnPropertyChanged(nameof(CustomAttribute1));
        }
      }
    }

    public CustomAttribute CustomAttribute2
    {
      get => customAttribute2;
      set
      {
        if (customAttribute2 != value)
        {
          customAttribute2 = value;
          OnPropertyChanged(nameof(CustomAttribute2));
        }
      }
    }

    public CustomAttribute CustomAttribute3
    {
      get => customAttribute3;
      set
      {
        if (customAttribute3 != value)
        {
          customAttribute3 = value;
          OnPropertyChanged(nameof(CustomAttribute3));
        }
      }
    }

    public Fragnet Fragnet
    {
      get => fragnet;
      set
      {
        if (fragnet != value)
        {
          fragnet = value;
          OnPropertyChanged(nameof(Fragnet));
        }
      }
    }

    public WBSItem WBSItem
    {
      get => wbsItem;
      set
      {
        if (wbsItem != value)
        {
          wbsItem = value;
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
      return (ID + Number + Name).GetHashCode();//if (!string.IsNullOrEmpty(Number))//  return ID ^ Number.GetHashCode();//if (!string.IsNullOrEmpty(Name))//  return ID ^ Name.GetHashCode();//return ID.GetHashCode();
    }

    #endregion

    public Milestone ChangeToMilestone()
    {
      Debug.Assert(ActivityType == ActivityType.Activity);
      var schedule = Schedule;
      var predecessors = GetPreceedingRelationships().ToList();
      var successors = GetSucceedingRelationships().ToList();
      var newMilestone = schedule.AddMilestone(IsFixed);
      newMilestone.Name = Name;
      newMilestone.Number = Number;
      newMilestone.Calendar = Calendar;
      newMilestone.Constraint = Constraint;
      newMilestone.ConstraintDate = ConstraintDate;
      newMilestone.Constraint = Constraint;
      newMilestone.CustomAttribute1 = CustomAttribute1;
      newMilestone.CustomAttribute2 = CustomAttribute2;
      newMilestone.CustomAttribute3 = CustomAttribute3;
      newMilestone.EarlyStartDate = EarlyStartDate;
      newMilestone.LateStartDate = LateStartDate;
      newMilestone.WBSItem = WBSItem;
      newMilestone.Fragnet = Fragnet;
      if (PercentComplete == 100)
      {
        newMilestone.PercentComplete = 100;
      }

      foreach (var r in predecessors)
      {
        var newRelationship = new Relationship(r.Activity1, newMilestone);
        newRelationship.RelationshipType = r.RelationshipType;
        newRelationship.Lag = r.Lag;
      }
      foreach (var r in successors)
      {
        var newRelationship = new Relationship(newMilestone, r.Activity2);
        newRelationship.RelationshipType = r.RelationshipType;
        newRelationship.Lag = r.Lag;
      }

      return newMilestone;
    }
  }
}
