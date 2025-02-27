using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using NAS.Models.Base;
using NAS.Models.Enums;
using NAS.Models.Scheduler;
using NAS.Resources;

namespace NAS.Models.Entities
{
  public class Schedule : NASObject
  {
    #region Events

    public event EventHandler<ItemEventArgs<Activity>> ActivityAdded;
    public event EventHandler<ItemEventArgs<Activity>> ActivityRemoved;
    public event EventHandler<ItemEventArgs<Relationship>> RelationshipAdded;
    public event EventHandler<ItemEventArgs<Relationship>> RelationshipRemoved;

    #endregion

    #region Fields

    private readonly List<Activity> _activities = [];
    private readonly List<Relationship> _relationships = [];

    #endregion

    #region Constructors

    public Schedule()
    {
      Fragnets.CollectionChanged += Fragnets_CollectionChanged;
      DataDate = DateTime.Today;
      StartDate = DateTime.Today;
      Name = NASResources.NewSchedule;
      CreatedDate = DateTime.Now;
      CustomAttribute1Header = NASResources.Attribute1;
      CustomAttribute2Header = NASResources.Attribute2;
      CustomAttribute3Header = NASResources.Attribute3;
    }

    private void Fragnets_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      if (e.OldItems != null)
      {
        foreach (Fragnet fragnet in e.OldItems.OfType<Fragnet>())
        {
          foreach (var activity in Activities)
          {
            if (activity.Fragnet == fragnet)
            {
              activity.Fragnet = null;
            }
          }
        }
      }
    }

    private Schedule(Schedule other)
    {
      var clonedCalendars = new Dictionary<Calendar, Calendar>();
      var clonedFragnets = new Dictionary<Fragnet, Fragnet>();
      var clonedWBSItems = new Dictionary<WBSItem, WBSItem>();
      var clonedActivities = new Dictionary<Activity, Activity>();
      var clonedResources = new Dictionary<Resource, Resource>();

      Func<Calendar, Calendar> cloneCalendar = null;
      cloneCalendar = new Func<Calendar, Calendar>(calendar =>
      {
        if (clonedCalendars.TryGetValue(calendar, out Calendar alreadyClonedClone))
        {
          return alreadyClonedClone;
        }
        else if (calendar.BaseCalendar == null)
        {
          var clone = calendar.Clone();
          clonedCalendars.Add(calendar, clone);
          return clone;
        }
        else
        {
          if (!clonedCalendars.TryGetValue(calendar.BaseCalendar, out Calendar clonedBase))
          {
            clonedBase = cloneCalendar(calendar.BaseCalendar);
            clonedCalendars.Add(calendar.BaseCalendar, clonedBase);
          }
          var clone = calendar.Clone(clonedBase);
          clonedCalendars.Add(calendar, clone);
          return clone;
        }
      });

      foreach (var calendar in other.Calendars)
      {
        Calendars.Add(cloneCalendar(calendar));
      }

      foreach (var fragnet in other.Fragnets)
      {
        var clonedFragnet = fragnet.Clone();
        Fragnets.Add(clonedFragnet);
        clonedFragnets.Add(fragnet, clonedFragnet);
      }

      foreach (var resource in other.Resources)
      {
        var clonedResource = resource.Clone();
        Resources.Add(clonedResource);
        clonedResources.Add(resource, clonedResource);
      }

      WBSItem = other.WBSItem.Clone();

      Action<WBSItem, WBSItem> addToClonedWBSItems = null;

      addToClonedWBSItems = new Action<WBSItem, WBSItem>((item, clone) =>
      {
        clonedWBSItems.Add(item, clone);
        foreach (var child in item.Children)
        {
          addToClonedWBSItems(child, clone.Children.First(x => x.Name == child.Name));
        }
      });

      addToClonedWBSItems(other.WBSItem, WBSItem);

      foreach (var activity in other.Activities)
      {
        var newActivity = activity.Clone();
        clonedActivities.Add(activity, newActivity);
        AddActivity(newActivity);

        if (activity.Fragnet != null)
        {
          newActivity.Fragnet = clonedFragnets[activity.Fragnet];
        }

        if (activity.Calendar != null)
        {
          newActivity.Calendar = clonedCalendars[activity.Calendar];
        }

        if (WBSItem != null && activity.WBSItem != null)
        {
          newActivity.WBSItem = FindWBSItem(WBSItem, activity.WBSItem.Number);
        }

        foreach (var distortion in activity.Distortions)
        {
          Distortion newDistortion = distortion.Clone();

          if (distortion.Fragnet != null)
          {
            if (!clonedFragnets.TryGetValue(distortion.Fragnet, out Fragnet clonedFragnet))
            {
              clonedFragnet = distortion.Fragnet.Clone();
              clonedFragnets.Add(distortion.Fragnet, clonedFragnet);
            }

            newDistortion.Fragnet = clonedFragnet;
          }
        }
      }

      StartDate = other.StartDate;
      DataDate = other.DataDate;
      Description = other.Description;
      EndDate = other.EndDate;
      Name = other.Name;

      foreach (var resourceAssignment in other.ResourceAssignments)
      {
        var activity = clonedActivities[resourceAssignment.Activity];
        var resource = clonedResources[resourceAssignment.Resource];
        ResourceAssignments.Add(resourceAssignment.Clone(activity, resource));
      }

      foreach (var relationship in other.Relationships)
      {
        var activity1 = clonedActivities[relationship.Activity1];
        var activity2 = clonedActivities[relationship.Activity2];
        AddRelationship(relationship.Clone(activity1, activity2));
      }

      foreach (var layout in other.Layouts)
      {
        var clonedLayout = layout.Clone(clonedResources);
        Layouts.Add(clonedLayout);
      }
    }

