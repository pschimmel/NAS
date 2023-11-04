using System;
using System.Collections.Generic;
using System.Linq;
using NAS.Model.Base;
using NAS.Model.Entities;
using NAS.Model.Enums;

namespace NAS.Model.Scheduler
{
  public class Scheduler
  {
    #region Events

    public event EventHandler<MessageEventArgs> Message;

    #endregion

    #region Fields

    public delegate void OnCalculationStarted();
    public delegate void OnCalculationEnded();
    public delegate void OnCalculationProgress(double progress);
    private readonly Schedule _schedule;
    private List<Activity> scheduledActivities = null;
    private DateTime projectEndDate;
    private readonly SchedulingSettings _settings;
    private readonly OnCalculationStarted calculationStarted = null;
    private readonly OnCalculationEnded calculationEnded = null;
    private readonly OnCalculationProgress calculationProgress = null;

    #endregion

    #region Constructors

    public Scheduler(Schedule schedule, SchedulingSettings settings)
    {
      _schedule = schedule ?? throw new ArgumentException("Schedule may not be null");
      _settings = settings ?? throw new ArgumentException("Settings may not be null");
      RelationshipCalendar = RelationshipCalendarType.Predecessor;
      scheduledActivities = new List<Activity>();
    }

    public Scheduler(Schedule schedule, SchedulingSettings settings, OnCalculationStarted calculationStarted, OnCalculationEnded calculationEnded, OnCalculationProgress calculationProgress)
      : this(schedule, settings)
    {
      this.calculationStarted = calculationStarted;
      this.calculationEnded = calculationEnded;
      this.calculationProgress = calculationProgress;
    }

    #endregion

    #region Public Members

    public static RelationshipCalendarType RelationshipCalendar { get; set; }

    public enum RelationshipCalendarType { Predecessor, Successor }

    public void Calculate(DateTime date)
    {
      _schedule.DataDate = date;
      Calculate();
    }

    public bool Calculate()
    {
      calculationStarted?.Invoke();
      try
      {
        if (!CheckForLoops())
        {
          OnMessage("Es wurde eine Schleife entdeckt.");
          return false;
        }
        CalculateForward();
        SetEndDate();
        CalculateBackward();
        CalculateFloat();
        CalculateActivityDates();
        SetCriticalPath();
        CalculateRelationshipDates();
      }
      catch (Exception ex)
      {
        OnMessage(ex.Message);
        return false;
      }
      finally
      {
        scheduledActivities = null;
        calculationEnded?.Invoke();
      }
      return true;
    }

    private void SetEndDate()
    {
      if (_schedule.EndDate.HasValue)
      {
        projectEndDate = _schedule.EndDate.Value;
      }
      else
      {
        projectEndDate = _schedule.StartDate;
        foreach (Activity a in scheduledActivities)
        {
          if (a.EarlyFinishDate > projectEndDate)
          {
            projectEndDate = a.EarlyFinishDate;
          }
        }
      }
    }

    #endregion

    #region Forward Calculation

    private void CalculateForward()
    {
      scheduledActivities.Clear();

      // Do a forward calculation for activities that have no predecessors only.
      foreach (Activity a in _schedule.Activities.Where(x => x.GetVisiblePredecessorCount() == 0))
      {
        CalculateForward2(a);
      }
    }

