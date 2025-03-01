using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using ES.Tools.Core.MVVM;
using NAS.Models.Base;
using NAS.Models.Entities;
using NAS.Models.Enums;
using NAS.Models.Scheduler;
using NAS.Resources;
using NAS.ViewModels.Base;
using NAS.ViewModels.Helpers;

namespace NAS.ViewModels
{
  public class ScheduleViewModel : ViewModelBase
  {
    #region Events

    public event EventHandler<ProgressEventArgs> CalculationProgress;
    public event EventHandler ColumnsChanged;
    public event EventHandler<ItemEventArgs<ActivityViewModel>> ActivityAdded;
    public event EventHandler<ItemEventArgs<ActivityViewModel>> ActivityDeleted;
    public event EventHandler<ItemEventArgs<ActivityViewModel>> ActivityChanged;
    public event EventHandler<ItemEventArgs<RelationshipViewModel>> RelationshipAdded;
    public event EventHandler<ItemEventArgs<RelationshipViewModel>> RelationshipDeleted;
    public event EventHandler<ItemEventArgs<RelationshipViewModel>> RelationshipChanged;
    public event EventHandler<ItemEventArgs<Layout>> RefreshLayout;
    public event EventHandler LayoutTypeChanged;

    #endregion

    #region Fields

    private bool _isBusy = false;
    private bool _isLoading = false;
    private bool _isSaving = false;
    private double _zoom = 1.0;
    private RelationshipViewModel _currentRelationship;
    private ActivityViewModel _currentActivity;
    private IVisibleLayoutViewModel _activeLayout;
    private ActionCommand<DateTime?> _calculateCommand;
    private ActionCommand _editSchedulingSettingsCommand;
    private ActionCommand<string> _addActivityCommand;
    private ActionCommand<string> _addMilestoneCommand;
    private ActionCommand _removeActivityCommand;
    private ActionCommand _editActivityCommand;
    private ActionCommand<AddRelationshipInfo> _addRelationshipCommand;
    private ActionCommand _removeRelationshipCommand;
    private ActionCommand _editRelationshipCommand;
    private ActionCommand _editLogicCommand;
    private ActionCommand _editPropertiesCommand;
    private ActionCommand _editWBSCommand;
    private ActionCommand _editResourcesCommand;
    private ActionCommand _editCustomAttributesCommand;
    private ActionCommand _editCalendarsCommand;
    private ActionCommand<IVisibleLayoutViewModel> _selectLayoutCommand;
    private ActionCommand<string> _changeZoomCommand;
    private ActionCommand _showRelationshipsCommand;
    private ActionCommand _showFloatCommand;
    private ActionCommand _showResourceCommand;
    private ActionCommand _closeResourceCommand;
    private ActionCommand _addGanttLayoutCommand;
    private ActionCommand _addPERTLayoutCommand;
    private ActionCommand _removeLayoutCommand;
    private ActionCommand _editLayoutCommand;
    private ActionCommand _copyLayoutCommand;
    private ActionCommand _editSortingAndGroupingCommand;
    private ActionCommand _editFiltersCommand;
    private ActionCommand _autoArrangePERTCommand;
    private ActionCommand _editBaselinesCommand;
    private ActionCommand _editFragnetsCommand;
    private ActionCommand _setFragnetVisibleCommand;
    private ActionCommand _compareWithBaselineCommand;
    private ActionCommand _compareWithDistortionsCommand;
    private ActionCommand _compareCommand;
    private ActionCommand _editDistortionsCommand;
    private ActionCommand _increaseDurationCommand;
    private ActionCommand _decreaseDurationCommand;
    private ActionCommand _splitActivityCommand;
    private ActionCommand _combineActivitiesCommand;
    private ActionCommand _changeIntoMilestoneCommand;
    private ActionCommand _changeIntoActivityCommand;
    private ActionCommand _editColumnsCommand;

    #endregion

    #region Constructor

    public ScheduleViewModel(Schedule schedule)
    {
      ArgumentNullException.ThrowIfNull(schedule);

      schedule.Validate();
      Schedule = schedule;

      Layouts = [];

      foreach (var layout in Schedule.Layouts)
      {
        AddLayoutVM(layout, layout == Schedule.ActiveLayout);
      }

      foreach (var activity in schedule.Activities)
      {
        Activities.Add(new ActivityViewModel(schedule, activity));
      }

      foreach (var relationship in schedule.Relationships)
      {
        Relationships.Add(new RelationshipViewModel(relationship));
      }


      Schedule.ActivityAdded += Schedule_ActivityAdded;
      Schedule.ActivityRemoved += Schedule_ActivityRemoved;
      RefreshResources();
    }

    private void Schedule_ActivityRemoved(object sender, ItemEventArgs<Activity> e)
    {
      Activities.Add(new ActivityViewModel(Schedule, e.Item));
    }

    private void Schedule_ActivityAdded(object sender, ItemEventArgs<Activity> e)
    {
      var deletedActivity = Activities.FirstOrDefault(x => x.Activity == e.Item);
      if (deletedActivity != null)
      {
        Activities.Remove(deletedActivity);
      }
    }

    #endregion

    #region Properties

    public override HelpTopic HelpTopicKey => HelpTopic.New;

    /// <summary>
    /// Gets the _scheduleVM.
    /// </summary>
    public Schedule Schedule { get; }

    /// <summary>
    /// Gets the layouts.
    /// </summary>
    public ObservableCollection<IVisibleLayoutViewModel> Layouts { get; }

    public ObservableCollection<ActivityViewModel> Activities { get; } = [];

