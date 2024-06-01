using NAS.Models.Entities;
using NAS.Models.Enums;
using NAS.Resources;
using NAS.ViewModels.Base;
using NAS.ViewModels.Helpers;

namespace NAS.ViewModels
{
  public class FilterDefinitionViewModel : ValidatingViewModel
  {
    #region Fields

    private readonly Schedule _schedule;
    private readonly FilterDefinition _definition;
    private ActivityProperty _property;
    private List<object> items;

    #endregion

    #region Constructor

    public FilterDefinitionViewModel(FilterDefinition definition)
    {
      _schedule = definition.Layout.Schedule;
      _definition = definition;
      Property = definition.Property;
      Relation = definition.Relation;
      ObjectString = definition.ObjectString;
    }

    #endregion

    #region Public Properties

    public List<ActivityProperty> ActivityProperties { get; } = Enum.GetValues(typeof(ActivityProperty)).Cast<ActivityProperty>().Where(x => x != ActivityProperty.None).ToList();

    public List<FilterRelation> FilterRelations { get; } = Enum.GetValues(typeof(FilterRelation)).Cast<FilterRelation>().ToList();

    public FilterRelation Relation { get; set; }

    public string ObjectString { get; set; }

    public ActivityProperty Property
    {
      get => _property;
      set
      {
        if (_property != value)
        {
          _property = value;
          ObjectString = null;
          items = null;
          OnPropertyChanged(nameof(Property));
          OnPropertyChanged(nameof(SelectionVisible));
          OnPropertyChanged(nameof(DateVisible));
          OnPropertyChanged(nameof(ValueVisible));
          OnPropertyChanged(nameof(Selection));
          OnPropertyChanged(nameof(Items));
        }
      }
    }

    public string Selection
    {
      get => Property switch
      {
        ActivityProperty.Fragnet or ActivityProperty.WBSItem => ActivityPropertyHelper.GetNameOfActivityProperty(Property),
        ActivityProperty.CustomAttribute1 => _schedule.CustomAttribute1Header,
        ActivityProperty.CustomAttribute2 => _schedule.CustomAttribute2Header,
        ActivityProperty.CustomAttribute3 => _schedule.CustomAttribute3Header,
        _ => null,
      };
    }

    public List<object> Items
    {
      get
      {
        if (items == null)
        {
          items = [];
          switch (Property)
          {
            case ActivityProperty.Fragnet:
              foreach (var item in _schedule.Fragnets)
              {
                items.Add(item);
              }

              break;
            case ActivityProperty.WBSItem:
              AddWBSItem(_schedule.WBSItem);
              break;
            case ActivityProperty.CustomAttribute1:
              foreach (var item in _schedule.CustomAttributes1)
              {
                items.Add(item);
              }

              break;
            case ActivityProperty.CustomAttribute2:
              foreach (var item in _schedule.CustomAttributes2)
              {
                items.Add(item);
              }

              break;
            case ActivityProperty.CustomAttribute3:
              foreach (var item in _schedule.CustomAttributes3)
              {
                items.Add(item);
              }

              break;
          }
          OnPropertyChanged(nameof(CurrentItem));
        }
        return items;
      }
    }

    public object CurrentItem
    {
      get
      {
        if (Guid.TryParse(ObjectString, out Guid id))
        {
          switch (Property)
          {
            case ActivityProperty.Fragnet:
              return _schedule.Fragnets.FirstOrDefault(x => x.ID == id);
            case ActivityProperty.WBSItem:
              return _schedule.FindWBSItem(id);
            case ActivityProperty.CustomAttribute1:
              return _schedule.CustomAttributes1.FirstOrDefault(x => x.ID == id);
            case ActivityProperty.CustomAttribute2:
              return _schedule.CustomAttributes2.FirstOrDefault(x => x.ID == id);
            case ActivityProperty.CustomAttribute3:
              return _schedule.CustomAttributes3.FirstOrDefault(x => x.ID == id);
          }
        }

        return null;
      }
      set
      {
        switch (Property)
        {
          case ActivityProperty.Fragnet:
            ObjectString = (value as Fragnet).ID.ToString();
            break;
          case ActivityProperty.WBSItem:
            ObjectString = (value as WBSItem).ID.ToString();
            break;
          case ActivityProperty.CustomAttribute1:
          case ActivityProperty.CustomAttribute2:
          case ActivityProperty.CustomAttribute3:
            ObjectString = (value as CustomAttribute).ID.ToString();
            break;
        }
        OnPropertyChanged(nameof(CurrentItem));
      }
    }

    private void AddWBSItem(WBSItem parent)
    {
      items.Add(parent);
      foreach (var child in parent.Children)
      {
        AddWBSItem(child);
      }
    }

