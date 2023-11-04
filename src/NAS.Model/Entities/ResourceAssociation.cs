using System;

namespace NAS.Model.Entities
{
  public class ResourceAssociation : NASObject
  {
    private decimal budget;
    private decimal fixedCosts;
    private double unitsPerDay;

    public decimal Budget
    {
      get => budget;
      set
      {
        if (budget != value)
        {
          budget = value;
          OnPropertyChanged(nameof(Budget));
        }
      }
    }

    public decimal FixedCosts
    {
      get => fixedCosts;
      set
      {
        if (fixedCosts != value)
        {
          fixedCosts = value;
          OnPropertyChanged(nameof(FixedCosts));
        }
      }
    }

    public double UnitsPerDay
    {
      get => unitsPerDay;
      set
      {
        if (unitsPerDay != value)
        {
          unitsPerDay = value;
          OnPropertyChanged(nameof(UnitsPerDay));
        }
      }
    }

    public virtual Activity Activity { get; set; }
    public virtual Resource Resource { get; set; }

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
