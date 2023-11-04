using NAS.Model.Entities;
using NAS.Model.Enums;

namespace NAS.ViewModel
{
  public class AddRelationshipInfo
  {
    public Activity Activity1 { get; set; }
    public Activity Activity2 { get; set; }
    public RelationshipType RelationshipType { get; set; }

    public int Lag { get; set; }
  }
}
