using System.Diagnostics;
using NAS.Models.Enums;
using NAS.Resources;

namespace NAS.Models.Entities
{
  [DebuggerDisplay("{Number} {Name}")]
  public class Milestone : Activity
  {
    public Milestone(Schedule schedule, bool isFixed = false)
      : base(schedule, isFixed)
    {
      Name = NASResources.NewMilestone;
    }

    public override ActivityType ActivityType => ActivityType.Milestone;

    public override int OriginalDuration
    {
      get => 0;
      set { }
    }

    public override int RemainingDuration
    {
      get => 0;
      set { }
    }

    public override int ActualDuration => 0;

    public Activity ChangeToActivity()
    {
      var schedule = Schedule;
      var predecessors = GetPreceedingRelationships().ToList();
      var successors = GetSucceedingRelationships().ToList();
      var newActivity = schedule.AddActivity(IsFixed);
      newActivity.Number = Number;
      newActivity.Name = Name;
      newActivity.Calendar = Calendar;
      newActivity.Constraint = Constraint;
      newActivity.ConstraintDate = ConstraintDate;
      newActivity.Constraint = Constraint;
      newActivity.CustomAttribute1 = CustomAttribute1;
      newActivity.CustomAttribute2 = CustomAttribute2;
      newActivity.CustomAttribute3 = CustomAttribute3;
      newActivity.EarlyStartDate = EarlyStartDate;
      newActivity.LateStartDate = LateStartDate;
      newActivity.WBSItem = WBSItem;
      newActivity.Fragnet = Fragnet;

      if (PercentComplete == 100)
      {
        newActivity.PercentComplete = 100;
      }

      foreach (var r in predecessors)
      {
        schedule.AddRelationship(r.Activity1, newActivity, r.RelationshipType).Lag = r.Lag;
        schedule.RemoveRelationship(r);
      }

      foreach (var r in successors)
      {
        schedule.AddRelationship(newActivity, r.Activity2, r.RelationshipType).Lag = r.Lag;
        schedule.RemoveRelationship(r);
      }

      return newActivity;
    }
  }
}
