using System.Windows.Data;
using NAS.Models.Entities;
using NAS.Models.Enums;
using NAS.Views.Converters;

namespace NAS.Views.Helpers
{
  public static class ActivityExtensions
  {
    #region Activities

    public static BindingBase GetBindingFromActivity(this Activity a, ActivityProperty property)
    {
      Binding b = null;
      switch (property)
      {
        case ActivityProperty.ActualDuration:
        case ActivityProperty.RetardedDuration:
        case ActivityProperty.FreeFloat:
        case ActivityProperty.Number:
        case ActivityProperty.Name:
        case ActivityProperty.OriginalDuration:
        case ActivityProperty.PercentComplete:
        case ActivityProperty.RemainingDuration:
        case ActivityProperty.TotalFloat:
          b = new Binding(property.ToString());
          break;
        case ActivityProperty.ActualFinishDate:
        case ActivityProperty.ActualStartDate:
        case ActivityProperty.EarlyFinishDate:
        case ActivityProperty.EarlyStartDate:
        case ActivityProperty.FinishDate:
        case ActivityProperty.LateFinishDate:
        case ActivityProperty.LateStartDate:
        case ActivityProperty.StartDate:
          b = new Binding(property.ToString()) { Converter = new DateConverter() };
          break;
        case ActivityProperty.Fragnet:
        case ActivityProperty.WBSItem:
        case ActivityProperty.CustomAttribute1:
        case ActivityProperty.CustomAttribute2:
        case ActivityProperty.CustomAttribute3:
          b = new Binding(property.ToString() + ".Name");
          break;
        case ActivityProperty.TotalActualCosts:
        case ActivityProperty.TotalBudget:
        case ActivityProperty.TotalPlannedCosts:
          b = new Binding(property.ToString()) { StringFormat = "{0:n}" };
          break;
      }
      if (b != null)
      {
        b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
        b.Mode = BindingMode.OneWay;
        b.Source = a;
      }
      return b;
    }

    #endregion
  }
}