    #endregion

    #region Properties

    public string Name { get; set; }

    public string Description { get; set; }

    public string FileName { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public DateTime DataDate { get; set; }

    public string CustomAttribute1Header { get; set; }

    public string CustomAttribute2Header { get; set; }

    public string CustomAttribute3Header { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string CreatedBy { get; set; }

    public string ModifiedBy { get; set; }

    public ObservableCollection<Schedule> Baselines { get; } = [];

    public Schedule BaselineOf { get; set; }

    public ObservableCollection<VisibleBaseline> VisibleBaselines { get; } = [];

    public IReadOnlyCollection<Activity> Activities => new ReadOnlyCollection<Activity>(_activities);

    public ObservableCollection<Calendar> Calendars { get; } = [];

    public ObservableCollection<CustomAttribute> CustomAttributes1 { get; } = [];

    public ObservableCollection<CustomAttribute> CustomAttributes2 { get; } = [];

    public ObservableCollection<CustomAttribute> CustomAttributes3 { get; } = [];

    public ObservableCollection<Fragnet> Fragnets { get; } = [];

    public ObservableCollection<Layout> Layouts { get; } = [];

    public ObservableCollection<Resource> Resources { get; } = [];

    public ObservableCollection<PERTDefinition> PERTDefinitions { get; } = [];

    public ObservableCollection<PERTActivityData> PERTActivityItems { get; } = [];

    public WBSItem WBSItem { get; set; }

    public IReadOnlyCollection<Relationship> Relationships => new ReadOnlyCollection<Relationship>(_relationships);

    public ObservableCollection<ResourceAssignment> ResourceAssignments { get; set; } = new ObservableCollection<ResourceAssignment>();

    public SchedulingSettings SchedulingSettings { get; } = new SchedulingSettings();

    #endregion

    #region Standards

    public Layout ActiveLayout
    {
      get
      {
        // There has to be always one active layout
        var layout = Layouts.FirstOrDefault(x => x.IsActive == true);
        if (layout == null)
        {
          if (Layouts.Count == 0)
          {
            Layouts.Add(new GanttLayout());
          }

          layout = Layouts.First();
          layout.IsActive = true;
        }

        return layout;
      }
      set
      {
        foreach (var l in Layouts)
        {
          l.IsActive = l == value;
        }
      }
    }

    internal void UpdateActiveLayout()
    {
      OnPropertyChanged(nameof(ActiveLayout));
    }

    public Calendar StandardCalendar
    {
      get => Calendars.FirstOrDefault(x => x.IsStandard == true);
      set
      {
        foreach (var c in Calendars)
        {
          c.IsStandard = c == value;
        }
      }
    }

    internal void UpdateStandardCalendar()
    {
      OnPropertyChanged(nameof(ActiveLayout));
    }

    public int OriginalDurationDefault { get; } = 5;

    public string NewActivityPrefixDefault { get; } = "A";

    public uint NewActivityDistance { get; } = 10;

    #endregion

    #region Costs

    /// <summary>
    /// Gets the total _budget.
    /// </summary>
    public decimal TotalBudget => Activities.OfType<Activity>().Where(a => a.Fragnet == null || a.Fragnet.IsVisible).Sum(x => x.TotalBudget);

    /// <summary>
    /// Gets the total actual costs.
    /// </summary>
    public decimal TotalActualCosts => Activities.OfType<Activity>().Where(a => a.Fragnet == null || a.Fragnet.IsVisible).Sum(x => x.TotalActualCosts);

    /// <summary>
    /// Gets the total planned costs.
    /// </summary>
    public decimal TotalPlannedCosts => Activities.OfType<Activity>().Where(a => a.Fragnet == null || a.Fragnet.IsVisible).Sum(x => x.TotalPlannedCosts);

    #endregion

    #region Dates

    /// <summary>
    /// Gets the last day of the project.
    /// </summary>
    public DateTime LastDay
    {
      get
      {
        var lastDay = StartDate;
        foreach (var a in Activities)
        {
          if ((a.Fragnet == null || a.Fragnet.IsVisible) && a.FinishDate > lastDay)
          {
            lastDay = a.FinishDate;
          }
        }
        return lastDay;
      }
    }

    /// <summary>
    /// Gets the first day of the project.
    /// </summary>
    public DateTime FirstDay
    {
      get
      {
        var firstDay = StartDate;
        foreach (var a in Activities)
        {
          if ((a.Fragnet == null || a.Fragnet.IsVisible) && a.StartDate < firstDay)
          {
            firstDay = a.StartDate;
          }
        }
        return firstDay;
      }
    }

    #endregion

    #region Activities

    public void AddActivity(Activity activity)
    {
      if (string.IsNullOrWhiteSpace(activity.Number) || _activities.Any(x => x.Number == activity.Number))
      {
        activity.Number = GetNewID();
      }

      activity.Calendar ??= StandardCalendar;

      _activities.Add(activity);
      OnActivityAdded(activity);
    }

    public Activity AddActivity(bool isFixed = false)
    {
      var newActivity = new Activity(isFixed)
      {
        Number = GetNewID(),
        Calendar = StandardCalendar,
        EarlyStartDate = DataDate,
        LateStartDate = DataDate,
        OriginalDuration = 5
      };
      _activities.Add(newActivity);
      OnActivityAdded(newActivity);
      return newActivity;
    }

    public void RemoveActivity(Activity activity)
    {
      foreach (var item in GetPreceedingRelationships(activity).ToList())
      {
        RemoveRelationship(item);
      }

      foreach (var item in GetSucceedingRelationships(activity).ToList())
      {
        RemoveRelationship(item);
      }

      if (_activities.Remove(activity))
      {
        OnActivityRemoved(activity);
      }
    }

    public Milestone AddMilestone(bool isFixed = false)
    {
      var newActivity = new Milestone(isFixed)
      {
        Number = GetNewID(),
        Calendar = StandardCalendar,
        EarlyStartDate = DataDate,
        LateStartDate = DataDate,
        OriginalDuration = 0
      };
      _activities.Add(newActivity);
      OnActivityAdded(newActivity);
      return newActivity;
    }

    public void RemoveMilestone(Milestone milestone)
    {
      RemoveActivity(milestone);
    }

    public Activity GetActivity(string number)
    {
      var activity = Activities.FirstOrDefault(x => x.Number == number);
      Debug.Assert(activity != null, "No matching activity found.");
      return activity;
    }

    public Activity GetActivity(Guid guid)
    {
      var activity = Activities.FirstOrDefault(x => x.ID == guid);
      Debug.Assert(activity != null, "No matching activity found.");
      return activity;
    }

    public IEnumerable<Activity> GetPredecessors(Activity activity)
    {
      return GetPreceedingRelationships(activity).Select(x => x.Activity1);
    }

    public IEnumerable<Activity> GetSuccessors(Activity activity)
    {
      return GetSucceedingRelationships(activity).Select(x => x.Activity2);
    }

    public IEnumerable<Activity> GetVisiblePredecessors(Activity activity)
    {
      return GetPredecessors(activity).Where(x => x.Fragnet == null || x.Fragnet.IsVisible);
    }

    public IEnumerable<Relationship> GetVisiblePreceedingRelationships(Activity activity)
    {
      var predecessors = GetVisiblePredecessors(activity);
      return GetPreceedingRelationships(activity).Where(x => predecessors.Any(y => y.ID == x.Activity1.ID));
    }

    public int GetVisiblePredecessorCount(Activity activity)
    {
      return GetPredecessors(activity).Count(x => x.Fragnet == null || x.Fragnet.IsVisible);
    }

    public IEnumerable<Activity> GetVisibleSuccessors(Activity activity)
    {
      return GetSuccessors(activity).Where(x => x.Fragnet == null || x.Fragnet.IsVisible);
    }

    public IEnumerable<Relationship> GetVisibleSucceedingRelationships(Activity activity)
    {
      var successors = GetVisibleSuccessors(activity);
      return GetSucceedingRelationships(activity).Where(x => successors.Any(y => y.ID == x.Activity2.ID));
    }

    public int GetVisibleSuccessorCount(Activity activity)
    {
      return GetSuccessors(activity).Count(x => x.Fragnet == null || x.Fragnet.IsVisible);
    }

    public static bool CanSplit(Activity activity)
    {
      return activity.ActivityType == ActivityType.Activity && activity.OriginalDuration > 1 && !activity.IsStarted;
    }

    /// <summary>
    /// Splits an activity into two parts. Each resulting activity will have half of the duration of the original
    /// activity and all features.
    /// </summary>
    public Activity SplitActivity(Activity activity)
    {
      if (!CanSplit(activity))
      {
        return null;
      }

      var newActivity = new Activity(activity.IsFixed)
      {
        Name = Name + NASResources.Copy,
        OriginalDuration = activity.OriginalDuration / 2,
        Calendar = activity.Calendar,
        Fragnet = activity.Fragnet,
        WBSItem = activity.WBSItem
      };
      activity.OriginalDuration -= newActivity.OriginalDuration;
      AddActivity(newActivity);

      // Move all successors of activity to new activity
      foreach (var r in GetSucceedingRelationships(activity).ToList())
      {
        AddRelationship(newActivity, r.Activity2);
        RemoveRelationship(r);
      }

      // Add default relationships between split activitiess
      AddRelationship(activity, newActivity);
      newActivity.EarlyStartDate = activity.Calendar.GetEndDate(activity.EarlyFinishDate, 2);
      return newActivity;
    }

    /// <summary>
    /// Checks if an activity can be combined with its following activity.
    /// </summary>
    public bool CanCombineActivity(Activity activity)
    {
      return activity.ActivityType == ActivityType.Activity && GetSuccessors(activity).Count(x => x.ActivityType == ActivityType.Activity) == 1;
    }

    /// <summary>
    /// Combines two subsequent activites.
    /// </summary>
    public void CombineActivities(Activity activity)
    {
      var successor = GetSuccessors(activity).SingleOrDefault();
      Debug.Assert(successor != null);

      if (successor == null)
      {
        return;
      }

      foreach (var successorOfSuccessor in GetSuccessors(successor))
      {
        AddRelationship(activity, successorOfSuccessor);
      }

      foreach (var relationship in GetSucceedingRelationships(successor))
      {
        RemoveRelationship(relationship);
      }

      activity.OriginalDuration += successor.OriginalDuration;
      RemoveActivity(successor);
    }

    public Milestone ChangeToMilestone(Activity activity)
    {
      if (activity is Milestone activityAsMilestone)
        return activityAsMilestone;

      Debug.Assert(activity.ActivityType == ActivityType.Activity);
      var predecessors = GetPreceedingRelationships(activity).ToList();
      var successors = GetSucceedingRelationships(activity).ToList();
      var newMilestone = AddMilestone(activity.IsFixed);
      newMilestone.Name = activity.Name;
      newMilestone.Number = activity.Number;
      newMilestone.Calendar = activity.Calendar;
      newMilestone.Constraint = activity.Constraint;
      newMilestone.ConstraintDate = activity.ConstraintDate;
      newMilestone.CustomAttribute1 = activity.CustomAttribute1;
      newMilestone.CustomAttribute2 = activity.CustomAttribute2;
      newMilestone.CustomAttribute3 = activity.CustomAttribute3;
      newMilestone.EarlyStartDate = activity.EarlyStartDate;
      newMilestone.LateStartDate = activity.LateStartDate;
      newMilestone.WBSItem = activity.WBSItem;
      newMilestone.Fragnet = activity.Fragnet;
      if (activity.PercentComplete == 100)
      {
        newMilestone.PercentComplete = 100;
      }

      foreach (var predecessor in predecessors)
      {
        var newRelationship = new Relationship(predecessor.Activity1, newMilestone)
        {
          RelationshipType = predecessor.RelationshipType,
          Lag = predecessor.Lag
        };

        RemoveRelationship(predecessor);
        AddRelationship(newRelationship);
      }
      foreach (var successor in successors)
      {
        var newRelationship = new Relationship(newMilestone, successor.Activity2)
        {
          RelationshipType = successor.RelationshipType,
          Lag = successor.Lag
        };

        RemoveRelationship(successor);
        AddRelationship(newRelationship);
      }

      return newMilestone;
    }

    public Activity ChangeToActivity(Milestone milestone)
    {
      var predecessors = GetPreceedingRelationships(milestone).ToList();
      var successors = GetSucceedingRelationships(milestone).ToList();
      var newActivity = AddActivity(milestone.IsFixed);
      newActivity.Number = milestone.Number;
      newActivity.Name = milestone.Name;
      newActivity.Calendar = milestone.Calendar;
      newActivity.Constraint = milestone.Constraint;
      newActivity.ConstraintDate = milestone.ConstraintDate;
      newActivity.Constraint = milestone.Constraint;
      newActivity.CustomAttribute1 = milestone.CustomAttribute1;
      newActivity.CustomAttribute2 = milestone.CustomAttribute2;
      newActivity.CustomAttribute3 = milestone.CustomAttribute3;
      newActivity.EarlyStartDate = milestone.EarlyStartDate;
      newActivity.LateStartDate = milestone.LateStartDate;
      newActivity.WBSItem = milestone.WBSItem;
      newActivity.Fragnet = milestone.Fragnet;

      if (milestone.PercentComplete == 100)
      {
        newActivity.PercentComplete = 100;
      }

      foreach (var predecessor in predecessors)
      {
        AddRelationship(predecessor.Activity1, newActivity, predecessor.RelationshipType).Lag = predecessor.Lag;
        RemoveRelationship(predecessor);
      }

      foreach (var successor in successors)
      {
        AddRelationship(newActivity, successor.Activity2, successor.RelationshipType).Lag = successor.Lag;
        RemoveRelationship(successor);
      }

      return newActivity;
    }


    #endregion

    #region Relationships

    public void AddRelationship(Relationship relationship)
    {
      _relationships.Add(relationship);
      OnRelationshipAdded(relationship);
    }

    public Relationship AddRelationship(Activity activity1, Activity activity2, RelationshipType type = RelationshipType.FinishFinish)
    {
      var relationship = new Relationship(activity1, activity2, type);
      _relationships.Add(relationship);
      OnRelationshipAdded(relationship);
      return relationship;
    }

    public void RemoveRelationship(Relationship relationship)
    {
      _relationships.Remove(relationship);
      OnRelationshipRemoved(relationship);
    }

    public Relationship GetRelationship(Activity activity1, Activity activity2)
    {
      return Relationships.FirstOrDefault(x => x.Activity1 == activity1 && x.Activity2 == activity2);
    }

    public IEnumerable<Relationship> GetPreceedingRelationships(Activity activity)
    {
      return Relationships.Where(x => x.Activity2 == activity);
    }

    public IEnumerable<Relationship> GetSucceedingRelationships(Activity activity)
    {
      return Relationships.Where(x => x.Activity1 == activity);
    }

    #endregion

    #region PERT

    public PERTActivityData GetOrAddActivityData(Activity activity)
    {
      var data = PERTActivityItems.FirstOrDefault(x => x.Activity.ID == activity.ID);

      if (data == null)
      {
        data = new PERTActivityData(activity);
        PERTActivityItems.Add(data);
      }

      return data;
    }

    #endregion

    #region WBS

    public IEnumerable<WBSItem> GetWBSItems()
    {
      var items = new List<WBSItem>();
      if (WBSItem != null)
      {
        items.Add(WBSItem);
        AddSubItems(WBSItem, items);
      }
      return items;
    }

    private static void AddSubItems(WBSItem parent, List<WBSItem> items)
    {
      foreach (var item in parent.Children)
      {
        items.Add(item);
        AddSubItems(item, items);
      }
    }

    #endregion

    #region Calendars

    public bool CanRemoveCalendar(Calendar calendar)
    {
      return !calendar.IsStandard && !Activities.Any(x => x.Calendar.ID == calendar.ID);
    }

    #endregion

    #region Resources

    public bool CanRemoveResource(Resource resource)
    {
      return !ResourceAssignments.Any(x => x.Resource == resource);
    }

    #endregion

    #region Custom Attributes

    public bool CanRemoveCustomAttribute1(CustomAttribute item)
    {
      return !Activities.Any(x => x.CustomAttribute1?.ID == item.ID);
    }

    public bool CanRemoveCustomAttribute2(CustomAttribute item)
    {
      return !Activities.Any(x => x.CustomAttribute2?.ID == item.ID);
    }

    public bool CanRemoveCustomAttribute3(CustomAttribute item)
    {
      return !Activities.Any(x => x.CustomAttribute3?.ID == item.ID);
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Ensures minimal requirements are set for the schedule.
    /// </summary>
    public void Validate()
    {
      // Ensure that we have at least one layout.
      if (Layouts.Count == 0)
      {
        var layout = new GanttLayout { IsActive = true };
        Layouts.Add(layout);
      }
      else
      {
        // Ensure that there is exactly one active layout.
        if (!Layouts.Any(x => x.IsActive))
        {
          var layout = Layouts.First();
          layout.IsActive = true;
        }
      }

      // Ensure that we have at least one calendar.
      if (Calendars.Count == 0)
      {
        var calendar = new Calendar { IsStandard = true };
        Calendars.Add(calendar);
      }
      else
      {
        // Ensure that there is one standard calendar
        if (!Calendars.Any(x => x.IsStandard))
        {
          var calendar = Calendars.First();
          calendar.IsStandard = true;
        }
      }
    }

    /// <summary>
    /// Removes activity baseline.
    /// </summary>
    public void RemoveBaseline(Schedule baseline)
    {
      baseline.BaselineOf = null;
      Baselines.Remove(baseline);

      // Also remove the visible baseline defintion from the layouts.
      foreach (var layout in Layouts)
      {
        foreach (var visibleBaseline in layout.VisibleBaselines.Where(x => x.Schedule == baseline).ToList())
        {
          layout.VisibleBaselines.Remove(visibleBaseline);
        }
      }
    }

    public void EnsureWBS()
    {
      if (WBSItem == null)
      {
        WBSItem = new WBSItem
        {
          Name = Name,
          Number = "1"
        };
      }
    }

    #endregion

    #region Protected Methods

    protected void OnActivityAdded(Activity activity)
    {
      ActivityAdded?.Invoke(this, new ItemEventArgs<Activity>(activity));
    }

    protected void OnActivityRemoved(Activity activity)
    {
      ActivityRemoved?.Invoke(this, new ItemEventArgs<Activity>(activity));
    }

    protected void OnRelationshipAdded(Relationship relationship)
    {
      RelationshipAdded?.Invoke(this, new ItemEventArgs<Relationship>(relationship));
    }

    protected void OnRelationshipRemoved(Relationship relationship)
    {
      RelationshipRemoved?.Invoke(this, new ItemEventArgs<Relationship>(relationship));
    }

    #endregion

    #region Private Methods

    private string GetNewID()
    {
      uint id = 0;
      var ids = Activities.ToList().Select(x => x.Number);

      while (ids.Contains(NewActivityPrefixDefault + id.ToString("0000")))
      {
        id += NewActivityDistance;
      }

      return NewActivityPrefixDefault + id.ToString("0000");
    }

    private static WBSItem FindWBSItem(WBSItem parent, string number)
    {
      if (parent.Number == number)
      {
        return parent;
      }

      foreach (var item in parent.Children)
      {
        if (FindWBSItem(item, number) != null)
        {
          return item;
        }
      }
      return null;
    }

    #endregion

    #region ICloneable

    public Schedule Clone()
    {
      return new Schedule(this);
    }

    #endregion
  }
}
