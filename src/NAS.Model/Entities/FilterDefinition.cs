using NAS.Model.Enums;

namespace NAS.Model.Entities
{
  public class FilterDefinition : NASObject
  {
    private ActivityProperty _property;
    private FilterRelation _relation;
    private string _objectString;

    public FilterDefinition()
    { }

    public FilterDefinition(FilterDefinition other)
    {
      _property = other.Property;
      _relation = other.Relation;
      _objectString = other.ObjectString;
    }

    public ActivityProperty Property
    {
      get => _property;
      set
      {
        if (_property != value)
        {
          _property = value;
          OnPropertyChanged(nameof(Property));
          OnPropertyChanged(nameof(Name));
        }
      }
    }

    public FilterRelation Relation
    {
      get => _relation;
      set
      {
        if (_relation != value)
        {
          _relation = value;
          OnPropertyChanged(nameof(Relation));
          OnPropertyChanged(nameof(Name));
        }
      }
    }

    public string ObjectString
    {
      get => _objectString;
      set
      {
        if (_objectString != value)
        {
          _objectString = value;
          OnPropertyChanged(nameof(ObjectString));
          OnPropertyChanged(nameof(Name));
        }
      }
    }

    public string Name
    {
      get
      {
        string s = ObjectString;
        if (Layout != null && Layout.Schedule != null)
        {
          var schedule = Layout.Schedule;
          if (Guid.TryParse(ObjectString, out var guid))
          {
            string id = guid.ToString();
            switch (Property)
            {
              case ActivityProperty.Fragnet:
                if (schedule.Fragnets.Any(x => x.ID == id.ToString()))
                {
                  s = schedule.Fragnets.First(x => x.ID == id).Name;
                }

                break;
              case ActivityProperty.WBSItem:
                var item = FindWBSItem(id);
                if (item != null)
                {
                  s = item.FullName;
                }

                break;
              case ActivityProperty.CustomAttribute1:
                if (schedule.CustomAttributes1.Any(x => x.ID == id))
                {
                  s = schedule.CustomAttributes1.First(x => x.ID == id).Name;
                }

                break;
              case ActivityProperty.CustomAttribute2:
                if (schedule.CustomAttributes2.Any(x => x.ID == id))
                {
                  s = schedule.CustomAttributes2.First(x => x.ID == id).Name;
                }

                break;
              case ActivityProperty.CustomAttribute3:
                if (schedule.CustomAttributes3.Any(x => x.ID == id))
                {
                  s = schedule.CustomAttributes3.First(x => x.ID == id).Name;
                }

                break;
            }
          }
        }
        return ActivityPropertyHelper.GetNameOfActivityProperty(Property) + " " + FilterRelationHelper.GetNameOfFilterRelation(Relation) + " " + s;
      }
    }

    public virtual Layout Layout { get; set; }