    //private void calculateForward(Activity activity) {
    //  if (activity.Fragnet != null && !activity.Fragnet.IsVisible)
    //    return;
    //  if (activity.GetType() != typeof(FixedActivity) && activity.GetType() != typeof(FixedMilestone)) { // Do not calculate fixed activities or fixed milestones
    //    DateTime dataDate = schedule.DataDate;
    //    while (!activity.Calendar.IsWorkingDay(dataDate)) // Ensure data day is workday!
    //      dataDate = dataDate.AddDays(1);
    //    // If activity has no predecessors it is the beginning of the schedule so the early start day is the data day
    //    activity.EarlyStartDate = dataDate;
    //    List<Relationship> predecessors = activity.GetVisiblePredecessors();
    //    if (predecessors.Count > 0) { // else calculate early start day based on predecessing activities
    //      activity.EarlyStartDate = dataDate;
    //      foreach (Relationship r in predecessors) {
    //        // Skip predecessing activities in invisible fragnets
    //        if (r.Activity1.Fragnet != null && !r.Activity1.Fragnet.IsVisible)
    //          continue;
    //        // Check if predecessor activity has been calculated before. Performance!
    //        if (!scheduledActivities.Contains(r.Activity1))
    //          calculateForward(r.Activity1);
    //        // Calculate start day based on relationship type
    //        DateTime startDate = r.Activity1.EarlyStartDate;
    //        DateTime finishDate = r.Activity1.EarlyFinishDate;
    //        if (r.Activity1.ActualFinishDate.HasValue)
    //          startDate = r.Activity1.ActualFinishDate.Value;
    //        if (r.Activity1.ActualFinishDate.HasValue)
    //          finishDate = r.Activity1.ActualFinishDate.Value;
    //        switch (r.RelationshipType) {
    //          case RelationshipType.FinishStart:
    //            if (r.Activity1.Calendar.GetEndDate(finishDate, r.Lag + 2) > activity.EarlyStartDate)
    //              activity.EarlyStartDate = r.Activity1.Calendar.GetEndDate(finishDate, r.Lag + 2);
    //            break;
    //          case RelationshipType.StartStart:
    //            if (r.Activity1.Calendar.GetEndDate(startDate, r.Lag) > activity.EarlyStartDate)
    //              activity.EarlyStartDate = r.Activity1.Calendar.GetEndDate(startDate, r.Lag);
    //            break;
    //          case RelationshipType.FinishFinish:
    //            if (r.Activity1.Calendar.GetEndDate(finishDate, r.Lag) > activity.EarlyFinishDate)
    //              activity.EarlyFinishDate = r.Activity1.Calendar.GetEndDate(finishDate, r.Lag);
    //            break;
    //        }
    //      }
    //    }
    //    // Check for constraints
    //    if (activity.Constraint != ConstraintType.None && activity.ConstraintDate.HasValue) {
    //      switch (activity.Constraint) {
    //        case ConstraintType.StartOn:
    //          activity.EarlyStartDate = activity.ConstraintDate.Value;
    //          break;
    //        case ConstraintType.EndOn:
    //          activity.EarlyFinishDate = activity.ConstraintDate.Value;
    //          break;
    //        case ConstraintType.StartOnOrLater:
    //          if (activity.EarlyStartDate < activity.ConstraintDate.Value)
    //            activity.EarlyStartDate = activity.ConstraintDate.Value;
    //          break;
    //        case ConstraintType.EndOnOrEarlier:
    //          if (activity.EarlyFinishDate > activity.ConstraintDate.Value)
    //            activity.EarlyFinishDate = activity.ConstraintDate.Value;
    //          break;
    //      }
    //    }
    //  }
    //  // All calcs are done. Add activity to scheduled activity list
    //  if (!scheduledActivities.Contains(activity))
    //    scheduledActivities.Add(activity);
    //  // Cast Progress event
    //  if (calculationProgress != null)
    //    calculationProgress((double)scheduledActivities.Count * 50 / (double)schedule.Activities.Count);
    //  // Do the same calculation with sucessing activities
    //  foreach (Relationship r in activity.GetVisibleSuccessors()) {
    //    if (!scheduledActivities.Contains(r.Activity2))
    //      calculateForward(r.Activity2);
    //  }
    //}

