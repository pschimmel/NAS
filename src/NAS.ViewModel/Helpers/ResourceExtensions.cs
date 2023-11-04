using System;
using System.Linq;
using NAS.Model.Entities;
using NAS.Model.Enums;

namespace NAS.ViewModel.Helpers
{
  public static class ResourceExtensions
  {
    public static double GetResourceAllocation(this Resource resource, DateTime day)
    {
      var schedule = resource.Schedule;
      double result = 0;
      foreach (var a in schedule.Activities.Where(x => x.Fragnet == null || x.Fragnet.IsVisible))
      {
        if (a.ActivityType == ActivityType.Activity)
        {
          if (day >= a.StartDate && day <= a.FinishDate && (resource is CalendarResource || a.Calendar.IsWorkDay(day)))
          {
            result += a.ResourceAssociations.Where(x => x.Resource == resource).Sum(x => x.UnitsPerDay);
          }
        }
      }
      return result;
    }

    public static decimal GetResourceBudget(this Resource resource, DateTime day)
    {
      var schedule = resource.Schedule;
      decimal result = 0;
      foreach (var a in schedule.Activities.Where(x => x.Fragnet == null || x.Fragnet.IsVisible))
      {
        if (a.ActivityType == ActivityType.Activity)
        {
          if (day >= a.StartDate && day <= a.FinishDate && (resource is CalendarResource || a.Calendar.IsWorkDay(day)))
          {
            foreach (var association in a.ResourceAssociations.Where(x => x.Resource == resource))
            {
              if (resource is CalendarResource && Math.Round((a.FinishDate - a.StartDate).TotalDays) > 0)
              {
                result += association.Budget / Convert.ToDecimal(Math.Round((a.FinishDate - a.StartDate).TotalDays));
              }
              else if (resource is MaterialResource || resource is WorkResource && a.Calendar.GetWorkDays(a.StartDate, a.FinishDate, true) > 0)
              {
                result += association.Budget / a.Calendar.GetWorkDays(a.StartDate, a.FinishDate, true);
              }
            }
          }
        }
      }
      return result;
    }

    public static decimal GetActualResourceCosts(this Resource resource, DateTime day)
    {
      var schedule = resource.Schedule;
      decimal result = 0;
      foreach (var a in schedule.Activities.Where(x => x.Fragnet == null || x.Fragnet.IsVisible))
      {
        if (a.ActivityType == ActivityType.Activity)
        {
          if (day >= a.StartDate && day <= a.FinishDate && (resource is CalendarResource || a.Calendar.IsWorkDay(day)))
          {
            foreach (var association in a.ResourceAssociations.Where(x => x.Resource == resource))
            {
              result += association.Resource.CostsPerUnit * Convert.ToDecimal(association.UnitsPerDay);
              if (resource is CalendarResource && a.IsFinished && Math.Round((a.FinishDate - a.StartDate).TotalDays) > 0)
              {
                result += association.FixedCosts / Convert.ToDecimal(Math.Round((a.FinishDate - a.StartDate).TotalDays));
              }
              else if (resource is MaterialResource || resource is WorkResource && a.Calendar.GetWorkDays(a.StartDate, a.FinishDate, true) > 0)
              {
                result += association.FixedCosts / a.Calendar.GetWorkDays(a.StartDate, a.FinishDate, true);
              }
            }
          }
        }
      }
      return result;
    }

    public static decimal GetPlannedResourceCosts(this Resource resource, DateTime day)
    {
      var schedule = resource.Schedule;
      decimal result = 0;
      foreach (var a in schedule.Activities.Where(x => x.Fragnet == null || x.Fragnet.IsVisible))
      {
        if (a.ActivityType == ActivityType.Activity)
        {
          if (day >= a.EarlyStartDate && day <= a.EarlyFinishDate && (resource is CalendarResource || a.Calendar.IsWorkDay(day)))
          {
            foreach (var association in a.ResourceAssociations.Where(x => x.Resource == resource))
            {
              result += association.Resource.CostsPerUnit * Convert.ToDecimal(association.UnitsPerDay);
              if (resource is CalendarResource && Math.Round((a.EarlyFinishDate - a.LateFinishDate).TotalDays) > 0)
              {
                result += association.FixedCosts / Convert.ToDecimal(Math.Round((a.EarlyFinishDate - a.LateFinishDate).TotalDays));
              }
              else if (resource is MaterialResource || resource is WorkResource && a.OriginalDuration > 0)
              {
                result += association.FixedCosts / a.OriginalDuration;
              }
            }
          }
        }
      }
      return result;
    }
  }
}
