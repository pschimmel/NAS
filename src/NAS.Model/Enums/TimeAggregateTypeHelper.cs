using System;
using System.Collections.Generic;
using NAS.Resources;

namespace NAS.Model.Enums
{
  public static class TimeAggregateTypeHelper
  {
    private static readonly Dictionary<TimeAggregateType, string> enumDescriptions = new Dictionary<TimeAggregateType, string>
    {
      { TimeAggregateType.Day, NASResources.Day },
      { TimeAggregateType.Week, NASResources.Week },
      { TimeAggregateType.Month, NASResources.Month },
      { TimeAggregateType.Year, NASResources.Year }
    };

    public static string GetNameOfConstraintType(TimeAggregateType item)
    {
      if (enumDescriptions.ContainsKey(item))
      {
        return enumDescriptions[item];
      }

      throw new ApplicationException(string.Format("Item {0} not found in dictionary.", item));
    }

    public static TimeAggregateType GetConstraintTypeByName(string name)
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