    public bool Compare(Activity activity)
    {
      switch (Property)
      {
        case ActivityProperty.Number:
          return CompareStrings(activity.Number, ObjectString, Relation);
        case ActivityProperty.Name:
          return CompareStrings(activity.Name, ObjectString, Relation);
        case ActivityProperty.Fragnet:
          if (activity.Fragnet == null)
          {
            return false;
          }

          return CompareGuids(activity.Fragnet.ID, ObjectString, Relation);
        case ActivityProperty.WBSItem:
          if (activity.WBSItem == null)
          {
            return false;
          }

          return CompareGuids(activity.WBSItem.ID, ObjectString, Relation);
        case ActivityProperty.CustomAttribute1:
          if (activity.CustomAttribute1 == null)
          {
            return false;
          }

          return CompareGuids(activity.CustomAttribute1.ID, ObjectString, Relation);
        case ActivityProperty.CustomAttribute2:
          if (activity.CustomAttribute2 == null)
          {
            return false;
          }

          return CompareGuids(activity.CustomAttribute2.ID, ObjectString, Relation);
        case ActivityProperty.CustomAttribute3:
          if (activity.CustomAttribute3 == null)
          {
            return false;
          }

          return CompareGuids(activity.CustomAttribute3.ID, ObjectString, Relation);
        case ActivityProperty.ActualFinishDate:
          return CompareDates(activity.ActualFinishDate, ObjectString, Relation);
        case ActivityProperty.ActualStartDate:
          return CompareDates(activity.ActualStartDate, ObjectString, Relation);
        case ActivityProperty.EarlyFinishDate:
          return CompareDates(activity.EarlyFinishDate, ObjectString, Relation);
        case ActivityProperty.EarlyStartDate:
          return CompareDates(activity.EarlyStartDate, ObjectString, Relation);
        case ActivityProperty.FinishDate:
          return CompareDates(activity.FinishDate, ObjectString, Relation);
        case ActivityProperty.LateFinishDate:
          return CompareDates(activity.LateFinishDate, ObjectString, Relation);
        case ActivityProperty.LateStartDate:
          return CompareDates(activity.LateStartDate, ObjectString, Relation);
        case ActivityProperty.StartDate:
          return CompareDates(activity.StartDate, ObjectString, Relation);
        case ActivityProperty.TotalActualCosts:
          return CompareDecimals(activity.TotalActualCosts, ObjectString, Relation);
        case ActivityProperty.TotalPlannedCosts:
          return CompareDecimals(activity.TotalPlannedCosts, ObjectString, Relation);
        case ActivityProperty.TotalBudget:
          return CompareDecimals(activity.TotalBudget, ObjectString, Relation);
        case ActivityProperty.ActualDuration:
          return CompareIntegers(activity.ActualDuration, ObjectString, Relation);
        case ActivityProperty.RetardedDuration:
          return CompareIntegers(activity.RetardedDuration, ObjectString, Relation);
        case ActivityProperty.FreeFloat:
          return CompareIntegers(activity.FreeFloat, ObjectString, Relation);
        case ActivityProperty.OriginalDuration:
          return CompareIntegers(activity.OriginalDuration, ObjectString, Relation);
        case ActivityProperty.RemainingDuration:
          return CompareIntegers(activity.RemainingDuration, ObjectString, Relation);
        case ActivityProperty.TotalFloat:
          return CompareIntegers(activity.TotalFloat, ObjectString, Relation);
        case ActivityProperty.PercentComplete:
          return CompareDoubles(activity.PercentComplete, ObjectString, Relation);
        default: throw new ApplicationException("ActivityProperty wurde nicht gefunden.");
      }
    }

    private static bool CompareStrings(string s1, string obj, FilterRelation relation)
    {
      string s2 = obj.ToString();
      return relation switch
      {
        FilterRelation.EqualTo => s1 == s2,
        FilterRelation.GreaterThan => s1.CompareTo(s2) > 0,
        FilterRelation.GreaterThanOrEqualTo => s1.CompareTo(s2) >= 0,
        FilterRelation.LessThan => s1.CompareTo(s2) < 0,
        FilterRelation.LessThanOrEqualTo => s1.CompareTo(s2) <= 0,
        FilterRelation.NotEqualTo => s1 != s2,
        _ => false,
      };
    }

    private static bool CompareDates(DateTime? d1, string obj, FilterRelation relation)
    {
      DateTime? d2 = null;
      if (DateTime.TryParse(obj, out var d))
      {
        d2 = d;
      }

      return relation switch
      {
        FilterRelation.EqualTo => d1 == d2,
        FilterRelation.GreaterThan => d1 > d2,
        FilterRelation.GreaterThanOrEqualTo => d1 >= d2,
        FilterRelation.LessThan => d1 < d2,
        FilterRelation.LessThanOrEqualTo => d1 <= d2,
        FilterRelation.NotEqualTo => d1 != d2,
        _ => false,
      };
    }

    private static bool CompareDecimals(decimal? d1, string obj, FilterRelation relation)
    {
      decimal? d2 = null;
      if (decimal.TryParse(obj, out decimal d))
      {
        d2 = d;
      }

      return relation switch
      {
        FilterRelation.EqualTo => d1 == d2,
        FilterRelation.GreaterThan => d1 > d2,
        FilterRelation.GreaterThanOrEqualTo => d1 >= d2,
        FilterRelation.LessThan => d1 < d2,
        FilterRelation.LessThanOrEqualTo => d1 <= d2,
        FilterRelation.NotEqualTo => d1 != d2,
        _ => false,
      };
    }

