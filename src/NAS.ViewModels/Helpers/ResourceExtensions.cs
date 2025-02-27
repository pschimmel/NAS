using NAS.Models.Entities;
using NAS.Models.Enums;

namespace NAS.ViewModels.Helpers
{
  public static class ResourceExtensions
  {
    public static double GetResourceAllocation(this Resource resource, Schedule schedule, DateTime day)
    {
      double result = 0;

      foreach (var a in schedule.Activities.Where(x => x.Fragnet == null || x.Fragnet.IsVisible))
      {
        if (a.ActivityType == ActivityType.Activity)
        {
          if (day >= a.StartDate && day <= a.FinishDate && (resource is CalendarResource || a.Calendar.IsWorkDay(day)))
          {
            var assignment = schedule.ResourceAssignments.Where(x => x.Activity == a && x.Resource == resource);
            if (assignment.Any())
            {
              result += assignment.Sum(x => x.UnitsPerDay);
            }
          }
        }
      }
      return result;
    }

    public static decimal GetResourceBudget(this Resource resource, Schedule schedule, DateTime day)
    {
      decimal result = 0;

      foreach (var a in schedule.Activities.Where(x => x.Fragnet == null || x.Fragnet.IsVisible))
      {
        if (a.ActivityType == ActivityType.Activity)
        {
          if (day >= a.StartDate && day <= a.FinishDate && (resource is CalendarResource || a.Calendar.IsWorkDay(day)))
          {
            var resourceAssignments = schedule.ResourceAssignments.Where(x => x.Activity == a && x.Resource == resource);
            foreach (var association in resourceAssignments)
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

    public static decimal GetActualResourceCosts(this Resource resource, Schedule schedule, DateTime day)
    {
      decimal result = 0;

      foreach (var a in schedule.Activities.Where(x => x.Fragnet == null || x.Fragnet.IsVisible))
      {
        if (a.ActivityType == ActivityType.Activity)
        {
          if (day >= a.StartDate && day <= a.FinishDate && (resource is CalendarResource || a.Calendar.IsWorkDay(day)))
          {
            var resourceAssignments = schedule.ResourceAssignments.Where(x => x.Activity == a && x.Resource == resource);

            foreach (var association in resourceAssignments)
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

    public static decimal GetPlannedResourceCosts(this Resource resource, Schedule schedule, DateTime day)
    {
      decimal result = 0;

      foreach (var a in schedule.Activities.Where(x => x.Fragnet == null || x.Fragnet.IsVisible))
      {
        if (a.ActivityType == ActivityType.Activity)
        {
          if (day >= a.EarlyStartDate && day <= a.EarlyFinishDate && (resource is CalendarResource || a.Calendar.IsWorkDay(day)))
          {
            var resourceAssignments = schedule.ResourceAssignments.Where(x => x.Activity == a && x.Resource == resource);

            foreach (var association in resourceAssignments)
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
