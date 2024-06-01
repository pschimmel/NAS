using NAS.Models.Enums;

namespace NAS.Models.Entities
{
  public class VisibleResource : NASObject
  {
    private bool _showResourceAllocation;
    private bool _showBudget;
    private bool _showPlannedCosts;
    private bool _showActualCosts;
    private TimeAggregateType _aggregationType;

    public VisibleResource(Layout layout, Resource resource)
    {
      Layout = layout;
      Resource = resource;
      layout.VisibleResources.Add(this);
    }

    public VisibleResource(VisibleResource other)
      : this(other.Layout, other.Resource)
    { }

    public bool ShowResourceAllocation
    {
      get => _showResourceAllocation;
      set
      {
        if (_showResourceAllocation != value)
        {
          _showResourceAllocation = value;
          OnPropertyChanged(nameof(ShowResourceAllocation));
        }
      }
    }

    public bool ShowBudget
    {
      get => _showBudget;
      set
      {
        if (_showBudget != value)
        {
          _showBudget = value;
          OnPropertyChanged(nameof(ShowBudget));
        }
      }
    }

    public bool ShowPlannedCosts
    {
      get => _showPlannedCosts;
      set
      {
        if (_showPlannedCosts != value)
        {
          _showPlannedCosts = value;
          OnPropertyChanged(nameof(ShowPlannedCosts));
        }
      }
    }

    public bool ShowActualCosts
    {
      get => _showActualCosts;
      set
      {
        if (_showActualCosts != value)
        {
          _showActualCosts = value;
          OnPropertyChanged(nameof(ShowActualCosts));
        }
      }
    }

    public TimeAggregateType AggregationType
    {
      get => _aggregationType;
      set
      {
        if (_aggregationType != value)
        {
          _aggregationType = value;
          OnPropertyChanged(nameof(AggregationType));
        }
      }
    }

    public Layout Layout { get; }

    public Resource Resource { get; }
  }
}