    private void CalculateForward2(Activity activity)
    {
      if (scheduledActivities.Contains(activity) || activity.Fragnet != null && !activity.Fragnet.IsVisible)
      {
        return;
      }

      if (!activity.IsFixed)
      { // Do not calculate fixed activities or fixed milestones
        DateTime startDate = EnsureStartDayIsWorkday(_schedule.DataDate, activity.Calendar);
        if (activity.Constraint != ConstraintType.None && activity.ConstraintDate.HasValue)
        {
          switch (activity.Constraint)
          {
            case ConstraintType.StartOn:
              startDate = activity.ConstraintDate.Value;
              break;
            case ConstraintType.EndOn:
              startDate = activity.Calendar.GetStartDate(activity.ConstraintDate.Value, activity.RemainingDuration);
              break;
            case ConstraintType.StartOnOrLater:
              if (startDate < activity.ConstraintDate.Value)
              {
                startDate = activity.ConstraintDate.Value;
              }

              startDate = CalculatePredecessors(activity, startDate);
              break;
            case ConstraintType.EndOnOrEarlier:
              startDate = CalculatePredecessors(activity, startDate);
              DateTime constraintStartDate = activity.Calendar.GetStartDate(activity.ConstraintDate.Value, activity.RemainingDuration);
              if (startDate > constraintStartDate)
              {
                startDate = constraintStartDate;
              }

              break;
          }
        }
        else
        {
          startDate = CalculatePredecessors(activity, startDate);
        }

        activity.EarlyStartDate = startDate;
      }
      // All calcs are done. Add activity to scheduled activity list
      if (!scheduledActivities.Contains(activity))
      {
        scheduledActivities.Add(activity);
      }
      // Cast Progress event
      calculationProgress?.Invoke(Convert.ToDouble(scheduledActivities.Count) * 50d / Convert.ToDouble(_schedule.Activities.Count));
      // Do the same calculation with sucessing activities
      foreach (Activity successor in activity.GetVisibleSuccessors())
      {
        CalculateForward2(successor);
      }
    }

    private DateTime CalculatePredecessors(Activity activity, DateTime startDate)
    {
      IEnumerable<Activity> predecessors = activity.GetVisiblePredecessors();

      foreach (Activity predecessor in predecessors.ToList())
      {
        if (predecessor.Fragnet != null && !predecessor.Fragnet.IsVisible)
        {
          continue;
        }

        // Calculate predecessor first
        CalculateForward2(predecessor);

        // Calculate start day based on relationship type
        DateTime predecessorStartDate = predecessor.EarlyStartDate;
        DateTime predecessorFinishDate = predecessor.EarlyFinishDate;
        if (predecessor.ActualFinishDate.HasValue)
        {
          predecessorStartDate = predecessor.ActualFinishDate.Value;
          predecessorFinishDate = predecessor.ActualFinishDate.Value;
        }
        DateTime? newStartDate = null;

        Calendar calendar = RelationshipCalendar == RelationshipCalendarType.Predecessor ? predecessor.Calendar : activity.Calendar;

        Relationship relationship = _schedule.GetRelationship(predecessor, activity);

        switch (relationship.RelationshipType)
        {
          case RelationshipType.FinishStart:
            newStartDate = calendar.GetEndDate(predecessorFinishDate, relationship.Lag + 2);
            break;
          case RelationshipType.StartStart:
            newStartDate = calendar.GetEndDate(predecessorStartDate, relationship.Lag + 1);
            break;
          case RelationshipType.FinishFinish:
            newStartDate = calendar.GetEndDate(predecessorFinishDate, relationship.Lag + 1);
            newStartDate = activity.Calendar.GetStartDate(newStartDate.Value, activity.RemainingDuration);
            break;
        }
        if (newStartDate > startDate)
        {
          startDate = newStartDate.Value;
        }
      }
      return startDate;
    }

    private static DateTime EnsureStartDayIsWorkday(DateTime date, Calendar calendar)
    {
      while (!calendar.IsWorkDay(date))
      {
        date = date.AddDays(1);
      }

      return date;
    }

    #endregion

    #region Backward Calculation

    private void CalculateBackward()
    {
      scheduledActivities.Clear();

      // Start backward calculation with activities that do not have successors
      foreach (Activity a in _schedule.Activities.Where(x => x.GetVisibleSuccessorCount() == 0).ToList())
      {
        CalculateBackward2(a);
      }
    }

