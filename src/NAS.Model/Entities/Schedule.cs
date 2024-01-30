using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using NAS.Model.Base;
using NAS.Model.Enums;
using NAS.Resources;

namespace NAS.Model.Entities
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

    private readonly List<Activity> _activities;
    private readonly List<Relationship> _relationships;
    private readonly ObservableCollection<Fragnet> _fragnets;

    #endregion

    #region Constructors

    public Schedule()
    {
      _activities = [];
      _relationships = [];
      Baselines = new ObservableCollection<Schedule>();
      VisibleBaselines = new ObservableCollection<VisibleBaseline>();
      Calendars = new ObservableCollection<Calendar>();
      CustomAttributes1 = new ObservableCollection<CustomAttribute>();
      CustomAttributes2 = new ObservableCollection<CustomAttribute>();
      CustomAttributes3 = new ObservableCollection<CustomAttribute>();
      _fragnets = [];
      _fragnets.CollectionChanged += Fragnets_CollectionChanged;
      Layouts = new ObservableCollection<Layout>();
      Resources = new ObservableCollection<Resource>();
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

    public Schedule(Schedule schedule)
    {
      CopySchedule(schedule, this);
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

    public ICollection<Schedule> Baselines { get; set; }

    public Schedule BaselineOf { get; set; }

    public ICollection<VisibleBaseline> VisibleBaselines { get; set; }

    public IReadOnlyCollection<Activity> Activities => new ReadOnlyCollection<Activity>(_activities);

    public ICollection<Calendar> Calendars { get; set; }

    public ICollection<CustomAttribute> CustomAttributes1 { get; set; }

    public ICollection<CustomAttribute> CustomAttributes2 { get; set; }

    public ICollection<CustomAttribute> CustomAttributes3 { get; set; }

    public ICollection<Fragnet> Fragnets { get; set; }

    public ICollection<Layout> Layouts { get; set; }

    public ICollection<Resource> Resources { get; set; }

    public ICollection<PERTDefinition> PERTDefinitions { get; set; }

    public ICollection<PERTActivityData> PERTActivityItems { get; set; }

    public WBSItem WBSItem { get; set; }

    public IReadOnlyCollection<Relationship> Relationships => new ReadOnlyCollection<Relationship>(_relationships);

    public string SchedulingSettings { get; set; }

    #endregion

    #region Standards

    public Layout CurrentLayout
    {
      get
      {
        if (!Layouts.Any(x => x.IsCurrent == true) && Layouts.Count != 0)
        {
          var layout = Layouts.First();
          layout.IsCurrent = true;
          return layout;
        }
        else
        {
          return Layouts.First(x => x.IsCurrent == true);
        }
      }
      set
      {
        foreach (var l in Layouts)
        {
          l.IsCurrent = l == value;
        }
      }
    }

    internal void UpdateCurrentLayout()
    {
      OnPropertyChanged(nameof(CurrentLayout));
    }

    public Calendar StandardCalendar
    {
      get
      {
        if (!Calendars.Any(x => x.IsStandard == true) && Calendars.Count != 0)
        {
          var calendar = Calendars.First();
          calendar.IsStandard = true;
          return calendar;
        }
        else
        {
          return Calendars.First(x => x.IsStandard == true);
        }
      }
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
      OnPropertyChanged(nameof(CurrentLayout));
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
      activity.Schedule = this;

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
      var newActivity = new Activity();
      newActivity.Number = GetNewID();
      newActivity.Schedule = this;
      newActivity.Calendar = StandardCalendar;
      newActivity.EarlyStartDate = DataDate;
      newActivity.LateStartDate = DataDate;
      newActivity.OriginalDuration = 5;
      newActivity.IsFixed = isFixed;
      _activities.Add(newActivity);
      OnActivityAdded(newActivity);
      return newActivity;
    }

    public void RemoveActivity(Activity activity)
    {
      foreach (var item in activity.GetPreceedingRelationships().ToList())
      {
        RemoveRelationship(item);
      }

      foreach (var item in activity.GetSucceedingRelationships().ToList())
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
      var newActivity = new Milestone();
      newActivity.Number = GetNewID();
      newActivity.Schedule = this;
      newActivity.Calendar = StandardCalendar;
      newActivity.EarlyStartDate = DataDate;
      newActivity.LateStartDate = DataDate;
      newActivity.OriginalDuration = 0;
      newActivity.IsFixed = isFixed;
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
      var data = PERTActivityItems.FirstOrDefault(x => x.ActivityID == activity.ID);

      if (data == null)
      {
        data = new PERTActivityData { ActivityID = activity.ID, Schedule = this };
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
      return !calendar.IsStandard && !Activities.Any(x => x.Schedule.ID == calendar.ID && x.Calendar.ID == calendar.ID);
    }

    public void RefreshCalendars(IEnumerable<Calendar> newCalendars)
    {
      if (newCalendars == null)
      {
        throw new ArgumentNullException(nameof(newCalendars), "Argument can't be null");
      }

      if (!newCalendars.Any())
      {
        throw new ArgumentException("Argument can't be empty", nameof(newCalendars));
      }

      Calendars.Clear();

      foreach (var calendar in newCalendars)
      {
        Calendars.Add(calendar);
      }

      if (!Calendars.Any(x => x.IsStandard))
      {
        Calendars.First().IsStandard = true;
      }
    }

    #endregion

    #region Resources

    public bool CanRemoveResource(Resource resource)
    {
      return Activities.Any(x => x.ResourceAssociations.Any(x => x.Resource == resource));
    }

    #endregion

    #region Custom Attributes

    public bool CanRemoveCustomAttribute1(CustomAttribute item)
    {
      return !Activities.Any(x => x.CustomAttribute1.ID == item.ID);
    }

    public bool CanRemoveCustomAttribute2(CustomAttribute item)
    {
      return !Activities.Any(x => x.CustomAttribute2.ID == item.ID);
    }

    public bool CanRemoveCustomAttribute3(CustomAttribute item)
    {
      return !Activities.Any(x => x.CustomAttribute3.ID == item.ID);
    }

    public void RefreshCustomAttributes(IEnumerable<CustomAttribute> customAttributes1, IEnumerable<CustomAttribute> customAttributes2, IEnumerable<CustomAttribute> customAttributes3)
    {
      if (customAttributes1 == null)
      {
        throw new ArgumentNullException(nameof(customAttributes1), "Argument can't be null");
      }

      if (customAttributes2 == null)
      {
        throw new ArgumentNullException(nameof(customAttributes2), "Argument can't be null");
      }

      if (customAttributes3 == null)
      {
        throw new ArgumentNullException(nameof(customAttributes3), "Argument can't be null");
      }

      CustomAttributes1.Clear();
      CustomAttributes2.Clear();
      CustomAttributes3.Clear();

      foreach (var customAttribute in customAttributes1)
      {
        CustomAttributes1.Add(customAttribute);
      }

      foreach (var customAttribute in customAttributes2)
      {
        CustomAttributes2.Add(customAttribute);
      }

      foreach (var customAttribute in customAttributes3)
      {
        CustomAttributes3.Add(customAttribute);
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Adds a schedule as baseline.
    /// </summary>
    public void AddBaseline(Schedule baseline, bool showInCurrentLayout)
    {
      Baselines.Add(baseline);

      if (showInCurrentLayout)
      {
        CurrentLayout.VisibleBaselines.Add(new VisibleBaseline(CurrentLayout, baseline));
      }
    }

    /// <summary>
    /// Adds a copy of the current schedule as baseline to itself.
    /// </summary>
    public Schedule AddBaseline(bool showInCurrentLayout)
    {
      var baseline = new Schedule(this);
      AddBaseline(baseline, showInCurrentLayout);
      return baseline;
    }

    /// <summary>
    /// Removes a baseline.
    /// </summary>
    public void RemoveBaseline(Schedule baseline)
    {
      baseline.BaselineOf = null;
      _ = Baselines.Remove(baseline);

      // Also remove the visible baseline defintion from the layouts.
      foreach (var layout in Layouts)
      {
        foreach (var visibleBaseline in layout.VisibleBaselines.Where(x => x.Schedule == baseline).ToList())
        {
          _ = layout.VisibleBaselines.Remove(visibleBaseline);
        }
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

    private static void CopySchedule(Schedule oldSchedule, Schedule newSchedule)
    {
      foreach (var c in oldSchedule.Calendars)
      {
        var newCalendar = new Calendar();
        newSchedule.Calendars.Add(newCalendar);
        newCalendar.Name = c.Name;
        newCalendar.Sunday = c.Sunday;
        newCalendar.Monday = c.Monday;
        newCalendar.Tuesday = c.Tuesday;
        newCalendar.Wednesday = c.Wednesday;
        newCalendar.Thursday = c.Thursday;
        newCalendar.Friday = c.Friday;
        newCalendar.Saturday = c.Saturday;
        newCalendar.IsStandard = c.IsStandard;
        foreach (var h in c.Holidays)
        {
          newCalendar.Holidays.Add(new Holiday() { Date = h.Date });
        }
      }
      foreach (var f in oldSchedule.Fragnets)
      {
        var newFragnet = new Fragnet();
        newSchedule.Fragnets.Add(newFragnet);
        newFragnet.Name = f.Name;
        newFragnet.Approved = f.Approved;
        newFragnet.Description = f.Description;
        newFragnet.Number = f.Number;
        newFragnet.Identified = f.Identified;
        newFragnet.IsDisputable = f.IsDisputable;
        newFragnet.Submitted = f.Submitted;
        newFragnet.IsVisible = f.IsVisible;
      }
      foreach (var r in oldSchedule.Resources)
      {
        Resource newResource;
        if (r is MaterialResource)
        {
          newResource = new MaterialResource();
          (newResource as MaterialResource).Unit = (r as MaterialResource).Unit;
        }
        else
        {
          newResource = r is WorkResource ? new WorkResource() : new CalendarResource();
        }

        newResource.Name = r.Name;
        newResource.Limit = r.Limit;
        newResource.CostsPerUnit = r.CostsPerUnit;
        newSchedule.Resources.Add(newResource);
      }
      CopyWBSItems(oldSchedule.WBSItem, newSchedule.WBSItem);
      foreach (var a in oldSchedule.Activities)
      {
        Activity newItem = null;
        if (a.ActivityType == ActivityType.Milestone)
        {
          newItem = a.IsFixed ? Activity.NewFixedMilestone() : Activity.NewMilestone();
        }
        else // a is activity
        {
          newItem = a.IsFixed ? Activity.NewFixedActivity() : Activity.NewActivity();
          newItem.OriginalDuration = a.OriginalDuration;
          newItem.Schedule = newSchedule;
        }
        newItem.Name = a.Name;
        newSchedule.AddActivity(newItem);
        if (a.Calendar != null)
        {
          newItem.Calendar = newSchedule.Calendars.ToList()[oldSchedule.Calendars.ToList().IndexOf(a.Calendar)];
        }

        newItem.ConstraintDate = a.ConstraintDate;
        newItem.EarlyStartDate = a.EarlyStartDate;
        newItem.LateStartDate = a.LateStartDate;
        newItem.EarlyFinishDate = a.EarlyFinishDate;
        newItem.LateFinishDate = a.LateFinishDate;
        newItem.ActualFinishDate = a.ActualFinishDate;
        newItem.ActualStartDate = a.ActualStartDate;
        newItem.Constraint = a.Constraint;
        if (a.Fragnet != null)
        {
          newItem.Fragnet = newSchedule.Fragnets.ToList()[oldSchedule.Fragnets.ToList().IndexOf(a.Fragnet)];
        }

        newItem.FreeFloat = a.FreeFloat;
        newItem.Number = a.Number;
        newItem.PercentComplete = a.PercentComplete;
        newItem.TotalFloat = a.TotalFloat;
        if (newSchedule.WBSItem != null && a.WBSItem != null)
        {
          newItem.WBSItem = FindWBSItem(newSchedule.WBSItem, a.WBSItem.Number);
        }

        if (a.ResourceAssociations != null)
        {
          foreach (var ra in a.ResourceAssociations)
          {
            var r = newSchedule.Resources.ToList()[oldSchedule.Resources.ToList().IndexOf(ra.Resource)];
            var newResourceAssociation = new ResourceAssociation(newItem, r);
            newResourceAssociation.Budget = ra.Budget;
            newResourceAssociation.FixedCosts = ra.FixedCosts;
            newResourceAssociation.UnitsPerDay = ra.UnitsPerDay;
          }
        }
        if (a.Distortions != null)
        {
          foreach (var d in a.Distortions)
          {
            Distortion newDistortion = null;
            if (d is Delay)
            {
              newDistortion = new Delay(newItem);
              (newDistortion as Delay).Days = (d as Delay).Days;
            }
            else if (d is Interruption)
            {
              newDistortion = new Interruption(newItem);
              (newDistortion as Interruption).Days = (d as Interruption).Days;
              (newDistortion as Interruption).Start = (d as Interruption).Start;
            }
            else if (d is Inhibition)
            {
              newDistortion = new Inhibition(newItem);
              (newDistortion as Inhibition).Percent = (d as Inhibition).Percent;
            }
            else if (d is Extension)
            {
              newDistortion = new Extension(newItem);
              (newDistortion as Extension).Days = (d as Extension).Days;
            }
            else if (d is Reduction)
            {
              newDistortion = new Reduction(newItem);
              (newDistortion as Reduction).Days = (d as Reduction).Days;
            }
            if (newDistortion != null)
            {
              newDistortion.Description = d.Description;
              if (d.Fragnet != null)
              {
                newDistortion.Fragnet = newSchedule.Fragnets.ToList()[oldSchedule.Fragnets.ToList().IndexOf(d.Fragnet)];
              }

              newItem.Distortions.Add(newDistortion);
            }
          }
        }
      }
      newSchedule.StartDate = oldSchedule.StartDate;
      newSchedule.DataDate = oldSchedule.DataDate;
      newSchedule.Description = oldSchedule.Description;
      newSchedule.EndDate = oldSchedule.EndDate;
      newSchedule.Name = oldSchedule.Name;
      foreach (var r in oldSchedule.Relationships)
      {
        var newRelationship = new Relationship(newSchedule.GetActivity(r.Activity1.Number), newSchedule.GetActivity(r.Activity2.Number));
        newRelationship.Lag = r.Lag;
        newRelationship.RelationshipType = r.RelationshipType;
      }
      foreach (var layout in oldSchedule.Layouts)
      {
        var newLayout = new Layout();
        newSchedule.Layouts.Add(newLayout);
        CopyLayoutData(layout, newLayout);
      }
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

    private static void CopyWBSItems(WBSItem parent, WBSItem copyTo)
    {
      if (parent != null && copyTo != null)
      {
        foreach (var item in parent.Children.ToArray())
        {
          var newItem = new WBSItem(parent);
          newItem.Number = item.Number;
          newItem.Name = item.Name;
          copyTo.Children.Add(newItem);
          CopyWBSItems(item, newItem);
        }
      }
    }

    private static void CopyLayoutData(Layout oldLayout, Layout newLayout)
    {
      newLayout.ActivityCriticalColor = oldLayout.ActivityCriticalColor;
      newLayout.ActivityDoneColor = oldLayout.ActivityDoneColor;
      newLayout.ActivityStandardColor = oldLayout.ActivityStandardColor;
      newLayout.CenterText = oldLayout.CenterText;
      newLayout.IsCurrent = oldLayout.IsCurrent;
      newLayout.DataDateColor = oldLayout.DataDateColor;
      newLayout.FilterCombination = oldLayout.FilterCombination;
      newLayout.LeftText = oldLayout.LeftText;
      newLayout.MilestoneCriticalColor = oldLayout.MilestoneCriticalColor;
      newLayout.MilestoneDoneColor = oldLayout.MilestoneDoneColor;
      newLayout.MilestoneStandardColor = oldLayout.MilestoneStandardColor;
      newLayout.Name = $"{oldLayout.Name} ({NASResources.Copy})";
      newLayout.RightText = oldLayout.RightText;
      newLayout.ShowRelationships = oldLayout.ShowRelationships;
      newLayout.ShowFloat = oldLayout.ShowFloat;
      foreach (var g in oldLayout.GroupingDefinitions)
      {
        var definition = new GroupingDefinition(g.Property);
        definition.Color = g.Color;
        newLayout.GroupingDefinitions.Add(definition);
      }
      foreach (var s in oldLayout.SortingDefinitions)
      {
        var definition = new SortingDefinition(s.Property);
        definition.Direction = s.Direction;
        newLayout.SortingDefinitions.Add(definition);
      }
      foreach (var col in oldLayout.ActivityColumns)
      {
        newLayout.ActivityColumns.Add(new ActivityColumn(col.Property) { ColumnWidth = col.ColumnWidth, Order = col.Order });
      }
      foreach (var f in oldLayout.FilterDefinitions)
      {
        var filter = new FilterDefinition(f.Property);
        filter.ObjectString = f.ObjectString;
        filter.Relation = f.Relation;
        newLayout.FilterDefinitions.Add(filter);
      }
      foreach (var b in oldLayout.VisibleBaselines)
      {
        var baseline = new VisibleBaseline(newLayout, b.Schedule);
        baseline.Color = b.Color;
        newLayout.VisibleBaselines.Add(baseline);
      }
      foreach (var r in oldLayout.VisibleResources)
      {
        var resource = new VisibleResource(newLayout, r.Resource);
        resource.ShowBudget = r.ShowBudget;
        resource.ShowActualCosts = r.ShowActualCosts;
        resource.ShowPlannedCosts = r.ShowPlannedCosts;
        resource.ShowResourceAllocation = r.ShowResourceAllocation;
        newLayout.VisibleResources.Add(resource);
      }
    }

    #endregion
  }
}
