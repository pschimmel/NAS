using System.Diagnostics;
using System.Windows.Input;
using ES.Tools.Core.MVVM;
using NAS.Models.Entities;
using NAS.Models.Enums;
using NAS.ViewModels.Base;
using NAS.ViewModels.Helpers;

namespace NAS.ViewModels
{
  public class ResourceViewModel : ViewModelBase
  {
    #region Events

    public event EventHandler Refresh;
    public event EventHandler Rebuild;

    #endregion

    #region Fields

    private readonly ScheduleViewModel _vm;
    private ResourceInfoHelper _helper;
    private readonly ICommand _closeCommand;

    #endregion

    #region Constructors

    public ResourceViewModel(VisibleResource visibleResource, ScheduleViewModel parentViewModel, ICommand closeCommand = null)
    {
      _vm = parentViewModel;
      Resource = visibleResource;
      Resource.PropertyChanged += (sender, args) => Refresh?.Invoke(this, EventArgs.Empty);

      if (closeCommand != null)
      {
        _closeCommand = closeCommand;
        closeCommand.CanExecuteChanged += (sender, e) => (CloseCommand as ActionCommand).RaiseCanExecuteChanged();
      }

      _vm.Schedule.PropertyChanged += (sender, e) =>
      {
        if (e.PropertyName is "StartDate" or "LastDay")
        {
          RefreshData();
          OnPropertyChanged(nameof(End));
        }
      };

      _vm.PropertyChanged += (sender, e) =>
      {
        if (e.PropertyName == "Zoom")
        {
          OnPropertyChanged(nameof(Zoom));
        }
      };

      RefreshData();
      SelectAggregationTypeCommand = new ActionCommand(SelectAggregationTypeCommandExecute, SelectAggregationTypeCommandCanExecute);
      CloseCommand = new ActionCommand(CloseCommandExecute, () => CloseCommandCanExecute);
    }

    #endregion

    #region Properties

    public DateTime Start => _vm.Schedule.StartDate;

    public DateTime End => _vm.Schedule.LastDay;

    public VisibleResource Resource { get; }

    public Dictionary<DateTime, double> ResourceAllocation => _helper.ResourceAllocation;

    public Dictionary<DateTime, decimal> ResourceBudget => _helper.ResourceBudget;

    public Dictionary<DateTime, decimal> ResourceCostsActual => _helper.ResourceCostsActual;

    public Dictionary<DateTime, decimal> ResourceCostsPlanned => _helper.ResourceCostsPlanned;

    public double ResourceMax
    {
      get
      {
        double maxHeight = 0;
        if (Resource.ShowResourceAllocation)
        {
          maxHeight = Math.Max(maxHeight, _helper.ResourceAllocationMax);
        }

        if (Resource.ShowBudget)
        {
          maxHeight = Math.Max(maxHeight, Convert.ToDouble(_helper.ResourceBudgetMax));
        }

        if (Resource.ShowActualCosts)
        {
          maxHeight = Math.Max(maxHeight, Convert.ToDouble(_helper.ResourceCostsActualMax));
        }

        if (Resource.ShowPlannedCosts)
        {
          maxHeight = Math.Max(maxHeight, Convert.ToDouble(_helper.ResourceCostsPlannedMax));
        }

        return maxHeight;
      }
    }

    public double ResourceMiddle => ResourceMax / 2;

    public double Zoom => _vm.Zoom;

    public TimeAggregateType AggregationType
    {
      get => Resource.AggregationType;
      set
      {
        if (Resource.AggregationType != value)
        {
          Resource.AggregationType = value;
          OnPropertyChanged(nameof(AggregationType));
          RefreshData();
          Rebuild?.Invoke(this, EventArgs.Empty);
        }
      }
    }

    public bool ShowBudget
    {
      get => Resource.ShowBudget;
      set
      {
        if (Resource.ShowBudget != value)
        {
          Resource.ShowBudget = value;
          OnPropertyChanged(nameof(ShowBudget));
          OnPropertyChanged(nameof(ResourceMax));
          OnPropertyChanged(nameof(ResourceMiddle));
        }
      }
    }

    public bool ShowPlannedCosts
    {
      get => Resource.ShowPlannedCosts;
      set
      {
        if (Resource.ShowPlannedCosts != value)
        {
          Resource.ShowPlannedCosts = value;
          OnPropertyChanged(nameof(ShowPlannedCosts));
          OnPropertyChanged(nameof(ResourceMax));
          OnPropertyChanged(nameof(ResourceMiddle));
        }
      }
    }

    public bool ShowActualCosts
    {
      get => Resource.ShowActualCosts;
      set
      {
        if (Resource.ShowActualCosts != value)
        {
          Resource.ShowActualCosts = value;
          OnPropertyChanged(nameof(ShowActualCosts));
          OnPropertyChanged(nameof(ResourceMax));
          OnPropertyChanged(nameof(ResourceMiddle));
        }
      }
    }

    public bool ShowResourceAllocation
    {
      get => Resource.ShowResourceAllocation;
      set
      {
        if (Resource.ShowResourceAllocation != value)
        {
          Resource.ShowResourceAllocation = value;
          OnPropertyChanged(nameof(ShowResourceAllocation));
          OnPropertyChanged(nameof(ResourceMax));
          OnPropertyChanged(nameof(ResourceMiddle));
        }
      }
    }

    #endregion

    #region Select Aggregation Type

    public ICommand SelectAggregationTypeCommand { get; }

    private void SelectAggregationTypeCommandExecute(object param)
    {
      Debug.Assert(param is TimeAggregateType);
      AggregationType = (TimeAggregateType)param;
    }

    private bool SelectAggregationTypeCommandCanExecute(object param)
    {
      Debug.Assert(param is TimeAggregateType);
      return AggregationType != (TimeAggregateType)param;
    }

    #endregion

    #region Close

    public ICommand CloseCommand { get; }

    private void CloseCommandExecute()
    {
      _closeCommand.Execute(this);
    }

    private bool CloseCommandCanExecute => _closeCommand != null && _closeCommand.CanExecute(this);

    #endregion

    #region Private Members

    private void RefreshData()
    {
      _helper = new ResourceInfoHelper(Resource.Resource, _vm.Schedule, Start, End, AggregationType);
      OnPropertyChanged(nameof(ResourceAllocation));
      OnPropertyChanged(nameof(ResourceBudget));
      OnPropertyChanged(nameof(ResourceCostsActual));
      OnPropertyChanged(nameof(ResourceCostsPlanned));
      OnPropertyChanged(nameof(ResourceMax));
      OnPropertyChanged(nameof(ResourceMiddle));
    }

    #endregion
  }
}
