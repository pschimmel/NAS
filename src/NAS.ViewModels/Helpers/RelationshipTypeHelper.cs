using NAS.Models.Enums;
using NAS.Resources;

namespace NAS.ViewModels.Helpers
{
  public static class RelationshipTypeHelper
  {
    private static readonly Dictionary<RelationshipType, string> enumDescriptions = new Dictionary<RelationshipType, string>
    {
      { RelationshipType.FinishFinish, NASResources.FinishFinish },
      { RelationshipType.FinishStart, NASResources.FinishStart },
      { RelationshipType.StartStart, NASResources.StartStart }
    };

    public static string GetNameOfRelationshipType(RelationshipType item)
    {
      if (enumDescriptions.ContainsKey(item))
      {
        return enumDescriptions[item];
      }

      throw new ApplicationException(string.Format("Item {0} not found in dictionary.", item));
    }

    public static RelationshipType GetRelationshipTypeByName(string name)
    {
      foreach (var kvp in enumDescriptions)
      {
        if (kvp.Value == name)
        {
          return kvp.Key;
        }
      }
      throw new ApplicationException(string.Format("Item {0} not found in dictionary.", name));
    }
  }
}
