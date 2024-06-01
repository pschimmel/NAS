using System.Diagnostics;
using NAS.Models.Enums;

namespace NAS.Models.Entities
{
  [DebuggerDisplay("{Activity1.Number} - {Activity2.Number}")]
  public class Relationship : NASObject
  {
    private RelationshipType _relationshipType;
    private int _lag;

    public Relationship(Activity activity1, Activity activity2, RelationshipType type = RelationshipType.FinishStart, int lag = 0)
    {
      Activity1 = activity1 ?? throw new ArgumentNullException(nameof(activity1));
      Activity2 = activity2 ?? throw new ArgumentNullException(nameof(activity2));
      _lag = lag;
      _relationshipType = type;
    }

    public RelationshipType RelationshipType
    {
      get => _relationshipType;
      set
      {
        if (_relationshipType != value)
        {
          _relationshipType = value;
          OnPropertyChanged(nameof(RelationshipType));
        }
      }
    }

    public int Lag
    {
      get => _lag;
      set
      {
        if (_lag != value)
        {
          _lag = value;
          OnPropertyChanged(nameof(Lag));
        }
      }
    }

    public Activity Activity1 { get; }

    public Activity Activity2 { get; }

    public bool IsDriving { get; set; }

    public bool IsCritical { get; set; }
  }
}
