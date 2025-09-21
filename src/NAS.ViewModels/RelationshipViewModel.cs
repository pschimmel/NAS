using NAS.Models.Entities;
using NAS.Models.Enums;
using NAS.ViewModels.Base;
using NAS.ViewModels.Helpers;

namespace NAS.ViewModels
{
  public class RelationshipViewModel : ViewModelBase
  {
    #region Constructor

    public RelationshipViewModel(Relationship relationship, ActivityViewModel activity1, ActivityViewModel activity2)
    {
      Relationship = relationship ?? throw new ArgumentNullException(nameof(relationship));
      Activity1 = activity1 ?? throw new ArgumentNullException(nameof(activity1));
      Activity2 = activity2 ?? throw new ArgumentNullException(nameof(activity2));
      Lag = relationship.Lag;
      SelectedRelationshipType = relationship.RelationshipType;
      if (activity1.Activity != Relationship.Activity1)
        throw new ArgumentException("Activity1 does not match the relationship's Activity1.", nameof(activity1));
      if (activity2.Activity != Relationship.Activity2)
        throw new ArgumentException("Activity2 does not match the relationship's Activity2.", nameof(activity2));
    }

    #endregion

    #region Properties

    public override HelpTopic HelpTopicKey => HelpTopic.Relationship;

    public Relationship Relationship { get; }

    public ActivityViewModel Activity1 { get; }

    public ActivityViewModel Activity2 { get; }

    public RelationshipType SelectedRelationshipType { get; }

    public int Lag { get; }

    public string DisplayName => $"{Relationship.Activity1.Number} - {Relationship.Activity2.Number}";

    #endregion
  }
}
