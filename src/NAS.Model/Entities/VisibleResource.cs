using NAS.Model.Enums;

namespace NAS.Model.Entities
{
  public class VisibleResource : NASObject
  {
    private bool showResourceAllocation;
    private bool showBudget;
    private bool showPlannedCosts;
    private bool showActualCosts;
    private TimeAggregateType aggregationType;

    internal VisibleResource()
    { }

    public VisibleResource(VisibleResource other)
      : this(other.Layout, other.Resource)
    { }

    public VisibleResource(Layout layout, Resource resource) : this()
    {
      Layout = layout;
      Layout_ID = layout.ID;
      Resource = resource;
      Resource_ID = resource.ID;
      layout.VisibleResources.Add(this);
      resource.VisibleResources.Add(this);
    }

    public bool ShowResourceAllocation
    {
      get => showResourceAllocation;
      set
      {
        if (showResourceAllocation != value)
        {
          showResourceAllocation = value;
          OnPropertyChanged(nameof(ShowResourceAllocation));
        }
      }
    }

    public bool ShowBudget
    {
      get => showBudget;
      set
      {
        if (showBudget != value)
        {
          showBudget = value;
          OnPropertyChanged(nameof(ShowBudget));
        }
      }
    }

    public bool ShowPlannedCosts
    {
      get => showPlannedCosts;
      set
      {
        if (showPlannedCosts != value)
        {
          showPlannedCosts = value;
          OnPropertyChanged(nameof(ShowPlannedCosts));
        }
      }
    }

    public bool ShowActualCosts
    {
      get => showActualCosts;
      set
      {
        if (showActualCosts != value)
        {
          showActualCosts = value;
          OnPropertyChanged(nameof(ShowActualCosts));
        }
      }
    }

    public virtual TimeAggregateType AggregationType
    {
      get => aggregationType;
      set
      {
        if (aggregationType != value)
        {
          aggregationType = value;
          OnPropertyChanged(nameof(AggregationType));
        }
      }
    }

    public string Layout_ID { get; set; }

    public string Resource_ID { get; set; }

    public virtual Layout Layout { get; set; }

    public virtual Resource Resource { get; set; }
  }
}
