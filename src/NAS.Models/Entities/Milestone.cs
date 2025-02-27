using System.Diagnostics;
using NAS.Models.Enums;
using NAS.Resources;

namespace NAS.Models.Entities
{
  [DebuggerDisplay("{Number} {Name}")]
  public class Milestone : Activity
  {
    #region Constructors

    public Milestone(bool isFixed = false)
      : base(isFixed)
    {
      Name = NASResources.NewMilestone;
    }

    public Milestone(Milestone other)
      : base(other)
    {
    }

    #endregion

    #region Properties

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

    #endregion

    #region ICloneable

    public override Activity Clone()
    {
      return new Milestone(this);
    }

    #endregion
  }
}
