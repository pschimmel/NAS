
namespace NAS.Model.Enums
{
  public enum ConstraintType
  {
    /// <summary>No Constraint</summary>
    None,
    /// <summary>Activity must start on the day</summary>
    StartOn,
    /// <summary>Activity must start on th day or later</summary>
    StartOnOrLater,
    /// <summary>Activity must end on the day</summary>
    EndOn,
    /// <summary>Activity must end on the day or earlier</summary>
    EndOnOrEarlier
  }
}
