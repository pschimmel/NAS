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

    public VisibleResource(Resource resource)
    {
      Resource = resource;
    }

    private VisibleResource(VisibleResource other)
      : this(other.Resource)
    {
      ShowResourceAllocation = other.ShowResourceAllocation;
      ShowBudget = other.ShowBudget;
      ShowPlannedCosts = other.ShowPlannedCosts;
      ShowActualCosts = other.ShowActualCosts;
      AggregationType = other.AggregationType;
    }

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

    public Resource Resource { get; }

    public VisibleResource Clone()
    {
      return new VisibleResource(this);
    }

    public VisibleResource Clone(Resource resource)
    {
      return new VisibleResource(resource)
      {
        ShowResourceAllocation = ShowResourceAllocation,
        ShowBudget = ShowBudget,
        ShowPlannedCosts = ShowPlannedCosts,
        ShowActualCosts = ShowActualCosts,
        AggregationType = AggregationType
      };
    }
  }
}