    public ObservableCollection<RelationshipViewModel> Relationships { get; } = [];

    /// <summary>
    /// Gets the constraint types.
    /// </summary>
    public List<ConstraintType> ConstraintTypes => Enum.GetValues(typeof(ConstraintType)).Cast<ConstraintType>().ToList();

    /// <summary>
    /// Gets the activity properties.
    /// </summary>
    public List<ActivityProperty> ActivityProperties => Enum.GetValues(typeof(ActivityProperty)).Cast<ActivityProperty>().ToList();

    /// <summary>
    /// Gets the filter relations.
    /// </summary>
    public List<FilterRelation> FilterRelations => Enum.GetValues(typeof(FilterRelation)).Cast<FilterRelation>().ToList();

    /// <summary>
    /// Gets the WBS items.
    /// </summary>
    public List<WBSItem> WBSItems => new(Schedule.GetWBSItems());

    /// <summary>
    /// Gets a value indicating whether this instance is loading.
    /// </summary>
    /// <value>
    /// 	<column>true</column> if this instance is loading; otherwise, <column>false</column>.
    /// </value>
    public bool IsLoading
    {
      get => _isLoading;
      private set
      {
        if (_isLoading != value)
        {
          _isLoading = value;
          OnPropertyChanged(nameof(IsLoading));
        }
      }
    }

    /// <summary>
    /// Gets a value indicating whether this instance is saving.
    /// </summary>
    /// <value>
    ///   <column>true</column> if this instance is saving; otherwise, <column>false</column>.
    /// </value>
    public bool IsSaving
    {
      get => _isSaving;
      private set
      {
        if (_isSaving != value)
        {
          _isSaving = value;
          OnPropertyChanged(nameof(IsSaving));
        }
      }
    }

    /// <summary>
    /// Gets or sets the zoom.
    /// </summary>
    /// <value>
    /// The zoom.
    /// </value>
    public double Zoom
    {
      get => _zoom;
      set
      {
        if (_zoom != value)
        {
          if (value < 0.1)
          {
            value = 0.1;
          }

          if (value > 5)
          {
            value = 5;
          }

          if (_zoom != value)
          {
            _zoom = value;
            OnPropertyChanged(nameof(Zoom));
          }
        }
      }
    }

    /// <summary>
    /// Sets or gets the current milestone
    /// </summary>
    public ActivityViewModel CurrentActivity
    {
      get => _currentActivity;
      set
      {
        if (_currentActivity != value)
        {
          _currentActivity = value;
          if (_currentActivity != null)
          {
            CurrentRelationship = null;
          }

          OnPropertyChanged(nameof(CurrentActivity));
          OnPropertyChanged(nameof(CurrentActivityIsActivity));
          OnPropertyChanged(nameof(CurrentActivityIsNotFixed));
          OnPropertyChanged(nameof(CurrentActivityIsFixed));
          OnPropertyChanged(nameof(ActivityMenuVisible));
        }
      }
    }

    /// <summary>
    /// Gets or sets the current relationship.
    /// </summary>
    public RelationshipViewModel CurrentRelationship
    {
      get => _currentRelationship;
      set
      {
        if (_currentRelationship != value)
        {
          _currentRelationship = value;
          if (_currentRelationship != null)
          {
            CurrentActivity = null;
          }

          OnPropertyChanged(nameof(CurrentRelationship));
          OnPropertyChanged(nameof(RelationshipMenuVisible));
        }
      }
    }

    /// <summary>
    /// Gets a value indicating whether the activity menu is visible.
    /// </summary>
    public bool ActivityMenuVisible => _currentActivity != null;

    /// <summary>
    /// Gets a value indicating whether the relationship menu is visible.
    /// </summary>
    public bool RelationshipMenuVisible => _currentRelationship != null;

    /// <summary>
    /// Gets a value indicating whether the current activity is an activity.
    /// </summary>
    public bool CurrentActivityIsActivity => _currentActivity != null && _currentActivity.Activity.ActivityType == ActivityType.Activity;

    /// <summary>
    /// Gets a value indicating whether the current activity is fixed.
    /// </summary>
    public bool CurrentActivityIsFixed => _currentActivity != null && _currentActivity.IsFixed;

    /// <summary>
    /// Gets a value indicating whether the current activity is not fixed.
    /// </summary>
    public bool CurrentActivityIsNotFixed => CurrentActivity != null && !CurrentActivityIsFixed;

    public IVisibleLayoutViewModel ActiveLayout
    {
      get => _activeLayout;
      set
      {
        if (_activeLayout != value)
        {
          bool typeChanged = !Equals(_activeLayout?.LayoutType, value?.LayoutType);
          _activeLayout = value;
          Schedule.ActiveLayout = value?.Layout;
          OnPropertyChanged(nameof(ActiveLayout));
          OnRefreshLayout(typeChanged);
        }
        foreach (var l in Layouts)
        {
          l.IsVisible = l == value;
        }
      }
    }

    public bool ShowPERTControls => Schedule.ActiveLayout.LayoutType == LayoutType.PERT;

    public bool ShowGanttControls => Schedule.ActiveLayout.LayoutType == LayoutType.Gantt;

    #endregion

    #region Commands

    #region Start

    #region Calculate

    public ICommand CalculateCommand => _calculateCommand ??= new ActionCommand<DateTime?>(Calculate, CanCalculate);