    private static bool CompareDoubles(double? d1, string obj, FilterRelation relation)
    {
      double? d2 = null;
      if (double.TryParse(obj, out double d))
      {
        d2 = d;
      }

      return relation switch
      {
        FilterRelation.EqualTo => d1 == d2,
        FilterRelation.GreaterThan => d1 > d2,
        FilterRelation.GreaterThanOrEqualTo => d1 >= d2,
        FilterRelation.LessThan => d1 < d2,
        FilterRelation.LessThanOrEqualTo => d1 <= d2,
        FilterRelation.NotEqualTo => d1 != d2,
        _ => false,
      };
    }

    private static bool CompareIntegers(int? i1, string obj, FilterRelation relation)
    {
      int? i2 = null;
      if (int.TryParse(obj, out int i))
      {
        i2 = i;
      }

      return relation switch
      {
        FilterRelation.EqualTo => i1 == i2,
        FilterRelation.GreaterThan => i1 > i2,
        FilterRelation.GreaterThanOrEqualTo => i1 >= i2,
        FilterRelation.LessThan => i1 < i2,
        FilterRelation.LessThanOrEqualTo => i1 <= i2,
        FilterRelation.NotEqualTo => i1 != i2,
        _ => false,
      };
    }

    private static bool CompareGuids(string id1, string obj, FilterRelation relation)
    {
      return Guid.TryParse(id1, out var id)
        ? CompareGuids(id, obj, relation)
        : throw new ArgumentException("Cannot parse into Guid.", nameof(id1));
    }

    private static bool CompareGuids(Guid? id1, string obj, FilterRelation relation)
    {
      Guid? id2 = null;
      if (Guid.TryParse(obj, out var id))
      {
        id2 = id;
      }

      return relation switch
      {
        FilterRelation.EqualTo => id1 == id2,
        FilterRelation.NotEqualTo => id1 != id2,
        _ => throw new ArgumentException(null, nameof(relation)),
      };
    }

    private WBSItem FindWBSItem(string id)
    {
      return Layout == null || Layout.Schedule == null ? null : FindWBSItem(Layout.Schedule.WBSItem, id);
    }

    private WBSItem FindWBSItem(WBSItem parent, string id)
    {
      if (parent == null)
      {
        return null;
      }

      if (parent.ID == id)
      {
        return parent;
      }

      foreach (var child in parent.Children)
      {
        var foundItem = FindWBSItem(child, id);
        if (foundItem != null)
        {
          return foundItem;
        }
      }
      return null;
    }

    public static FilterRelation[] GetValidFilterRelations(ActivityProperty property)
    {
      return property switch
      {
        ActivityProperty.None => Array.Empty<FilterRelation>(),
        ActivityProperty.Number or ActivityProperty.Name or ActivityProperty.EarlyStartDate or ActivityProperty.EarlyFinishDate or ActivityProperty.LateStartDate or ActivityProperty.LateFinishDate or ActivityProperty.ActualStartDate or ActivityProperty.ActualFinishDate or ActivityProperty.StartDate or ActivityProperty.FinishDate or ActivityProperty.PercentComplete or ActivityProperty.OriginalDuration or ActivityProperty.RemainingDuration or ActivityProperty.RetardedDuration or ActivityProperty.ActualDuration or ActivityProperty.TotalFloat or ActivityProperty.FreeFloat or ActivityProperty.TotalBudget or ActivityProperty.TotalActualCosts or ActivityProperty.TotalPlannedCosts => new FilterRelation[] {
            FilterRelation.EqualTo,
            FilterRelation.GreaterThan,
            FilterRelation.GreaterThanOrEqualTo,
            FilterRelation.LessThan,
            FilterRelation.LessThanOrEqualTo,
            FilterRelation.NotEqualTo
          },
        ActivityProperty.WBSItem or ActivityProperty.Fragnet or ActivityProperty.CustomAttribute1 or ActivityProperty.CustomAttribute2 or ActivityProperty.CustomAttribute3 => new FilterRelation[] {
            FilterRelation.EqualTo,
            FilterRelation.NotEqualTo
          },
        _ => throw new ArgumentException(null, nameof(property)),
      };
    }

    public override string ToString()
    {
      return Name;
    }
  }
}
