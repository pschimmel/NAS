﻿using NAS.Models.Entities;
using NAS.Models.Enums;
using NAS.ViewModels.Base;
using NAS.ViewModels.Helpers;

namespace NAS.ViewModels
{
  public class RelationshipViewModel : ViewModelBase
  {
    #region Constructor

    public RelationshipViewModel(Relationship relationship)
    {
      Relationship = relationship ?? throw new ArgumentNullException(nameof(relationship));
    }

    #endregion

    #region Properties

    public override HelpTopic HelpTopicKey => HelpTopic.Relationship;

    public Relationship Relationship { get; set; }

    public Activity Activity1 => Relationship.Activity1;

    public Activity Activity2 => Relationship.Activity2;

    public RelationshipType SelectedRelationshipType => Relationship.RelationshipType;

    public int Lag => Relationship.Lag;

    public string DisplayName => $"{Relationship.Activity1.Number} - {Relationship.Activity2.Number}";

    #endregion
  }
}
