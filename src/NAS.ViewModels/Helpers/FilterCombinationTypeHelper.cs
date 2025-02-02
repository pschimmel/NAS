using NAS.Models.Enums;
using NAS.Resources;

namespace NAS.ViewModels.Helpers
{
  public static class FilterCombinationTypeHelper
  {
    private static readonly Dictionary<FilterCombinationType, string> enumDescriptions = new Dictionary<FilterCombinationType, string>
    {
      { FilterCombinationType.And, NASResources.And },
      { FilterCombinationType.Or, NASResources.Or }
    };

    public static string GetNameOfFilterCombinationType(FilterCombinationType item)
    {
      if (enumDescriptions.ContainsKey(item))
      {
        return enumDescriptions[item];
      }

      throw new ApplicationException(string.Format("Item {0} not found in dictionary.", item));
    }

    public static FilterCombinationType GetFilterCombinationTypeByName(string name)
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