    //private void calculateBackward(Activity activity) {
    //  if (activity.Fragnet != null && !activity.Fragnet.IsVisible)
    //    return;
    //  if (!(activity is FixedActivity || activity is FixedMilestone)) { // Do not calculate fixed activities or fixed milestones
    //    // If activity has no successors it is the end of the schedule so the late finish day is the end day
    //    activity.LateFinishDate = projectEndDate;
    //    List<Relationship> successors = activity.GetVisibleSuccessors();
    //    if (successors.Count > 0) { // else calculate late end day based on successing activities
    //      foreach (Relationship r in successors) {
    //        // Skip successing activities in invisible fragnets
    //        if (r.Activity2.Fragnet != null && !r.Activity2.Fragnet.IsVisible)
    //          continue;
    //        // Check if successor activity has been calculated before. Performance!
    //        if (!scheduledActivities.Contains(r.Activity2))
    //          calculateBackward(r.Activity2);
    //        // Calculate start day based on relationship type
    //        DateTime startDate = r.Activity2.LateFinishDate;
    //        DateTime finishDate = r.Activity2.LateFinishDate;
    //        if (r.Activity2.ActualFinishDate.HasValue)
    //          startDate = r.Activity2.ActualFinishDate.Value;
    //        if (r.Activity2.ActualFinishDate.HasValue)
    //          finishDate = r.Activity2.ActualFinishDate.Value;
    //        switch (r.RelationshipType) {
    //          case RelationshipType.FinishStart:
    //            if (r.Activity1.Calendar.GetStartDate(r.Activity2.LateStartDate, r.Lag + 2) < activity.LateFinishDate)
    //              activity.LateFinishDate = r.Activity1.Calendar.GetStartDate(r.Activity2.LateStartDate, r.Lag + 2);
    //            break;
    //          case RelationshipType.StartStart:
    //            if (r.Activity1.Calendar.GetStartDate(r.Activity2.LateStartDate, r.Lag) < activity.LateStartDate)
    //              activity.LateStartDate = r.Activity1.Calendar.GetStartDate(r.Activity2.LateStartDate, r.Lag);
    //            break;
    //          case RelationshipType.FinishFinish:
    //            if (r.Activity1.Calendar.GetStartDate(r.Activity2.LateFinishDate, r.Lag) < activity.LateFinishDate)
    //              activity.LateFinishDate = r.Activity1.Calendar.GetStartDate(r.Activity2.LateFinishDate, r.Lag);
    //            break;
    //        }
    //      }
    //    }
    //    // Check for constraints
    //    if (activity.Constraint != ConstraintType.None && activity.ConstraintDate.HasValue) {
    //      switch (activity.Constraint) {
    //        case ConstraintType.StartOn:
    //          activity.LateStartDate = activity.ConstraintDate.Value;
    //          break;
    //        case ConstraintType.EndOn:
    //          activity.LateFinishDate = activity.ConstraintDate.Value;
    //          break;
    //        case ConstraintType.StartOnOrLater:
    //          if (activity.LateStartDate < activity.ConstraintDate.Value)
    //            activity.LateStartDate = activity.ConstraintDate.Value;
    //          break;
    //        case ConstraintType.EndOnOrEarlier:
    //          if (activity.LateFinishDate > activity.ConstraintDate.Value)
    //            activity.LateFinishDate = activity.ConstraintDate.Value;
    //          break;
    //      }
    //    }
    //  }
    //  // All calcs are done. Add activity to scheduled activity list
    //  if (!scheduledActivities.Contains(activity))
    //    scheduledActivities.Add(activity);
    //  // Cast Progress event
    //  if (calculationProgress != null)
    //    calculationProgress((double)scheduledActivities.Count * 50 / (double)schedule.Activities.Count + 50);
    //  // Do the same calculation with predecessing activities
    //  foreach (Relationship r in activity.GetVisiblePredecessors()) {
    //    if (!scheduledActivities.Contains(r.Activity1))
    //      calculateBackward(r.Activity1);
    //  }
    //}