    private void Calculate(DateTime? dataDate)
    {
      InstantHelpManager.Instance.SetHelpTopic(HelpTopic.Calculate);
      var scheduler = new Scheduler(Schedule, () =>
      {
        _isBusy = true;
        CalculationProgress?.Invoke(this, ProgressEventArgs.Starting);
      }, () =>
      {
        _isBusy = false;
        CalculationProgress?.Invoke(this, ProgressEventArgs.Finished);
        OnRefreshLayout(false);
      }, (value) =>
      {
        CalculationProgress?.Invoke(this, ProgressEventArgs.Progress(value));
      });
      scheduler.Message += (sender, e) =>
      {
        UserNotificationService.Instance.Information(e.Message);
      };

      if (dataDate == null)
      {
        var vm = new GetDateViewModel(NASResources.Reschedule, Schedule.DataDate, Schedule.StartDate);
        if (ViewFactory.Instance.ShowDialog(vm) == true)
        {
          dataDate = vm.Date;
        }
        else
        {
          return;
        }
      }

      scheduler.Calculate(dataDate.Value);
    }

    private bool CanCalculate(DateTime? _)
    {
      return !_isBusy;
    }

    #endregion

    #region Scheduling Settings

    public ICommand EditSchedulingSettingsCommand => _editSchedulingSettingsCommand ??= new ActionCommand(EditSchedulingSettings, CanEditSchedulingSettings);

    private void EditSchedulingSettings()
    {
      var vm = new EditSchedulingSettingsViewModel(Schedule);
      ViewFactory.Instance.ShowDialog(vm);
    }

    private bool CanEditSchedulingSettings()
    {
      return Schedule != null && !_isBusy;
    }

    #endregion

    #region Add Activity

    public ICommand AddActivityCommand => _addActivityCommand ??= new ActionCommand<string>(AddActivity, CanAddActivity);

    private void AddActivity(string param)
    {
      InstantHelpManager.Instance.SetHelpTopic(HelpTopic.Activity);
      var activity = Schedule.AddActivity(param?.ToString() == "Fixed");
      var newActivityVM = new ActivityViewModel(Schedule, activity);
      Activities.Add(newActivityVM);
      OnActivityAdded(newActivityVM);
      CurrentActivity = newActivityVM;
    }

    private bool CanAddActivity(string _)
    {
      return !_isBusy;
    }

    #endregion

    #region Add Milestone

    public ICommand AddMilestoneCommand => _addMilestoneCommand ??= new ActionCommand<string>(AddMilestone, CanAddMilestone);

    private void AddMilestone(string param)
    {
      InstantHelpManager.Instance.SetHelpTopic(HelpTopic.Activity);
      var milestone = Schedule.AddMilestone(param?.ToString() == "Fixed");
      var newActivityVM = new ActivityViewModel(Schedule, milestone);
      Activities.Add(newActivityVM);
      CurrentActivity = newActivityVM;
      OnActivityAdded(newActivityVM);
    }

    private bool CanAddMilestone(string _)
    {
      return !_isBusy;
    }

    #endregion

    #region Remove Activity

    public ICommand RemoveActivityCommand => _removeActivityCommand ??= new ActionCommand(RemoveActivity, CanRemoveActivity);

    private void RemoveActivity()
    {
      InstantHelpManager.Instance.SetHelpTopic(HelpTopic.Activity);
      string s = NASResources.MessageDeleteActivity;
      var activityVM = CurrentActivity;
      if (activityVM.Activity.ActivityType == ActivityType.Milestone)
      {
        s = NASResources.MessageDeleteMilestone;
      }

      s = string.Format(s, activityVM.DisplayName);
      UserNotificationService.Instance.Question(s, () =>
      {
        Schedule.RemoveActivity(activityVM.Activity);
        Activities.Remove(activityVM);
        OnActivityDeleted(activityVM);
        if (ActiveLayout.LayoutType == LayoutType.PERT)
        {
          var view = ViewModelExtensions.GetView(Activities);
          CurrentActivity = view.CurrentItem as ActivityViewModel;
        }
      });
    }

    private bool CanRemoveActivity()
    {
      return !_isBusy && CurrentActivity != null;
    }

    #endregion

    #region Edit Activity

    public ICommand EditActivityCommand => _editActivityCommand ??= new ActionCommand(EditActivity, CanEditActivity);

    private void EditActivity()
    {
      var vm = new EditActivityViewModel(Schedule, CurrentActivity.Activity);
      if (ViewFactory.Instance.ShowDialog(vm) == true)
      {
        SortFilterAndGroup(Activities);
        OnPropertyChanged(nameof(Activities));
        OnActivityChanged(CurrentActivity);
      }
    }

    private bool CanEditActivity()
    {
      return !_isBusy && CurrentActivity != null;
    }

    #endregion

    #region Add Relationship

    public ICommand AddRelationshipCommand => _addRelationshipCommand ??= new ActionCommand<AddRelationshipInfo>(AddRelationship, CanAddRelationship);

    private void AddRelationship(AddRelationshipInfo relationshipInfo)
    {
      InstantHelpManager.Instance.SetHelpTopic(HelpTopic.Relationship);
      Relationship relationship;

      if (relationshipInfo != null)
      {
        relationship = Schedule.AddRelationship(relationshipInfo.Activity1, relationshipInfo.Activity2, relationshipInfo.RelationshipType);
      }
      else
      {
        var vm = new EditRelationshipViewModel(Schedule);
        if (ViewFactory.Instance.ShowDialog(vm) != true)
        {
          return;
        }

        relationship = Schedule.AddRelationship(vm.SelectedActivity1, vm.SelectedActivity2, vm.SelectedRelationshipType);
        relationship.Lag = vm.Lag;
      }

      if (!new Scheduler(Schedule).CheckForLoops(Activities.Select(x => x.Activity).ToList(), [.. Schedule.Relationships]))
      {
        UserNotificationService.Instance.Error(NASResources.MessageNetworkLoopError);
        Schedule.RemoveRelationship(relationship);
      }
      else
      {
        CurrentRelationship = new RelationshipViewModel(relationship);
        OnRelationshipAdded(CurrentRelationship);
      }
    }

