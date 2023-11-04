using System;
using System.Collections.Generic;
using NAS.Resources;

namespace NAS.Model.Enums
{
  public static class SortDirectionHelper
  {
    private static readonly Dictionary<SortDirection, string> enumDescriptions = new Dictionary<SortDirection, string>
    {
      { SortDirection.Ascending, NASResources.Ascending },
      { SortDirection.Descending, NASResources.Descending }
    };

    public static string GetNameOfSortDirection(SortDirection item)
    {
      if (enumDescriptions.ContainsKey(item))
      {
        return enumDescriptions[item];
      }

      throw new ApplicationException(string.Format("Item {0} not found in dictionary.", item));
    }

    public static SortDirection GetSortDirectionByName(string name)
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