    private void CalculateBackward2(Activity activity)
    {
      if (scheduledActivities.Contains(activity) || activity.Fragnet != null && !activity.Fragnet.IsVisible)
      {
        return;
      }

      if (!activity.IsFixed)
      { // Do not calculate fixed activities or fixed milestones
        DateTime endDate = EnsureEndDayIsWorkday(projectEndDate, activity.Calendar);
        if (activity.Constraint != ConstraintType.None && activity.ConstraintDate.HasValue)
        {
          switch (activity.Constraint)
          {
            case ConstraintType.StartOn:
              endDate = activity.Calendar.GetEndDate(activity.ConstraintDate.Value, activity.RemainingDuration);
              break;
            case ConstraintType.EndOn:
              endDate = activity.ConstraintDate.Value;
              break;
            case ConstraintType.StartOnOrLater:
              endDate = CalculateSuccessors(activity, endDate);
              DateTime constraintEndDate = activity.Calendar.GetStartDate(activity.ConstraintDate.Value, activity.RemainingDuration);
              if (endDate < constraintEndDate)
              {
                endDate = constraintEndDate;
              }

              break;
            case ConstraintType.EndOnOrEarlier:
              if (endDate > activity.ConstraintDate.Value)
              {
                endDate = activity.ConstraintDate.Value;
              }

              endDate = CalculateSuccessors(activity, endDate);
              break;
          }
        }
        else
        {
          endDate = CalculateSuccessors(activity, endDate);
        }

        activity.LateFinishDate = endDate;
      }
      // All calcs are done. Add activity to scheduled activity list
      if (!scheduledActivities.Contains(activity))
      {
        scheduledActivities.Add(activity);
      }
      // Cast Progress event
      calculationProgress?.Invoke(Convert.ToDouble(scheduledActivities.Count) * 50d / Convert.ToDouble(_schedule.Activities.Count) + 50d);
      // Do the same calculation with predecessing activities
      foreach (Activity predecessor in activity.GetVisiblePredecessors())
      {
        CalculateBackward2(predecessor);
      }
    }

    private DateTime CalculateSuccessors(Activity activity, DateTime endDate)
    {
      IEnumerable<Activity> successors = activity.GetVisibleSuccessors();

      foreach (Activity successor in successors)
      {
        if (successor.Fragnet != null && !successor.Fragnet.IsVisible)
        {
          continue;
        }

        // Calculate sucessors first
        CalculateBackward2(successor);

        // Calculate start day based on relationship type
        DateTime successorStartDate = successor.LateStartDate;
        DateTime successorFinishDate = successor.LateFinishDate;
        if (successor.ActualStartDate.HasValue)
        {
          successorFinishDate = successor.ActualStartDate.Value;
          successorStartDate = successor.ActualStartDate.Value;
        }

        DateTime? newEndDate = null;
        Calendar calendar = RelationshipCalendar == RelationshipCalendarType.Predecessor ? activity.Calendar : successor.Calendar;
        Relationship relationship = _schedule.GetRelationship(activity, successor);

        switch (relationship.RelationshipType)
        {
          case RelationshipType.FinishStart:
            newEndDate = calendar.GetStartDate(successorStartDate, relationship.Lag + 2);
            break;
          case RelationshipType.StartStart:
            newEndDate = calendar.GetStartDate(successorStartDate, relationship.Lag + 1);
            newEndDate = activity.Calendar.GetEndDate(newEndDate.Value, activity.RemainingDuration);
            break;
          case RelationshipType.FinishFinish:
            newEndDate = calendar.GetEndDate(successorFinishDate, relationship.Lag + 1);
            break;
        }
        if (newEndDate < endDate)
        {
          endDate = newEndDate.Value;
        }
      }
      return endDate;
    }

    private static DateTime EnsureEndDayIsWorkday(DateTime date, Calendar calendar)
    {
      while (!calendar.IsWorkDay(date))
      {
        date = date.AddDays(-1);
      }

      return date;
    }

    #endregion

    #region Float Calculation