    private bool CanAddRelationship(AddRelationshipInfo _)
    {
      return !_isBusy;
    }

    #endregion

    #region Remove Relationship

    public ICommand RemoveRelationshipCommand => _removeRelationshipCommand ??= new ActionCommand(RemoveRelationship, CanRemoveRelationship);

    private void RemoveRelationship()
    {
      InstantHelpManager.Instance.SetHelpTopic(HelpTopic.Relationship);
      UserNotificationService.Instance.Question(string.Format(NASResources.MessageDeleteRelationship, CurrentRelationship.DisplayName), () =>
      {
        var relationship = CurrentRelationship.Relationship;
        Schedule.RemoveRelationship(relationship);
        OnRelationshipDeleted(CurrentRelationship);
        CurrentRelationship = null;
      });
    }

    private bool CanRemoveRelationship()
    {
      return !_isBusy && CurrentRelationship != null;
    }

    #endregion

    #region Edit Relationship

    public ICommand EditRelationshipCommand => _editRelationshipCommand ??= new ActionCommand(EditRelationship, CanEditRelationship);

    private void EditRelationship()
    {
      var relationship = CurrentRelationship.Relationship;
      using var vm = new EditRelationshipViewModel(Schedule, relationship);

      if (ViewFactory.Instance.ShowDialog(vm) == true)
      {
        if (vm.Validate().IsOK)
        {
          CurrentRelationship.Relationship = relationship;
          OnRelationshipChanged(CurrentRelationship);
        }
      }
    }

    private bool CanEditRelationship()
    {
      return CurrentRelationship != null && !_isBusy;
    }

    #endregion

    #region Edit Logic

    public ICommand EditLogicCommand => _editLogicCommand ??= new ActionCommand(EditLogic, CanEditLogic);

    private void EditLogic()
    {
      using var vm = new EditLogicViewModel(Schedule, CurrentActivity.Activity);
      ViewFactory.Instance.ShowDialog(vm);
    }

    private bool CanEditLogic()
    {
      return CurrentActivity != null;
    }

    #endregion

    #region Properties

    public ICommand EditPropertiesCommand => _editPropertiesCommand ??= new ActionCommand(EditProperties, CanEditProperties);

    private void EditProperties()
    {
      using var vm = new EditPropertiesViewModel(Schedule);
      ViewFactory.Instance.ShowDialog(vm);
    }

    private bool CanEditProperties()
    {
      return !_isBusy;
    }

    #endregion

    #region Edit WBS

    public ICommand EditWBSCommand => _editWBSCommand ??= new ActionCommand(EditWBS, CanEditWBS);

    private void EditWBS()
    {
      using var vm = new EditWBSViewModel(Schedule);
      ViewFactory.Instance.ShowDialog(vm);
    }

    private bool CanEditWBS()
    {
      return !_isBusy;
    }

    #endregion

    #region Edit Calendars

    public ICommand EditCalendarsCommand => _editCalendarsCommand ??= new ActionCommand(EditCalendars, CanEditCalendars);

    private void EditCalendars()
    {
      using var vm = new EditCalendarsViewModel(Schedule);
      ViewFactory.Instance.ShowDialog(vm);
    }

    private bool CanEditCalendars()
    {
      return !_isBusy;
    }

    #endregion

    #region Edit Resources

    public ICommand EditResourcesCommand => _editResourcesCommand ??= new ActionCommand(EditResources, CanEditResources);

    private void EditResources()
    {
      using var vm = new EditResourcesViewModel(Schedule);
      ViewFactory.Instance.ShowDialog(vm);
    }

    private bool CanEditResources()
    {
      return !_isBusy;
    }

    #endregion

    #region Edit Custom Attributes

    public ICommand EditCustomAttributesCommand => _editCustomAttributesCommand ??= new ActionCommand(EditCustomAttributes, CanEditCustomAttributes);

    private void EditCustomAttributes()
    {
      using var vm = new EditCustomAttributesViewModel(Schedule);
      ViewFactory.Instance.ShowDialog(vm);
    }

    private bool CanEditCustomAttributes()
    {
      return !_isBusy;
    }

    #endregion

    #endregion

    #region View

    #region Select Layout

    public ICommand SelectLayoutCommand => _selectLayoutCommand ??= new ActionCommand<IVisibleLayoutViewModel>(SelectLayout, CanSelectLayout);

    private void SelectLayout(IVisibleLayoutViewModel layout)
    {
      InstantHelpManager.Instance.SetHelpTopic(HelpTopic.View);
      ActiveLayout = layout;
    }

    private bool CanSelectLayout(IVisibleLayoutViewModel layout)
    {
      return layout != null && !_isBusy;
    }

    #endregion

    #region Change Zoom

    public ICommand ChangeZoomCommand => _changeZoomCommand ??= new ActionCommand<string>(ChangeZoom, CanChangeZoom);

    private void ChangeZoom(string zoom)
    {
      InstantHelpManager.Instance.SetHelpTopic(HelpTopic.Relationship);
      Zoom = double.Parse(zoom);
    }

    private bool CanChangeZoom(string zoom)
    {
      return !_isBusy
             && double.TryParse(zoom, out double zoomAsDouble)
             && double.IsFinite(zoomAsDouble)
             && double.IsPositive(zoomAsDouble);
    }

