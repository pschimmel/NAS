using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using NAS.Model.Enums;

namespace NAS.Model.Entities
{
  [DebuggerDisplay("{GetActivity1().Number} - {GetActivity2().Number}")]
  public class Relationship : NASObject
  {
    private RelationshipType relationshipType;
    private int lag;

    public Relationship()
    {
      lag = 0;
      relationshipType = RelationshipType.FinishStart;
    }

    public virtual Schedule Schedule { get; set; }

    public RelationshipType RelationshipType
    {
      get => relationshipType;
      set
      {
        if (relationshipType != value)
        {
          relationshipType = value;
          OnPropertyChanged(nameof(RelationshipType));
        }
      }
    }

    public int Lag
    {
      get => lag;
      set
      {
        if (lag != value)
        {
          lag = value;
          OnPropertyChanged(nameof(Lag));
        }
      }
    }

    public string Activity1ID
    {
      get => Activity1Guid.ToString();
      set => Activity1Guid = Guid.Parse(value);
    }

    [NotMapped]
    public Guid Activity1Guid { get; set; } = Guid.Empty;

    public string Activity2ID
    {
      get => Activity2Guid.ToString();
      set => Activity2Guid = Guid.Parse(value);
    }

    [NotMapped]
    public Guid Activity2Guid { get; set; } = Guid.Empty;

    public Activity GetActivity1()
    {
      return Schedule.GetActivity(Activity1Guid);
    }

    public Activity GetActivity2()
    {
      return Schedule.GetActivity(Activity2Guid);
    }

    public bool IsDriving { get; set; }

    public bool IsCritical { get; set; }
  }
}
