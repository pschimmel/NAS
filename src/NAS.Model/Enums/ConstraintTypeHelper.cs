using System;
using System.Collections.Generic;
using NAS.Resources;

namespace NAS.Model.Enums
{
  public static class ConstraintTypeHelper
  {
    private static readonly Dictionary<ConstraintType, string> enumDescriptions = new Dictionary<ConstraintType, string>
    {
      { ConstraintType.None, NASResources.NoConstraint },
      { ConstraintType.EndOn, NASResources.EndOn },
      { ConstraintType.EndOnOrEarlier, NASResources.EndOnOrEarlier },
      { ConstraintType.StartOn, NASResources.StartOn },
      { ConstraintType.StartOnOrLater, NASResources.StartOnOrLater }
    };

    public static string GetNameOfConstraintType(ConstraintType item)
    {
      if (enumDescriptions.ContainsKey(item))
      {
        return enumDescriptions[item];
      }

      throw new ApplicationException(string.Format("Item {0} not found in dictionary.", item));
    }

    public static ConstraintType GetConstraintTypeByName(string name)
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