    #endregion

    #region Show Relationships

    public ICommand ShowRelationshipsCommand => _showRelationshipsCommand ??= new ActionCommand(ShowRelationships, CanShowRelationships);

    private void ShowRelationships()
    {
      InstantHelpManager.Instance.SetHelpTopic(HelpTopic.View);
      OnRefreshLayout(false);
    }

    private bool CanShowRelationships()
    {
      return !_isBusy;
    }

    #endregion

    #region Show Float

    public ICommand ShowFloatCommand => _showFloatCommand ??= new ActionCommand(ShowFloat, CanShowFloat);

    private void ShowFloat()
    {
      InstantHelpManager.Instance.SetHelpTopic(HelpTopic.View);
      OnRefreshLayout(false);
    }

    private bool CanShowFloat()
    {
      return !_isBusy;
    }

    #endregion

    #region Show Resource

    public ICommand ShowResourceCommand => _showResourceCommand ??= new ActionCommand(ShowResource, CanShowResource);

    private void ShowResource()
    {
      using var vm = new SelectResourceViewModel(Schedule.Resources);
      if (ViewFactory.Instance.ShowDialog(vm) == true && vm.SelectedResource != null)
      {
        var vr = new VisibleResource(vm.SelectedResource);
        Resources.Add(new ResourceViewModel(vr, this, CloseResourceCommand));
      }
    }

    private bool CanShowResource()
    {
      return !_isBusy && Schedule.Resources.Count > 0;
    }

    #endregion

    #region Close Resource

    public ICommand CloseResourceCommand => _closeResourceCommand ??= new ActionCommand(CloseResource, CanCloseResource);

    private void CloseResource(object param)
    {
      if (param is ResourceViewModel resourceVM)
      {
        Resources.Remove(resourceVM);
        OnRefreshLayout(false);
      }
    }

    private bool CanCloseResource(object param)
    {
      return !_isBusy && Resources.Contains(param as ResourceViewModel);
    }

    #endregion

    #region Add Gantt Layout

    public ICommand AddGanttLayoutCommand => _addGanttLayoutCommand ??= new ActionCommand(AddGanttLayout, CanAddGanttLayout);

    private void AddGanttLayout()
    {
      var item = new GanttLayout();

      using var vm = new EditLayoutViewModel(Schedule, item);
      if (ViewFactory.Instance.ShowDialog(vm) == true)
      {
        AddLayoutVM(item, true);
      }
    }

    private bool CanAddGanttLayout()
    {
      return !_isBusy;
    }

    #endregion

    #region Add PERT Layout

    public ICommand AddPERTLayoutCommand => _addPERTLayoutCommand ??= new ActionCommand(AddPERTLayout, CanAddPERTLayout);

    private void AddPERTLayout()
    {
      var item = new PERTLayout();

      using var vm = new EditLayoutViewModel(Schedule, item);
      if (ViewFactory.Instance.ShowDialog(vm) == true)
      {
        AddLayoutVM(item, true);
      }
    }

    private bool CanAddPERTLayout()
    {
      return !_isBusy;
    }

    #endregion

    #region Remove Layout

    public ICommand RemoveLayoutCommand => _removeLayoutCommand ??= new ActionCommand(RemoveLayout, CanRemoveLayout);

    private void RemoveLayout()
    {
      InstantHelpManager.Instance.SetHelpTopic(HelpTopic.Layout);
      UserNotificationService.Instance.Question(NASResources.MessageDeleteLayout, () =>
      {
        var layoutToRemove = ActiveLayout;
        int idx = Layouts.IndexOf(ActiveLayout);
        ActiveLayout = idx + 1 < Layouts.Count ? Layouts[idx + 1] : Layouts[idx - 1];
        Layouts.Remove(layoutToRemove);
      });
    }

    private bool CanRemoveLayout()
    {
      return ActiveLayout != null && Schedule.Layouts.Count > 1;
    }

    #endregion

    #region Copy Layout

    public ICommand CopyLayoutCommand => _copyLayoutCommand ??= new ActionCommand(CopyLayout, CanCopyLayout);

    private void CopyLayout()
    {
      InstantHelpManager.Instance.SetHelpTopic(HelpTopic.Layout);
      var vm = new GetTextViewModel(NASResources.CopyLayout, NASResources.Name, ActiveLayout.Name + " (" + NASResources.Copy + ")");
      if (ViewFactory.Instance.ShowDialog(vm) == true)
      {
        var newLayout = ActiveLayout.Layout.Clone();
        newLayout.Name = vm.Text;
        AddLayoutVM(newLayout, true);
      }
    }

    private bool CanCopyLayout()
    {
      return !_isBusy && ActiveLayout != null;
    }

    #endregion

    #region Edit Layout

    public ICommand EditLayoutCommand => _editLayoutCommand ??= new ActionCommand(EditLayout, CanEditLayout);

    private void EditLayout()
    {
      var layout = ActiveLayout.Layout;
      using var vm = new EditLayoutViewModel(Schedule, layout);
      ViewFactory.Instance.ShowDialog(vm);
    }

    private bool CanEditLayout()
    {
      return ActiveLayout != null;
    }

    #endregion

    #region Edit Sorting and Grouping

    public ICommand EditSortingAndGroupingCommand => _editSortingAndGroupingCommand ??= new ActionCommand(EditSortingAndGrouping, CanEditSortingAndGrouping);

