using NAS.Model.Entities;
using NAS.Model.Enums;
using NAS.ViewModel.Base;

namespace NAS.ViewModel
{
  public class LogicRelatedViewModel : ViewModelBase
  {
    public LogicRelatedViewModel(Activity activity, Relationship relationship)
    {
      Activity = activity;
      Relationship = relationship;
    }

    public Activity Activity { get; }

    public Relationship Relationship { get; }

    public string ActivityName => Activity.Name;

    public RelationshipType RelationshipType
    {
      get => Relationship.RelationshipType;
      set => Relationship.RelationshipType = value;
    }

    public int Lag
    {
      get => Relationship.Lag;
      set => Relationship.Lag = value;
    }

    public bool IsDriving => Relationship.IsDriving;

    public bool IsCritical => Relationship.IsCritical;
  }
}
