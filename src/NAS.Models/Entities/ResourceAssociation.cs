namespace NAS.Models.Entities
{
  public class ResourceAssociation : NASObject
  {
    private decimal _budget;
    private decimal _fixedCosts;
    private double _unitsPerDay;

    public ResourceAssociation(Activity activity, Resource resource)
    {
      Activity = activity;
      Resource = resource;
    }

    public decimal Budget
    {
      get => _budget;
      set
      {
        if (_budget != value)
        {
          _budget = value;
          OnPropertyChanged(nameof(Budget));
        }
      }
    }

    public decimal FixedCosts
    {
      get => _fixedCosts;
      set
      {
        if (_fixedCosts != value)
        {
          _fixedCosts = value;
          OnPropertyChanged(nameof(FixedCosts));
        }
      }
    }

    public double UnitsPerDay
    {
      get => _unitsPerDay;
      set
      {
        if (_unitsPerDay != value)
        {
          _unitsPerDay = value;
          OnPropertyChanged(nameof(UnitsPerDay));
        }
      }
    }

    public Activity Activity { get; }

    public Resource Resource { get; }

    /// <summary>
    /// Gets the actual costs.
    /// </summary>
    public decimal PlannedCosts
    {
      get
      {
        decimal d = FixedCosts;
        if (Resource is MaterialResource)
        {
          d += Convert.ToDecimal(Convert.ToDouble(Resource.CostsPerUnit) * UnitsPerDay * Convert.ToDouble(Activity.OriginalDuration));
        }
        else if (Resource is WorkResource)
        {
          d += Convert.ToDecimal(Convert.ToDouble(Resource.CostsPerUnit) * UnitsPerDay * Convert.ToDouble(Activity.OriginalDuration));
        }

        if (Resource is CalendarResource)
        {
          d += Convert.ToDecimal(Convert.ToDouble(Resource.CostsPerUnit) * UnitsPerDay * (Activity.EarlyFinishDate - Activity.EarlyStartDate).TotalDays);
        }
        return d;
      }
    }

    /// <summary>
    /// Gets the actual costs.
    /// </summary>
    public decimal ActualCosts
    {
      get
      {
        decimal d = FixedCosts;
        if (Resource is MaterialResource)
        {
          d += Convert.ToDecimal(Convert.ToDouble(Resource.CostsPerUnit) * UnitsPerDay * Convert.ToDouble(Activity.ActualDuration));
        }
        else if (Resource is WorkResource)
        {
          d += Convert.ToDecimal(Convert.ToDouble(Resource.CostsPerUnit) * UnitsPerDay * Convert.ToDouble(Activity.ActualDuration));
        }

        if (Resource is CalendarResource)
        {
          d += Convert.ToDecimal(Convert.ToDouble(Resource.CostsPerUnit) * UnitsPerDay * (Activity.FinishDate - Activity.StartDate).TotalDays);
        }
        return d;
      }
    }

    public override string ToString()
    {
      return Resource == null ? string.Empty : Resource.ToString();
    }
  }
}