    private void EditSortingAndGrouping()
    {
      using (var vm = new EditSortingAndGroupingViewModel(Schedule.ActiveLayout))
      {
        ViewFactory.Instance.ShowDialog(vm);
      }

      OnRefreshLayout(false);
    }

    private bool CanEditSortingAndGrouping()
    {
      return !_isBusy && Schedule.ActiveLayout != null;
    }

    #endregion

    #region Edit Filters

    public ICommand EditFiltersCommand => _editFiltersCommand ??= new ActionCommand(EditFilters, CanEditFilters);

    private void EditFilters()
    {
      using (var vm = new EditFiltersViewModel(Schedule, Schedule.ActiveLayout))
      {
        ViewFactory.Instance.ShowDialog(vm);
      }

      OnRefreshLayout(false);
    }

    private bool CanEditFilters()
    {
      return !_isBusy && Schedule.ActiveLayout != null;
    }

    #endregion

    #region Auto Arrange PERT

    public ICommand AutoArrangePERTCommand => _autoArrangePERTCommand ??= new ActionCommand(AutoArrangePERT, CanAutoArrangePERT);

    private void AutoArrangePERT()
    {
      PERTCanvasHelper.ResetActivityPositions(Schedule);
      OnRefreshLayout(false);
    }

    private bool CanAutoArrangePERT()
    {
      return !_isBusy && ActiveLayout.LayoutType == LayoutType.PERT;
    }

    #endregion

    #endregion

    #region Controlling

    #region Edit Baselines

    public ICommand EditBaselinesCommand => _editBaselinesCommand ??= new ActionCommand(EditBaselines, CanEditBaselines);

    private void EditBaselines()
    {
      using var vm = new EditBaselinesViewModel(this.Schedule);
      ViewFactory.Instance.ShowDialog(vm);
    }

    private bool CanEditBaselines()
    {
      return !_isBusy;
    }

    #endregion

    #region Edit Fragnets

    public ICommand EditFragnetsCommand => _editFragnetsCommand ??= new ActionCommand(EditFragnets, CanEditFragnets);

    private void EditFragnets()
    {
      using var vm = new EditFragnetsViewModel(Schedule);
      ViewFactory.Instance.ShowDialog(vm);
    }

    private bool CanEditFragnets()
    {
      return !_isBusy;
    }

    #endregion

    #region Set Fragnets Visible

    public ICommand SetFragnetVisibleCommand => _setFragnetVisibleCommand ??= new ActionCommand(SetFragnetVisible, CanSetFragnetVisible);

    private void SetFragnetVisible()
    {
      UserNotificationService.Instance.Question(NASResources.MessageChangingFragnetVisibilityWarning, () =>
      {
        Calculate(Schedule.DataDate);
      });

      InstantHelpManager.Instance.SetHelpTopic(HelpTopic.Fragnets);
    }

    private bool CanSetFragnetVisible()
    {
      return !_isBusy;
    }

    #endregion

    #region Compare with Baseline

    public ICommand CompareWithBaselineCommand => _compareWithBaselineCommand ??= new ActionCommand(CompareWithBaseline, CanCompareWithBaseline);

    private void CompareWithBaseline()
    {
      using var vm = new SelectBaselineViewModel(Schedule.Baselines);
      if (ViewFactory.Instance.ShowDialog(vm) == true && vm.SelectedBaseline != null)
      {
        var baseline = vm.SelectedBaseline;
        string headline = string.Format(NASResources.BaselinesCompared, baseline.Name, baseline.ModifiedDate.HasValue ? baseline.ModifiedDate.Value.ToShortDateString() : "");
        var p1 = baseline;
        var p2 = Schedule;
        var vm2 = new CompareResultsViewModel(new ComparisonData(p1, p2) { Headline = headline });
        ViewFactory.Instance.ShowDialog(vm2);
      }
    }

    private bool CanCompareWithBaseline()
    {
      return !_isBusy;
    }

    #endregion

    #region Compare with Distortions

    public ICommand CompareWithDistortionsCommand => _compareWithDistortionsCommand ??= new ActionCommand(CompareWithDistortions, CanCompareWithDistortions);

    private void CompareWithDistortions()
    {
      InstantHelpManager.Instance.SetHelpTopic(HelpTopic.Compare);
      string headline = NASResources.DistortionsCompared;
      var p1 = Schedule.Clone();
      var p2 = Schedule.Clone();
      p1.Fragnets.ToList().ForEach(x => x.IsVisible = true);
      p2.Fragnets.ToList().ForEach(x => x.IsVisible = true);
      foreach (var a in p1.Activities)
      {
        a.Distortions?.Clear();
      }

      var d = Schedule.DataDate;
      var s1 = new Scheduler(p1);
      s1.Calculate(d);
      var s2 = new Scheduler(p2);
      s2.Calculate(d);
      using var vm = new CompareResultsViewModel(new ComparisonData(p1, p2) { Headline = headline });
      ViewFactory.Instance.ShowDialog(vm);
    }

    private bool CanCompareWithDistortions()
    {
      return !_isBusy;
    }

    #endregion

    #region Compare

    public ICommand CompareCommand => _compareCommand ??= new ActionCommand(Compare, CanCompare);

    private void Compare()
    {
      using var vm = new CompareSchedulesViewModel(Schedule);
      if (ViewFactory.Instance.ShowDialog(vm) == true)
      {
        var data = vm.Compare();
        using var vm2 = new CompareResultsViewModel(data);
        ViewFactory.Instance.ShowDialog(vm2);
      }
    }

    private bool CanCompare()
    {
      return !_isBusy && Schedule.Fragnets.Count > 0;
    }

    #endregion

    #region Edit Distortions