    private void CalculateFloat()
    {
      foreach (Activity activity in _schedule.Activities)
      {
        if (activity.Fragnet != null && !activity.Fragnet.IsVisible)
        {
          continue;
        }

        activity.TotalFloat = activity.IsFinished ? 0 : activity.Calendar.GetWorkDays(activity.EarlyFinishDate, activity.LateFinishDate, false);
        int freeFloat = activity.TotalFloat;
        IEnumerable<Activity> successors = activity.GetVisibleSuccessors();

        foreach (Activity successor in successors)
        {
          Relationship relationship = _schedule.GetRelationship(activity, successor);

          switch (relationship.RelationshipType)
          {
            case RelationshipType.FinishStart:
              freeFloat = Math.Min(freeFloat, activity.Calendar.GetWorkDays(activity.EarlyFinishDate, successor.EarlyStartDate, false) - relationship.Lag - 1);
              break;
            case RelationshipType.StartStart:
              freeFloat = Math.Min(freeFloat, activity.Calendar.GetWorkDays(activity.EarlyStartDate, successor.EarlyStartDate, false) - relationship.Lag);
              break;
            case RelationshipType.FinishFinish:
              freeFloat = Math.Min(freeFloat, activity.Calendar.GetWorkDays(activity.EarlyFinishDate, successor.EarlyFinishDate, false) - relationship.Lag);
              break;
          }
        }

        activity.FreeFloat = freeFloat;
      }
    }

    #endregion

    #region Checks

    /// <summary>
    /// Checks for loops.
    /// </summary>
    /// <returns>true, if no loop was found</returns>
    public bool CheckForLoops()
    {
      var activities = _schedule.Activities.Where(x => x.Fragnet == null || x.Fragnet.IsVisible).OrderBy(x => x.ID).ToList();
      return CheckForLoops(activities, _schedule.Relationships.ToList());
    }

    /// <summary>
    /// Checks for loops.
    /// </summary>
    /// <param number="relationships">The relationships to check.</param>
    /// <returns>true, if no loop was found</returns>
    public static bool CheckForLoops(List<Activity> activities, List<Relationship> relationships)
    {
      if (activities == null)
      {
        throw new ArgumentException("Activities cannot be null.");
      }

      if (relationships == null)
      {
        throw new ArgumentException("Relationships cannot be null.");
      }

      if (relationships.Count <= 1 || activities.Count == 0)
      {
        return true;
      }

      Schedule schedule = activities[0].Schedule;
      var relatedActivities = new List<(Activity, Activity)>();

      foreach (Relationship relationship in relationships)
      {
        if (activities.Any(x => x.Guid == relationship.Activity1Guid) && activities.Any(x => x.Guid == relationship.Activity2Guid))
        {
          relatedActivities.Add((schedule.GetActivity(relationship.Activity1Guid), schedule.GetActivity(relationship.Activity2Guid)));
        }
      }

      int[,] matrix = new int[activities.Count, activities.Count];
      int[] vector = new int[activities.Count];

      // Set up matrix and vector
      foreach ((Activity activity1, Activity activity2) in relatedActivities)
      {
        if (activity1 != null && activity2 != null)
        {
          int idx1 = activities.IndexOf(activity1);
          int idx2 = activities.IndexOf(activity2);
          matrix[idx1, idx2] = 1;
          vector[idx2]++;
        }
      }

      var deletedColumns = new List<int>();
      bool newColumn = true;

      // Reduce matrix colunns
      while (deletedColumns.Count < activities.Count && newColumn == true)
      {
        newColumn = false;
        for (int i = 0; i < activities.Count; i++)
        {
          if (vector[i] == 0 && !deletedColumns.Contains(i))
          {
            // Sum is zero
            for (int j = 0; j < activities.Count; j++)
            {
              vector[j] -= matrix[i, j];
            }

            deletedColumns.Add(i);
            newColumn = true;
          }
        }
      }

      return deletedColumns.Count == activities.Count;
    }

    #endregion

    #region Activity Dates

    public void CalculateActivityDates()
    {
      foreach (Activity activity in _schedule.Activities)
      {
        RefreshDates(activity);
      }
    }

