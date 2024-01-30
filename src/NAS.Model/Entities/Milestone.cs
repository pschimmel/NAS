using System.Diagnostics;
using NAS.Model.Enums;
using NAS.Resources;

namespace NAS.Model.Entities
{
  [DebuggerDisplay("{Number} {Name}")]
  public class Milestone : Activity
  {
    public Milestone(bool isFixed = false)
      : base(isFixed)
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
        var newRelationship = new Relationship(r.Activity1, newActivity);
        newRelationship.RelationshipType = r.RelationshipType;
        newRelationship.Lag = r.Lag;
      }
      foreach (var r in successors)
      {
        var newRelationship = new Relationship(newActivity, r.Activity2);
        newRelationship.RelationshipType = r.RelationshipType;
        newRelationship.Lag = r.Lag;
      }

      return newActivity;
    }
  }
}
