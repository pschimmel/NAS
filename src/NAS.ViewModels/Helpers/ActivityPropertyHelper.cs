using System.Windows;
using NAS.Models.Entities;
using NAS.Resources;

namespace NAS.Models.Enums
{
  public static class ActivityPropertyHelper
  {
    private static readonly Dictionary<ActivityProperty, string> enumDescriptions = new Dictionary<ActivityProperty, string>
    {
      { ActivityProperty.None, NASResources.None },
      { ActivityProperty.Number, NASResources.Number },
      { ActivityProperty.Name, NASResources.Name },
      { ActivityProperty.EarlyStartDate, NASResources.EarlyStartDate },
      { ActivityProperty.EarlyFinishDate, NASResources.EarlyFinishDate },
      { ActivityProperty.LateStartDate, NASResources.LateStartDate },
      { ActivityProperty.LateFinishDate, NASResources.LateFinishDate },
      { ActivityProperty.ActualStartDate, NASResources.ActualStartDate },
      { ActivityProperty.ActualFinishDate, NASResources.ActualFinishDate },
      { ActivityProperty.StartDate, NASResources.StartDate },
      { ActivityProperty.FinishDate, NASResources.FinishDate },
      { ActivityProperty.OriginalDuration, NASResources.PlannedDuration},
      { ActivityProperty.FreeFloat, NASResources.FreeFloat},
      { ActivityProperty.TotalFloat, NASResources.TotalFloat},
      { ActivityProperty.PercentComplete, NASResources.PercentComplete},
      { ActivityProperty.RemainingDuration, NASResources.RemainingDuration},
      { ActivityProperty.ActualDuration, NASResources.ActualDuration},
      { ActivityProperty.RetardedDuration, NASResources.RetardedDuration},
      { ActivityProperty.WBSItem, NASResources.WBSItem},
      { ActivityProperty.Fragnet, NASResources.Fragnet},
      { ActivityProperty.TotalBudget, NASResources.Budget},
      { ActivityProperty.TotalPlannedCosts, NASResources.PlannedCosts},
      { ActivityProperty.TotalActualCosts, NASResources.ActualCosts},
      { ActivityProperty.CustomAttribute1, NASResources.Attribute1},
      { ActivityProperty.CustomAttribute2, NASResources.Attribute2},
      { ActivityProperty.CustomAttribute3, NASResources.Attribute3}
    };

    public static string GetNameOfActivityProperty(ActivityProperty item)
    {
      if (enumDescriptions.ContainsKey(item))
      {
        return enumDescriptions[item];
      }

      throw new ApplicationException(string.Format("Item {0} not found in dictionary.", item));
    }

    public static ActivityProperty GetActivityPropertyByName(string name)
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

    /// <summary>
    /// Determines whether the _property is readonly.
    /// </summary>
    /// <param _number="_property">The _property.</param>
    /// <returns>
    ///   <c>true</c> if the _property is readonly; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsPropertyReadonly(ActivityProperty property)
    {
      return property switch
      {
        ActivityProperty.ActualDuration or ActivityProperty.RetardedDuration or ActivityProperty.EarlyFinishDate or ActivityProperty.EarlyStartDate or ActivityProperty.FinishDate or ActivityProperty.FreeFloat or ActivityProperty.LateFinishDate or ActivityProperty.LateStartDate or ActivityProperty.StartDate or ActivityProperty.TotalActualCosts or ActivityProperty.TotalFloat or ActivityProperty.TotalPlannedCosts => true,
        _ => false,
      };
    }

    /// <summary>
    /// Gets the type of the _property.
    /// </summary>
    public static Type GetPropertyType(ActivityProperty property)
    {
      var t = typeof(Activity);
      var info = t.GetProperty(property.ToString());
      return info?.PropertyType;
    }