    public ICommand EditDistortionsCommand => _editDistortionsCommand ??= new ActionCommand(EditDistortions, CanEditDistortions);

    private void EditDistortions()
    {
      using var vm = new DistortionsViewModel(CurrentActivity.Activity);
      ViewFactory.Instance.ShowDialog(vm);
    }

    private bool CanEditDistortions()
    {
      return CurrentActivity != null && CurrentActivity.Activity.ActivityType == ActivityType.Activity;
    }

    #endregion

    #endregion

    #region Context

    #region Increase Duration

    public ICommand IncreaseDurationCommand => _increaseDurationCommand ??= new ActionCommand(IncreaseDuration, CanIncreaseDuration);

    private void IncreaseDuration()
    {
      InstantHelpManager.Instance.SetHelpTopic(HelpTopic.Activity);
      CurrentActivity.Activity.OriginalDuration++;
    }

    private bool CanIncreaseDuration()
    {
      return !_isBusy
             && CurrentActivity != null
             && CurrentActivity.Activity.ActivityType == ActivityType.Activity
             && !CurrentActivity.Activity.IsFinished;
    }

    #endregion

    #region Decrease Duration

    public ICommand DecreaseDurationCommand => _decreaseDurationCommand ??= new ActionCommand(DecreaseDuration, CanDecreaseDuration);

    private void DecreaseDuration()
    {
      InstantHelpManager.Instance.SetHelpTopic(HelpTopic.Activity);
      CurrentActivity.Activity.OriginalDuration--;
    }

    private bool CanDecreaseDuration()
    {
      return !_isBusy
             && CurrentActivity != null
             && CurrentActivity.Activity.ActivityType == ActivityType.Activity
             && !CurrentActivity.Activity.IsFinished
             && CurrentActivity.Activity.OriginalDuration > 1;
    }

    #endregion

    #region Split Activity

    public ICommand SplitActivityCommand => _splitActivityCommand ??= new ActionCommand(SplitActivity, CanSplitActivity);

    private void SplitActivity()
    {
      InstantHelpManager.Instance.SetHelpTopic(HelpTopic.Start);
      var newActivity = Schedule.SplitActivity(CurrentActivity.Activity);
      Activities.Add(new ActivityViewModel(Schedule, newActivity));
    }

    private bool CanSplitActivity()
    {
      return !_isBusy && CurrentActivity != null && Schedule.CanSplit(CurrentActivity.Activity);
    }

    #endregion

    #region Combine Activities

    public ICommand CombineActivitiesCommand => _combineActivitiesCommand ??= new ActionCommand(CombineActivities, CanCombineActivities);

    private void CombineActivities()
    {
      InstantHelpManager.Instance.SetHelpTopic(HelpTopic.Activity);
      Schedule.CombineActivities(CurrentActivity.Activity);
    }

    private bool CanCombineActivities()
    {
      return !_isBusy && CurrentActivity != null && Schedule.CanCombineActivity(CurrentActivity.Activity);
    }

    #endregion

    #region Change Into Milestone

    public ICommand ChangeIntoMilestoneCommand => _changeIntoMilestoneCommand ??= new ActionCommand(ChangeIntoMilestone, CanChangeIntoMilestone);

    private void ChangeIntoMilestone()
    {
      InstantHelpManager.Instance.SetHelpTopic(HelpTopic.Activity);
      UserNotificationService.Instance.Question(NASResources.MessageChangeIntoMilestone, () =>
      {
        var activityVM = CurrentActivity;
        var activity = CurrentActivity.Activity;
        var newMilestone = Schedule.ChangeToMilestone(activity);
        var newMilestoneVM = new ActivityViewModel(Schedule, newMilestone);
        Activities.Remove(activityVM);
        Activities.Add(newMilestoneVM);
        OnActivityDeleted(activityVM);
        OnActivityAdded(newMilestoneVM);
        CurrentActivity = newMilestoneVM;
      });
    }

    private bool CanChangeIntoMilestone()
    {
      return !_isBusy && _currentActivity != null && _currentActivity.Activity.ActivityType == ActivityType.Activity;
    }

    #endregion

    #region Change Into Activity

    public ICommand ChangeIntoActivityCommand => _changeIntoActivityCommand ??= new ActionCommand(ChangeIntoActivity, CanChangeIntoActivity);

    private void ChangeIntoActivity()
    {
      InstantHelpManager.Instance.SetHelpTopic(HelpTopic.Activity);
      UserNotificationService.Instance.Question(NASResources.MessageChangeIntoActivity, () =>
      {
        var milestoneVM = CurrentActivity;
        var milestone = CurrentActivity.Activity as Milestone;
        var newActivity = Schedule.ChangeToActivity(milestone);
        var newActivityVM = new ActivityViewModel(Schedule, newActivity);
        Activities.Remove(milestoneVM);
        Activities.Add(newActivityVM);
        OnActivityDeleted(milestoneVM);
        OnActivityAdded(newActivityVM);
        CurrentActivity = newActivityVM;
      });
    }

    private bool CanChangeIntoActivity()
    {
      return !_isBusy && CurrentActivity != null && CurrentActivity.IsMilestone;
    }

    #endregion

    #endregion

    #endregion

    #region Gantt Chart

    #region Edit Columns

    public ICommand EditColumnsCommand => _editColumnsCommand ??= new ActionCommand(EditColumns, CanEditColumns);