    public static void RefreshDates(Activity activity)
    {
      int remainingDuration = Convert.ToInt32(Convert.ToDouble(activity.RetardedDuration) * (100d - activity.PercentComplete) / 100d);
      if (remainingDuration != activity.RemainingDuration)
      {
        activity.RemainingDuration = remainingDuration;
      }

      if (activity.Calendar != null)
      {
        DateTime earlyFinishDate = activity.Calendar.GetEndDate(activity.EarlyStartDate, activity.RemainingDuration);
        if (activity.EarlyFinishDate != earlyFinishDate)
        {
          activity.EarlyFinishDate = earlyFinishDate;
        }

        DateTime lateStartDate = activity.Calendar.GetStartDate(activity.LateFinishDate, activity.RemainingDuration);
        if (activity.LateStartDate != lateStartDate)
        {
          activity.LateStartDate = lateStartDate;
        }
      }
    }

    private void SetCriticalPath()
    {
      if (_settings.CriticalPath == CriticalPathDefinition.FloatZeroOrLess)
      {
        foreach (Activity activity in _schedule.Activities)
        {
          activity.IsCritical = activity.TotalFloat <= 0 && !activity.IsFinished;
        }
      }
      else // Longest Path
      {
        foreach (Activity activity in _schedule.Activities)
        {
          activity.IsCritical = false;
        }

        var paths = _schedule.Activities.Where(x => !x.GetPreceedingRelationships().Any())
                                              .Select(x => GetLongestPath(x)).ToList();
        var longestPaths = paths.Where(x => x.Length == paths.Max(y => y.Length)).ToList();
        SetLongestPathCritical(longestPaths);
      }
    }

    private static PathItem GetLongestPath(Activity parent)
    {
      var item = new PathItem(parent);

      foreach (Relationship relationship in parent.GetSucceedingRelationships())
      {
        Activity successor = relationship.GetActivity2();
        int count = relationship.Lag + successor.AtCompletionDuration;

        if (count >= item.Length)
        {
          if (count > item.Length)
          {
            item.Children.Clear();
            item.Length = count;
          }

          item.Children.Add(GetLongestPath(successor));
        }
      }

      return item;
    }

    private void SetLongestPathCritical(List<PathItem> longestPaths)
    {
      foreach (PathItem path in longestPaths)
      {
        Activity activity =     _schedule.GetActivity(path.ActivityID);
        activity.IsCritical = true;
        SetLongestPathCritical(path.Children);
      }
    }

    public void CalculateRelationshipDates()
    {
      foreach (Relationship relationship in _schedule.Relationships)
      {
        RefreshDates(relationship);
      }
    }

    public static void RefreshDates(Relationship relationship)
    {
      // IsDriving
      if (relationship.Activity1Guid == Guid.Empty || relationship.Activity2Guid == Guid.Empty)
      {
        relationship.IsDriving = false;
        relationship.IsCritical = false;
        return;
      }

      Activity activity1 = relationship.GetActivity1();
      Activity activity2 = relationship.GetActivity2();

      if (activity1.Calendar == null)
      {
        relationship.IsDriving = false;
        return;
      }

      switch (relationship.RelationshipType)
      {
        case RelationshipType.FinishStart:
          relationship.IsDriving = activity1.Calendar.GetWorkDays(activity1.EarlyFinishDate, activity2.EarlyStartDate, false) - relationship.Lag - 1 <= 0;
          break;
        case RelationshipType.StartStart:
          relationship.IsDriving = activity1.Calendar.GetWorkDays(activity1.EarlyStartDate, activity2.EarlyStartDate, false) - relationship.Lag <= 0;
          break;
        case RelationshipType.FinishFinish:
          relationship.IsDriving = activity1.Calendar.GetWorkDays(activity1.EarlyFinishDate, activity2.EarlyFinishDate, false) - relationship.Lag <= 0;
          break;
        default:
          relationship.IsDriving = false;
          break;
      }

      // IsCritical
      relationship.IsCritical = activity1.IsCritical && activity2.IsCritical && relationship.IsDriving;
    }

    #endregion

    #region Private Members

    protected void OnMessage(string text)
    {
      Message?.Invoke(this, new MessageEventArgs(text));
    }

    #endregion
  }
}
