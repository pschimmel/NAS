using NAS.Models.Entities;
using NAS.Models.Enums;

namespace NAS.ViewModels
{
  public class AddRelationshipInfo
  {
    public Activity Activity1 { get; set; }
    public Activity Activity2 { get; set; }
    public RelationshipType RelationshipType { get; set; }

    public int Lag { get; set; }
  }
}