    private void EditColumns()
    {
      using var vm = new EditColumnsViewModel(ActiveLayout.Layout);
      if (ViewFactory.Instance.ShowDialog(vm) == true)
      {
        var visibleColumns = new List<ActivityColumn>(ActiveLayout.Layout.ActivityColumns);

        // First remove all columns that are not checked anymore
        foreach (var column in visibleColumns)
        {
          foreach (var c in vm.EditColumns)
          {
            if (!c.IsVisible && c.Property == column.Property && ActiveLayout is Layout layout)
            {
              layout.ActivityColumns.Remove(column);
            }
          }
        }

        foreach (var column in vm.EditColumns)
        {
          if (column.IsVisible)
          {
            var activityColumn = visibleColumns.FirstOrDefault(x => x.Property == column.Property);
            activityColumn ??= new ActivityColumn(column.Property);
            activityColumn.Order = vm.EditColumns.IndexOf(column);
          }
        }

        ColumnsChanged?.Invoke(this, EventArgs.Empty);
      }
    }

    private bool CanEditColumns()
    {
      return !_isBusy && Schedule.ActiveLayout != null;
    }

    #endregion

    #region Resources

    public ObservableCollection<ResourceViewModel> Resources { get; private set; } = [];

    private void RefreshResources()
    {
      Resources.Clear();
      if (ActiveLayout != null)
      {
        foreach (var resource in ActiveLayout.Layout.VisibleResources)
        {
          Resources.Add(new ResourceViewModel(resource, this, CloseResourceCommand));
        }
      }
    }

    #endregion

    #endregion

    #region Private Members

    private void OnActivityAdded(ActivityViewModel a)
    {
      ActivityAdded?.Invoke(this, new ItemEventArgs<ActivityViewModel>(a));
    }

    private void OnActivityDeleted(ActivityViewModel a)
    {
      ActivityDeleted?.Invoke(this, new ItemEventArgs<ActivityViewModel>(a));
    }

    private void OnActivityChanged(ActivityViewModel a)
    {
      ActivityChanged?.Invoke(this, new ItemEventArgs<ActivityViewModel>(a));
    }

    private void OnRelationshipAdded(RelationshipViewModel r)
    {
      RelationshipAdded?.Invoke(this, new ItemEventArgs<RelationshipViewModel>(r));
    }

    private void OnRelationshipDeleted(RelationshipViewModel r)
    {
      RelationshipDeleted?.Invoke(this, new ItemEventArgs<RelationshipViewModel>(r));
    }

    private void OnRelationshipChanged(RelationshipViewModel r)
    {
      RelationshipChanged?.Invoke(this, new ItemEventArgs<RelationshipViewModel>(r));
    }

    private void OnRefreshLayout(bool typeChanged)
    {
      if (typeChanged)
      {
        LayoutTypeChanged?.Invoke(this, EventArgs.Empty);
        OnPropertyChanged(nameof(ShowGanttControls));
        OnPropertyChanged(nameof(ShowPERTControls));
      }
      else
      {
        RefreshLayout?.Invoke(this, new ItemEventArgs<Layout>(ActiveLayout.Layout));
      }
    }

    private IVisibleLayoutViewModel AddLayoutVM(Layout layout, bool makeCurrent)
    {
      var vm = LayoutVMFactory.CreateVM(Schedule, layout);

      Layouts.Add(vm);

      if (makeCurrent)
      {
        ActiveLayout = vm;
      }

      return vm;
    }

    private CollectionView SortFilterAndGroup(ObservableCollection<ActivityViewModel> activities)
    {
      var cvs = new CollectionViewSource
      {
        Source = activities
      };
      var myView = (CollectionView)cvs.View;
      if (myView != null)
      {
        // Filtering
        myView.Filter = new Predicate<object>(Contains);
        // Sorting
        myView.SortDescriptions.Clear();
        foreach (var sortDefinition in Schedule.ActiveLayout.SortingDefinitions)
        {
          if (sortDefinition.Direction == SortDirection.Descending)
          {
            myView.SortDescriptions.Add(new SortDescription(sortDefinition.Property.ToString(), ListSortDirection.Descending));
          }
          else
          {
            myView.SortDescriptions.Add(new SortDescription(sortDefinition.Property.ToString(), ListSortDirection.Ascending));
          }
        }
        // Grouping
        myView.GroupDescriptions.Clear();
        foreach (var groupDefinition in Schedule.ActiveLayout.GroupingDefinitions)
        {
          myView.GroupDescriptions.Add(new PropertyGroupDescription(groupDefinition.Property.ToString()));
        }
      }
      return myView;
    }

    private bool Contains(object obj)
    {
      if (obj is not Activity activity)
      {
        return false;
      }

      if (activity.Fragnet != null && !activity.Fragnet.IsVisible)
      {
        return false;
      }

      if (Schedule.ActiveLayout.FilterDefinitions.Count == 0)
      {
        return true;
      }

      foreach (var filter in Schedule.ActiveLayout.FilterDefinitions)
      {
        bool isTrue = filter.Compare(activity);
        if (Schedule.ActiveLayout.FilterCombination == FilterCombinationType.Or && isTrue)
        {
          return true;
        }

        if (Schedule.ActiveLayout.FilterCombination == FilterCombinationType.And && !isTrue)
        {
          return false;
        }
      }
      return Schedule.ActiveLayout.FilterCombination == FilterCombinationType.And;
    }

    #endregion

    #region IDisposable

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);

      if (disposing)
      {
        Schedule.ActivityAdded -= Schedule_ActivityAdded;
        Schedule.ActivityRemoved -= Schedule_ActivityRemoved;

        if (_activeLayout != null)
        {
          _activeLayout.Dispose();
          _activeLayout = null;
        }
      }
    }

    #endregion
  }
}