    public bool SelectionVisible
    {
      get
      {
        var propertyType = ActivityPropertyHelper.GetPropertyType(Property);
        return Property == ActivityProperty.Fragnet || propertyType == typeof(WBSItem) || propertyType == typeof(CustomAttribute);
      }
    }

    public bool DateVisible
    {
      get
      {
        var propertyType = ActivityPropertyHelper.GetPropertyType(Property);
        return propertyType == typeof(DateTime) || propertyType == typeof(DateTime?);
      }
    }

    public bool ValueVisible => !SelectionVisible && !DateVisible;

    #endregion

    #region Validation


    protected override ValidationResult OnValidating()
    {
      var result = ValidationResult.OK();

      var relation = Relation;

      switch (Property)
      {
        case ActivityProperty.ActualDuration:
        case ActivityProperty.RetardedDuration:
        case ActivityProperty.OriginalDuration:
        case ActivityProperty.RemainingDuration:
          if (!int.TryParse(ObjectString, out int i) || i < 0)
          {
            result = ValidationResult.Error(NASResources.ValueNotValid);
          }
          break;
        case ActivityProperty.ActualFinishDate:
        case ActivityProperty.ActualStartDate:
        case ActivityProperty.EarlyFinishDate:
        case ActivityProperty.EarlyStartDate:
        case ActivityProperty.FinishDate:
        case ActivityProperty.LateFinishDate:
        case ActivityProperty.LateStartDate:
        case ActivityProperty.StartDate:
          if (!DateTime.TryParse(ObjectString, out _))
          {
            result = ValidationResult.Error(NASResources.ValueNotValid);
          }
          break;
        case ActivityProperty.Fragnet:
          if (!Guid.TryParse(ObjectString, out Guid fragnetID) || !_schedule.Fragnets.Any(x => x.ID == fragnetID))
          {
            result = ValidationResult.Error(NASResources.ValueNotValid);
          }
          break;
        case ActivityProperty.FreeFloat:
        case ActivityProperty.TotalFloat:
          if (!int.TryParse(ObjectString, out _))
          {
            result = ValidationResult.Error(NASResources.ValueNotValid);
          }
          break;
        case ActivityProperty.WBSItem:
          if (!Guid.TryParse(ObjectString, out Guid wbsID) || GetWBSItem(_schedule.WBSItem, wbsID) == null)
          {
            result = ValidationResult.Error(NASResources.ValueNotValid);
          }
          break;
        case ActivityProperty.PercentComplete:
          if (!double.TryParse(ObjectString, out double doubleValue) || doubleValue < 0d || doubleValue > 100d)
          {
            result = ValidationResult.Error(NASResources.ValueNotValid);
          }
          break;
        case ActivityProperty.TotalBudget:
        case ActivityProperty.TotalActualCosts:
        case ActivityProperty.TotalPlannedCosts:
          if (!decimal.TryParse(ObjectString, out decimal decimalValue) || decimalValue < 0m)
          {
            result = ValidationResult.Error(NASResources.ValueNotValid);
          }
          break;
        case ActivityProperty.CustomAttribute1:
          if (!Guid.TryParse(ObjectString, out Guid customAttribute1ID) || !_schedule.CustomAttributes1.Any(x => x.ID == customAttribute1ID))
          {
            result = ValidationResult.Error(NASResources.ValueNotValid);
          }
          break;
        case ActivityProperty.CustomAttribute2:
          if (!Guid.TryParse(ObjectString, out Guid customAttribute2ID) || !_schedule.CustomAttributes2.Any(x => x.ID == customAttribute2ID))
          {
            result = ValidationResult.Error(NASResources.ValueNotValid);
          }
          break;
        case ActivityProperty.CustomAttribute3:
          if (!Guid.TryParse(ObjectString, out Guid customAttribute3ID) || !_schedule.CustomAttributes3.Any(x => x.ID == customAttribute3ID))
          {
            result = ValidationResult.Error(NASResources.ValueNotValid);
          }
          break;
        case ActivityProperty.None:
          result = ValidationResult.Error(NASResources.PleaseSelectProperty);
          break;
      }

      return result;
    }

    #endregion

    #region Apply

    public void Apply()
    {
      if (Validate().IsOK)
      {
        _definition.Property = Property;
        _definition.Relation = Relation;
        _definition.ObjectString = ObjectString;
      }
    }

    #endregion

    #region Private Members

    private static WBSItem GetWBSItem(WBSItem parent, Guid id)
    {
      if (parent.ID == id)
      {
        return parent;
      }

      foreach (var child in parent.Children)
      {
        var item = GetWBSItem(child, id);
        if (item != null)
        {
          return item;
        }
      }
      return null;
    }

    #endregion
  }
}