    /// <summary>
    /// Gets the text from an activity.
    /// </summary>
    /// <param _number="a">Activity</param>
    /// <param _number="_property">The _property.</param>
    /// <returns>Formated text</returns>
    public static string GetTextFromActivity(this Activity a, ActivityProperty property)
    {
      return property switch
      {
        ActivityProperty.ActualDuration => a.ActualDuration.ToString(),
        ActivityProperty.RetardedDuration => a.RetardedDuration.ToString(),
        ActivityProperty.ActualFinishDate => a.ActualFinishDate.HasValue ? a.ActualFinishDate.Value.ToString(GetFormatStringForType(GetPropertyType(property))) : null,
        ActivityProperty.ActualStartDate => a.ActualStartDate.HasValue ? a.ActualStartDate.Value.ToString(GetFormatStringForType(GetPropertyType(property))) : null,
        ActivityProperty.EarlyFinishDate => a.EarlyFinishDate.ToString(GetFormatStringForType(GetPropertyType(property))),
        ActivityProperty.EarlyStartDate => a.EarlyStartDate.ToString(GetFormatStringForType(GetPropertyType(property))),
        ActivityProperty.FinishDate => a.FinishDate.ToString(GetFormatStringForType(GetPropertyType(property))),
        ActivityProperty.Fragnet => a.Fragnet?.ToString(),
        ActivityProperty.FreeFloat => a.FreeFloat.ToString(),
        ActivityProperty.Number => a.Number,
        ActivityProperty.LateFinishDate => a.LateFinishDate.ToString(GetFormatStringForType(GetPropertyType(property))),
        ActivityProperty.LateStartDate => a.LateStartDate.ToString(GetFormatStringForType(GetPropertyType(property))),
        ActivityProperty.Name => a.Name,
        ActivityProperty.OriginalDuration => a.OriginalDuration.ToString(),
        ActivityProperty.PercentComplete => a.PercentComplete.ToString(GetFormatStringForType(GetPropertyType(property))),
        ActivityProperty.RemainingDuration => a.RemainingDuration.ToString(),
        ActivityProperty.StartDate => a.StartDate.ToString(GetFormatStringForType(GetPropertyType(property))),
        ActivityProperty.TotalBudget => a.TotalBudget.ToString(GetFormatStringForType(GetPropertyType(property))),
        ActivityProperty.TotalActualCosts => a.TotalActualCosts.ToString(GetFormatStringForType(GetPropertyType(property))),
        ActivityProperty.TotalFloat => a.TotalFloat.ToString(),
        ActivityProperty.TotalPlannedCosts => a.TotalPlannedCosts.ToString(GetFormatStringForType(GetPropertyType(property))),
        ActivityProperty.WBSItem => a.WBSItem?.ToString(),
        ActivityProperty.CustomAttribute1 => a.CustomAttribute1?.ToString(),
        ActivityProperty.CustomAttribute2 => a.CustomAttribute2?.ToString(),
        ActivityProperty.CustomAttribute3 => a.CustomAttribute3?.ToString(),
        _ => null,
      };
    }

    /// <summary>
    /// Gets the type of the format string for.
    /// </summary>
    /// <param _number="type">The type.</param>
    /// <returns></returns>
    public static string GetFormatStringForType(Type type)
    {
      return type == typeof(DateTime) || type == typeof(DateTime?)
        ? "yyyy-MM-dd"
        : type == typeof(decimal) || type == typeof(decimal?)
        ? "F"
        : type == typeof(double) || type == typeof(double?)
        ? "F"
        : null;
    }

    /// <summary>
    /// Gets the text alignment of a type in a table cell.
    /// </summary>
    public static TextAlignment GetAlignment(Type type)
    {
      return type == typeof(DateTime) || type == typeof(DateTime)
        ? TextAlignment.Center
        : type == typeof(int) || type == typeof(decimal) || type == typeof(double) || type == typeof(int?) || type == typeof(decimal?) || type == typeof(double?)
          ? TextAlignment.Right
          : TextAlignment.Left;
    }
  }
}
