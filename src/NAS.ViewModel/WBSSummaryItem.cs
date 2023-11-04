using System;
using System.Collections.Generic;
using System.Linq;
using NAS.Model.Entities;

namespace NAS.ViewModel
{
  public class WBSSummaryItem
  {
    private readonly List<Activity> activities;

    public WBSSummaryItem(List<Activity> a, WBSItem i)
    {
      Item = i;
      activities = a.FindAll(x => (x.Fragnet == null || x.Fragnet.IsVisible) && Item.ContainsItem(x));
    }

    public WBSItem Item { get; }

    public DateTime Start => activities == null || activities.Count == 0 ? DateTime.MinValue : activities.Min(x => x.StartDate);

    public DateTime Finish => activities == null || activities.Count == 0 ? DateTime.MinValue : activities.Max(x => x.FinishDate);

    public decimal Budget => activities == null || activities.Count == 0 ? 0 : activities.Sum(x => x.TotalBudget);

    public decimal PlannedCosts => activities == null || activities.Count == 0 ? 0 : activities.Sum(x => x.TotalPlannedCosts);

    public decimal ActualCosts => activities == null || activities.Count == 0 ? 0 : activities.Sum(x => x.TotalActualCosts);
  }
}
