using System;
using System.Collections.Generic;

namespace NAS.Model.Enums
{
  public static class FilterRelationHelper
  {
    private static readonly Dictionary<FilterRelation, string> enumDescriptions = new Dictionary<FilterRelation, string>
    {
     { FilterRelation.EqualTo, "=" },
     { FilterRelation.GreaterThan, ">" },
     { FilterRelation.GreaterThanOrEqualTo, ">=" },
     { FilterRelation.LessThan, "<" },
     { FilterRelation.LessThanOrEqualTo, "<=" },
     { FilterRelation.NotEqualTo, "<>" }
    };

    public static string GetNameOfFilterRelation(FilterRelation item)
    {
      if (enumDescriptions.ContainsKey(item))
      {
        return enumDescriptions[item];
      }

      throw new ApplicationException(string.Format("Item {0} not found in dictionary.", item));
    }

    public static FilterRelation GetFilterRelationByName(string name)
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
