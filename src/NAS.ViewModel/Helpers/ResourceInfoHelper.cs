using System;
using System.Collections.Generic;
using System.Linq;
using NAS.Model.Entities;
using NAS.Model.Enums;

namespace NAS.ViewModel.Helpers
{
  public class ResourceInfoHelper
  {
    #region Constructor

    public ResourceInfoHelper(Resource resource, DateTime start, DateTime end, TimeAggregateType aggregate)
    {
      start = start.Date;
      end = end.Date;

      ResourceAllocation = new Dictionary<DateTime, double>();
      ResourceBudget = new Dictionary<DateTime, decimal>();
      ResourceCostsActual = new Dictionary<DateTime, decimal>();
      ResourceCostsPlanned = new Dictionary<DateTime, decimal>();

      double allocation = 0;
      decimal budget = 0;
      decimal actualCost = 0;
      decimal plannedCost = 0;

      var currentDay = start;

      for (var day = start; day <= end; day = day.AddDays(1))
      {
        allocation += resource.GetResourceAllocation(day);
        budget += resource.GetResourceBudget(day);
        actualCost += resource.GetActualResourceCosts(day);
        plannedCost += resource.GetPlannedResourceCosts(day);

        if (aggregate == TimeAggregateType.Day ||
          aggregate == TimeAggregateType.Week && day.DayOfWeek == DayOfWeek.Saturday ||
          aggregate == TimeAggregateType.Month && day.AddDays(-1).Day == 1 ||
          aggregate == TimeAggregateType.Year && day.AddDays(-1).Day == 1 && day.Month == 1 ||
          day == end)
        {
          ResourceAllocation.Add(currentDay, allocation);
          ResourceBudget.Add(currentDay, budget);
          ResourceCostsActual.Add(currentDay, plannedCost);
          ResourceCostsPlanned.Add(currentDay, actualCost);
          allocation = 0;
          budget = 0;
          actualCost = 0;
          plannedCost = 0;
          currentDay = day;

          if (aggregate == TimeAggregateType.Day)
          {
            currentDay = currentDay.AddDays(1);
          }
        }
      }

      RefreshMaxValues();
    }

    private void RefreshMaxValues()
    {
      ResourceAllocationMax = ResourceAllocation.Max(x => x.Value);
      ResourceBudgetMax = ResourceBudget.Max(x => x.Value);
      ResourceCostsActualMax = ResourceCostsActual.Max(x => x.Value);
      ResourceCostsPlannedMax = ResourceCostsPlanned.Max(x => x.Value);
    }

    #endregion

    #region Public Properties

    public Dictionary<DateTime, double> ResourceAllocation { get; private set; }

    public Dictionary<DateTime, decimal> ResourceBudget { get; private set; }

    public Dictionary<DateTime, decimal> ResourceCostsActual { get; private set; }

    public Dictionary<DateTime, decimal> ResourceCostsPlanned { get; private set; }

    public double ResourceAllocationMax { get; private set; }

    public decimal ResourceBudgetMax { get; private set; }

    public decimal ResourceCostsActualMax { get; private set; }

    public decimal ResourceCostsPlannedMax { get; private set; }

    #endregion
  }
}
