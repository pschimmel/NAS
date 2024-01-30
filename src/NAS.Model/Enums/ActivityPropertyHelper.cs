using System;
using System.Collections.Generic;
using System.Windows;
using NAS.Model.Entities;
using NAS.Resources;

namespace NAS.Model.Enums
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
      switch (property)
      {
        case ActivityProperty.ActualDuration:
        case ActivityProperty.RetardedDuration:
        case ActivityProperty.EarlyFinishDate:
        case ActivityProperty.EarlyStartDate:
        case ActivityProperty.FinishDate:
        case ActivityProperty.FreeFloat:
        case ActivityProperty.LateFinishDate:
        case ActivityProperty.LateStartDate:
        case ActivityProperty.StartDate:
        case ActivityProperty.TotalActualCosts:
        case ActivityProperty.TotalFloat:
        case ActivityProperty.TotalPlannedCosts:
          return true;
        default:
          return false;
      }
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
      switch (property)
      {
        case ActivityProperty.ActualDuration:
          return a.ActualDuration.ToString();
        case ActivityProperty.RetardedDuration:
          return a.RetardedDuration.ToString();
        case ActivityProperty.ActualFinishDate:
          return a.ActualFinishDate.HasValue ? a.ActualFinishDate.Value.ToString(GetFormatStringForType(GetPropertyType(property))) : null;

        case ActivityProperty.ActualStartDate:
          return a.ActualStartDate.HasValue ? a.ActualStartDate.Value.ToString(GetFormatStringForType(GetPropertyType(property))) : null;

        case ActivityProperty.EarlyFinishDate:
          return a.EarlyFinishDate.ToString(GetFormatStringForType(GetPropertyType(property)));
        case ActivityProperty.EarlyStartDate:
          return a.EarlyStartDate.ToString(GetFormatStringForType(GetPropertyType(property)));
        case ActivityProperty.FinishDate:
          return a.FinishDate.ToString(GetFormatStringForType(GetPropertyType(property)));
        case ActivityProperty.Fragnet:
          return a.Fragnet?.ToString();

        case ActivityProperty.FreeFloat:
          return a.FreeFloat.ToString();
        case ActivityProperty.Number:
          return a.Number;
        case ActivityProperty.LateFinishDate:
          return a.LateFinishDate.ToString(GetFormatStringForType(GetPropertyType(property)));
        case ActivityProperty.LateStartDate:
          return a.LateStartDate.ToString(GetFormatStringForType(GetPropertyType(property)));
        case ActivityProperty.Name:
          return a.Name;
        case ActivityProperty.OriginalDuration:
          return a.OriginalDuration.ToString();
        case ActivityProperty.PercentComplete:
          return a.PercentComplete.ToString(GetFormatStringForType(GetPropertyType(property)));
        case ActivityProperty.RemainingDuration:
          return a.RemainingDuration.ToString();
        case ActivityProperty.StartDate:
          return a.StartDate.ToString(GetFormatStringForType(GetPropertyType(property)));
        case ActivityProperty.TotalBudget:
          return a.TotalBudget.ToString(GetFormatStringForType(GetPropertyType(property)));
        case ActivityProperty.TotalActualCosts:
          return a.TotalActualCosts.ToString(GetFormatStringForType(GetPropertyType(property)));
        case ActivityProperty.TotalFloat:
          return a.TotalFloat.ToString();
        case ActivityProperty.TotalPlannedCosts:
          return a.TotalPlannedCosts.ToString(GetFormatStringForType(GetPropertyType(property)));
        case ActivityProperty.WBSItem:
          return a.WBSItem?.ToString();

        case ActivityProperty.CustomAttribute1:
          return a.CustomAttribute1?.ToString();

        case ActivityProperty.CustomAttribute2:
          return a.CustomAttribute2?.ToString();

        case ActivityProperty.CustomAttribute3:
          return a.CustomAttribute3?.ToString();

        default:
          return null;
      }
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
