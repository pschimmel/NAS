using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using ES.Tools.Core.MVVM;
using NAS.Model.Base;
using NAS.Model.Entities;
using NAS.Model.Enums;
using NAS.Model.Scheduler;
using NAS.Resources;
using NAS.ViewModel.Base;
using NAS.ViewModel.Helpers;

namespace NAS.ViewModel
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
    private IVisibleLayoutViewModel _currentLayout;

    #endregion

    #region Constructor

    public ScheduleViewModel(Schedule schedule)
    {
      if (schedule == null)
      {
        throw new ArgumentNullException(nameof(schedule));
      }

      Layouts = new ObservableCollection<IVisibleLayoutViewModel>();
      foreach (var layout in Schedule.Layouts)
      {
        _ = AddLayoutVM(layout, layout == Schedule.CurrentLayout);
      }

      foreach (var activity in schedule.Activities)
      {
        Activities.Add(new ActivityViewModel(activity));
      }

      foreach (var relationship in schedule.Relationships)
      {
        Relationships.Add(new RelationshipViewModel(relationship));
      }

      CalculateCommand = new ActionCommand(CalculateCommandExecute, param => CalculateCommandCanExecute);
      SchedulingSettingsCommand = new ActionCommand(SchedulingSettingsCommandExecute, () => SchedulingSettingsCommandCanExecute);
      AddActivityCommand = new ActionCommand(AddActivityCommandExecute, param => AddActivityCommandCanExecute);
      AddMilestoneCommand = new ActionCommand(AddMilestoneCommandExecute, param => AddMilestoneCommandCanExecute);
      RemoveActivityCommand = new ActionCommand(RemoveActivityCommandExecute, () => RemoveActivityCommandCanExecute);
      EditActivityCommand = new ActionCommand(EditActivityCommandExecute, () => EditActivityCommandCanExecute);
      AddRelationshipCommand = new ActionCommand(AddRelationshipCommandExecute, param => AddRelationshipCommandCanExecute);
      RemoveRelationshipCommand = new ActionCommand(RemoveRelationshipCommandExecute, () => RemoveRelationshipCommandCanExecute);
      EditRelationshipCommand = new ActionCommand(EditRelationshipCommandExecute, () => EditRelationshipCommandCanExecute);
      EditLogicCommand = new ActionCommand(EditLogicCommandExecute, () => EditLogicCommandCanExecute);
      PropertiesCommand = new ActionCommand(PropertiesCommandExecute, () => PropertiesCommandCanExecute);
      EditWBSCommand = new ActionCommand(EditWBSCommandExecute, () => EditWBSCommandCanExecute);
      EditResourcesCommand = new ActionCommand(EditResourcesCommandExecute, () => EditResourcesCommandCanExecute);
      EditCustomAttributesCommand = new ActionCommand(EditCustomAttributesCommandExecute, () => EditCustomAttributesCommandCanExecute);
      EditCalendarsCommand = new ActionCommand(EditCalendarsCommandExecute, () => EditCalendarsCommandCanExecute);
      ShowAsGanttCommand = new ActionCommand(ShowAsGanttCommandExecute, () => ShowAsGanttCommandCanExecute);
      ShowAsPertCommand = new ActionCommand(ShowAsPertCommandExecute, () => ShowAsPertCommandCanExecute);
      ZoomCommand = new ActionCommand(ZoomCommandExecute, param => ZoomCommandCanExecute);
      ShowRelationshipsCommand = new ActionCommand(ShowRelationshipsCommandExecute, () => ShowRelationshipsCommandCanExecute);
      ShowFloatCommand = new ActionCommand(ShowFloatCommandExecute, () => ShowFloatCommandCanExecute);
      ShowResourceCommand = new ActionCommand(ShowResourceCommandExecute, () => ShowResourceCommandCanExecute);
      CloseResourceCommand = new ActionCommand(CloseResourceCommandExecute, CloseResourceCommandCanExecute);
      AddLayoutCommand = new ActionCommand(AddLayoutCommandExecute);
      RemoveLayoutCommand = new ActionCommand(RemoveLayoutCommandExecute, () => RemoveLayoutCommandCanExecute);
      EditLayoutCommand = new ActionCommand(EditLayoutCommandExecute, () => EditLayoutCommandCanExecute);
      CopyLayoutCommand = new ActionCommand(CopyLayoutCommandExecute, () => CopyLayoutCommandCanExecute);
      EditSortingAndGroupingCommand = new ActionCommand(EditSortingAndGroupingCommandExecute, () => EditSortingAndGroupingCommandCanExecute);
      EditFiltersCommand = new ActionCommand(EditFiltersCommandExecute, () => EditFiltersCommandCanExecute);
      AutoArrangePERTCommand = new ActionCommand(AutoArrangePERTCommandExecute, () => AutoArrangePERTCommandCanExecute);
      EditBaselinesCommand = new ActionCommand(EditBaselinesCommandExecute, () => EditBaselinesCommandCanExecute);
      EditFragnetsCommand = new ActionCommand(EditFragnetsCommandExecute, () => EditFragnetsCommandCanExecute);
      SetFragnetVisibleCommand = new ActionCommand(SetFragnetVisibleCommandExecute, () => SetFragnetVisibleCommandCanExecute);
      CompareWithBaselineCommand = new ActionCommand(CompareWithBaselineCommandExecute, () => CompareWithBaselineCommandCanExecute);
      CompareWithDistortionsCommand = new ActionCommand(CompareWithDistortionsCommandExecute, () => CompareWithDistortionsCommandCanExecute);
      CompareCommand = new ActionCommand(CompareCommandExecute, () => CompareCommandCanExecute);
      EditDistortionsCommand = new ActionCommand(EditDistortionsCommandExecute, () => EditDistortionsCommandCanExecute);
      IncreaseDurationCommand = new ActionCommand(IncreaseDurationCommandExecute, () => IncreaseDurationCommandCanExecute);
      DecreaseDurationCommand = new ActionCommand(DecreaseDurationCommandExecute, () => DecreaseDurationCommandCanExecute);
      SplitActivityCommand = new ActionCommand(SplitActivityCommandExecute, () => SplitActivityCommandCanExecute);
      CombineActivitiesCommand = new ActionCommand(CombineActivitiesCommandExecute, () => CombineActivitiesCommandCanExecute);
      ChangeIntoMilestoneCommand = new ActionCommand(ChangeIntoMilestoneCommandExecute, () => ChangeIntoMilestoneCommandCanExecute);
      ChangeIntoActivityCommand = new ActionCommand(ChangeIntoActivityCommandExecute, () => ChangeIntoActivityCommandCanExecute);
      EditColumnsCommand = new ActionCommand(EditColumnsCommandExecute, () => EditColumnsCommandCanExecute);

      Schedule.ActivityAdded += Schedule_ActivityAdded;
      Schedule.ActivityRemoved += Schedule_ActivityRemoved;
      RefreshResources();
    }

    private void Schedule_ActivityRemoved(object sender, ItemEventArgs<Activity> e)
    {
      Activities.Add(new ActivityViewModel(e.Item));
    }

    private void Schedule_ActivityAdded(object sender, ItemEventArgs<Activity> e)
    {
      var deletedActivity = Activities.FirstOrDefault(x => x.Activity == e.Item);
      if (deletedActivity != null)
      {
        _ = Activities.Remove(deletedActivity);
      }
    }

    #endregion

    #region Public Properties

    public override HelpTopic HelpTopicKey => HelpTopic.New;

    /// <summary>
    /// Gets the schedule.
    /// </summary>
    public Schedule Schedule { get; }

    /// <summary>
    /// Gets the layouts.
    /// </summary>
    public ObservableCollection<IVisibleLayoutViewModel> Layouts { get; }

    public ObservableCollection<ActivityViewModel> Activities { get; } = new ObservableCollection<ActivityViewModel>();

    public ObservableCollection<RelationshipViewModel> Relationships { get; } = new ObservableCollection<RelationshipViewModel>();

#pragma warning disable CA1822 // Mark members as static

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

#pragma warning restore CA1822 // Mark members as static

    /// <summary>
    /// Gets the WBS items.
    /// </summary>
    public List<WBSItem> WBSItems => new(Schedule.GetWBSItems());

    /// <summary>
    /// Gets a value indicating whether this instance is loading.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is loading; otherwise, <c>false</c>.
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
    ///   <c>true</c> if this instance is saving; otherwise, <c>false</c>.
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

    public IVisibleLayoutViewModel CurrentLayout
    {
      get => _currentLayout;
      set
      {
        if (_currentLayout != value)
        {
          bool typeChanged = !Equals(_currentLayout?.LayoutType, value?.LayoutType);
          _currentLayout = value;
          Schedule.CurrentLayout = value?.Layout;
          OnPropertyChanged(nameof(CurrentLayout));
          OnRefreshLayout(typeChanged);
        }
        foreach (var l in Layouts)
        {
          l.IsVisible = l == value;
        }
      }
    }

    public bool ShowPERTControls => Schedule != null && _currentLayout.LayoutType == LayoutType.PERT;

    public bool ShowGanttControls => Schedule != null && _currentLayout.LayoutType == LayoutType.Gantt;

    #endregion

    #region Public Methods

    #endregion

    #region Commands

    #region Start

    #region Calculate

    public ICommand CalculateCommand { get; }

    private void CalculateCommandExecute(object param)
    {
      InstantHelpManager.Instance.SetHelpTopic(HelpTopic.Calculate);
      var scheduler = new Scheduler(Schedule, SchedulingSettingsHelper.LoadSchedulingSettings(Schedule.SchedulingSettings), () =>
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
      DateTime dataDate;
      if (param == null)
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
      else
      {
        dataDate = (DateTime)param;
      }

      scheduler.Calculate(dataDate);
    }

    private bool CalculateCommandCanExecute => !_isBusy && Schedule != null;

    #endregion

    #region Scheduling Settings

    public ICommand SchedulingSettingsCommand { get; }

    private void SchedulingSettingsCommandExecute()
    {
      var vm = new SchedulingSettingsViewModel(Schedule.SchedulingSettings);
      if (ViewFactory.Instance.ShowDialog(vm) == true)
      {
        Schedule.SchedulingSettings = vm.GetScheduleSettingsString();
      }
    }

    private bool SchedulingSettingsCommandCanExecute => Schedule != null && !_isBusy;

    #endregion

    #region Add Activity

    public ICommand AddActivityCommand { get; }

    private void AddActivityCommandExecute(object param)
    {
      InstantHelpManager.Instance.SetHelpTopic(HelpTopic.Activity);
      var activity = Schedule.AddActivity(param?.ToString() == "Fixed");
      var newActivityVM = new ActivityViewModel(activity);
      Activities.Add(newActivityVM);
      OnActivityAdded(newActivityVM);
      CurrentActivity = newActivityVM;
    }

    private bool AddActivityCommandCanExecute => !_isBusy && Schedule != null;

    #endregion

    #region Add Milestone

    public ICommand AddMilestoneCommand { get; }

    private void AddMilestoneCommandExecute(object param)
    {
      InstantHelpManager.Instance.SetHelpTopic(HelpTopic.Activity);
      var milestone = Schedule.AddMilestone(param?.ToString() == "Fixed");
      var newActivityVM = new ActivityViewModel(milestone);
      Activities.Add(newActivityVM);
      CurrentActivity = newActivityVM;
      OnActivityAdded(newActivityVM);
    }

    private bool AddMilestoneCommandCanExecute => !_isBusy && Schedule != null;

    #endregion

    #region Remove Activity

    public ICommand RemoveActivityCommand { get; }

    private void RemoveActivityCommandExecute()
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
        _ = Activities.Remove(activityVM);
        OnActivityDeleted(activityVM);
        if (CurrentLayout.LayoutType == LayoutType.PERT)
        {
          var view = ES.Tools.Core.MVVM.ViewModelExtensions.GetView(Activities);
          CurrentActivity = view.CurrentItem as ActivityViewModel;
        }
      });
    }

    private bool RemoveActivityCommandCanExecute => !_isBusy && Schedule != null && CurrentActivity != null;

    #endregion

    #region Edit Activity

    public ICommand EditActivityCommand { get; }

    private void EditActivityCommandExecute()
    {
      var vm = CurrentActivity;
      if (ViewFactory.Instance.ShowDialog(vm) == true)
      {
        _ = SortFilterAndGroup(Activities);
        OnPropertyChanged(nameof(Activities));
        OnActivityChanged(CurrentActivity);
      }
    }

    private bool EditActivityCommandCanExecute => !_isBusy && Schedule != null && CurrentActivity != null;

    #endregion

    #region Add Relationship

    public ICommand AddRelationshipCommand { get; }

    private void AddRelationshipCommandExecute(object param)
    {
      InstantHelpManager.Instance.SetHelpTopic(HelpTopic.Relationship);
      Relationship relationship;

      if (param is AddRelationshipInfo relationshipInfo)
      {
        relationship = Schedule.AddRelationship(relationshipInfo.Activity1, relationshipInfo.Activity2, relationshipInfo.RelationshipType);
      }
      else
      {
        relationship = new Relationship();
        var vm = new RelationshipViewModel(relationship);
        if (ViewFactory.Instance.ShowDialog(vm) != true)
        {
          return;
        }

        relationship = Schedule.AddRelationship(vm.Activity1, vm.Activity2, vm.SelectedRelationshipType);
        relationship.Lag = vm.Lag;
      }

      if (!Scheduler.CheckForLoops(Activities.Select(x => x.Activity).ToList(), Schedule.Relationships.ToList()))
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

    private bool AddRelationshipCommandCanExecute => !_isBusy && Schedule != null;

    #endregion

    #region Remove Relationship

    public ICommand RemoveRelationshipCommand { get; }

    private void RemoveRelationshipCommandExecute()
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

    private bool RemoveRelationshipCommandCanExecute => !_isBusy && Schedule != null && CurrentRelationship != null;

    #endregion

    #region Edit Relationship

    public ICommand EditRelationshipCommand { get; }

    private void EditRelationshipCommandExecute()
    {
      var relationship = CurrentRelationship.Relationship;
      using var vm = new EditRelationshipViewModel(relationship, Schedule);

      if (ViewFactory.Instance.ShowDialog(vm) == true)
      {
        if (vm.Validate())
        {
          vm.Apply();
          CurrentRelationship.Relationship = relationship;
          OnRelationshipChanged(CurrentRelationship);
        }
      }
    }

    private bool EditRelationshipCommandCanExecute => CurrentRelationship != null && !_isBusy;

    #endregion

    #region Edit Logic

    public ICommand EditLogicCommand { get; }

    private void EditLogicCommandExecute()
    {
      using var vm = new EditLogicViewModel(CurrentActivity.Activity);
      _ = ViewFactory.Instance.ShowDialog(vm);
    }

    private bool EditLogicCommandCanExecute => CurrentActivity != null;

    #endregion

    #region Properties

    public ICommand PropertiesCommand { get; }

    private void PropertiesCommandExecute()
    {
      using var vm = new SchedulePropertiesViewModel(Schedule);
      _ = ViewFactory.Instance.ShowDialog(vm);
    }

    private bool PropertiesCommandCanExecute => !_isBusy && Schedule != null;

    #endregion

    #region Edit WBS

    public ICommand EditWBSCommand { get; }

    private void EditWBSCommandExecute()
    {
      using var vm = new WBSViewModel(Schedule);
      _ = ViewFactory.Instance.ShowDialog(vm);
    }

    private bool EditWBSCommandCanExecute => !_isBusy && Schedule != null;

    #endregion

    #region Edit Calendars

    public ICommand EditCalendarsCommand { get; }

    private void EditCalendarsCommandExecute()
    {
      using var vm = new CalendarsViewModel(Schedule);
      _ = ViewFactory.Instance.ShowDialog(vm);
    }

    private bool EditCalendarsCommandCanExecute => !_isBusy && Schedule != null;

    #endregion

    #region Edit Resources

    public ICommand EditResourcesCommand { get; }

    private void EditResourcesCommandExecute()
    {
      using var vm = new ResourcesViewModel(Schedule);
      _ = ViewFactory.Instance.ShowDialog(vm);
    }

    private bool EditResourcesCommandCanExecute => !_isBusy && Schedule != null;

    #endregion

    #region Edit Custom Attributes

    public ICommand EditCustomAttributesCommand { get; }

    private void EditCustomAttributesCommandExecute()
    {
      using var vm = new CustomAttributesViewModel(Schedule);
      _ = ViewFactory.Instance.ShowDialog(vm);
    }

    private bool EditCustomAttributesCommandCanExecute => !_isBusy && Schedule != null;

    #endregion

    #endregion

    #region View

    #region Show as Gantt

    public ICommand ShowAsGanttCommand { get; }

    private void ShowAsGanttCommandExecute()
    {
      InstantHelpManager.Instance.SetHelpTopic(HelpTopic.View);
      CurrentLayout.Layout.LayoutType = LayoutType.Gantt;
      ChangeLayout(CurrentLayout.Layout);
      OnRefreshLayout(true);
    }

    private bool ShowAsGanttCommandCanExecute => !_isBusy && _currentLayout.LayoutType != LayoutType.Gantt;

    #endregion

    #region Show as PERT

    public ICommand ShowAsPertCommand { get; }

    private void ShowAsPertCommandExecute()
    {
      InstantHelpManager.Instance.SetHelpTopic(HelpTopic.View);
      CurrentLayout.Layout.LayoutType = LayoutType.PERT;
      ChangeLayout(CurrentLayout.Layout);
      OnRefreshLayout(true);
      Schedule.CurrentLayout.VisibleResources.Clear();
    }

    private bool ShowAsPertCommandCanExecute => !_isBusy && CurrentLayout.LayoutType != LayoutType.PERT;

    #endregion

    #region Zoom

    public ICommand ZoomCommand { get; }

    private void ZoomCommandExecute(object param)
    {
      InstantHelpManager.Instance.SetHelpTopic(HelpTopic.Relationship);
      var style = System.Globalization.NumberStyles.Number | System.Globalization.NumberStyles.Float;
      var culture = System.Globalization.CultureInfo.CreateSpecificCulture("en-GB");
      if (double.TryParse(param.ToString(), style, culture, out double z))
      {
        Zoom = z;
      }
    }

    private bool ZoomCommandCanExecute => !_isBusy && Schedule != null;

    #endregion

    #region Show Relationships

    public ICommand ShowRelationshipsCommand { get; }

    private void ShowRelationshipsCommandExecute()
    {
      InstantHelpManager.Instance.SetHelpTopic(HelpTopic.View);
      OnRefreshLayout(false);
    }

    private bool ShowRelationshipsCommandCanExecute => !_isBusy && Schedule != null;

    #endregion

    #region Show Float

    public ICommand ShowFloatCommand { get; }

    private void ShowFloatCommandExecute()
    {
      InstantHelpManager.Instance.SetHelpTopic(HelpTopic.View);
      OnRefreshLayout(false);
    }

    private bool ShowFloatCommandCanExecute => !_isBusy && Schedule != null;

    #endregion

    #region Show Resource

    public ICommand ShowResourceCommand { get; }

    private void ShowResourceCommandExecute()
    {
      using var vm = new SelectResourceViewModel(Schedule);
      if (ViewFactory.Instance.ShowDialog(vm) == true && vm.SelectedResource != null)
      {
        var vr = new VisibleResource(_currentLayout.Layout, vm.SelectedResource);
        Resources.Add(new ResourceViewModel(vr, this, CloseResourceCommand));
      }
    }

    private bool ShowResourceCommandCanExecute => !_isBusy && Schedule != null && Schedule.Resources.Count > 0;

    #endregion

    #region Close Resource

    public ICommand CloseResourceCommand { get; }

    private void CloseResourceCommandExecute(object param)
    {
      if (param is ResourceViewModel resourceVM)
      {
        _ = Resources.Remove(resourceVM);
        OnRefreshLayout(false);
      }
    }

    private bool CloseResourceCommandCanExecute(object param)
    {
      return !_isBusy && Resources.Contains(param as ResourceViewModel);
    }

    #endregion

    #region Add Layout

    public ICommand AddLayoutCommand { get; }

    private void AddLayoutCommandExecute()
    {
      var item = new Layout();

      using var vm = new EditLayoutViewModel(item);
      if (!ViewFactory.Instance.ShowDialog(vm) == true)
      {
        _ = AddLayoutVM(item, true);
      }
    }

    #endregion

    #region Remove Layout

    public ICommand RemoveLayoutCommand { get; }

    private void RemoveLayoutCommandExecute()
    {
      InstantHelpManager.Instance.SetHelpTopic(HelpTopic.Layout);
      UserNotificationService.Instance.Question(NASResources.MessageDeleteLayout, () =>
      {
        var layoutToRemove = CurrentLayout;
        int idx = Layouts.IndexOf(CurrentLayout);
        CurrentLayout = idx + 1 < Layouts.Count ? Layouts[idx + 1] : Layouts[idx - 1];
        _ = Layouts.Remove(layoutToRemove);
      });
    }

    private bool RemoveLayoutCommandCanExecute => CurrentLayout != null && Schedule.Layouts.Count > 1;

    #endregion

    #region Copy Layout

    public ICommand CopyLayoutCommand { get; }

    private void CopyLayoutCommandExecute()
    {
      InstantHelpManager.Instance.SetHelpTopic(HelpTopic.Layout);
      var vm = new GetTextViewModel(NASResources.CopyLayout, NASResources.Name, CurrentLayout.Name + " (" + NASResources.Copy + ")");
      if (ViewFactory.Instance.ShowDialog(vm) == true)
      {
        var newLayout = new Layout(CurrentLayout.Layout);
        newLayout.Name = vm.Text;
        _ = AddLayoutVM(newLayout, true);
      }
    }

    private bool CopyLayoutCommandCanExecute => CurrentLayout != null;

    #endregion

    #region Edit Layout

    public ICommand EditLayoutCommand { get; }

    private void EditLayoutCommandExecute()
    {
      var layoutVM = CurrentLayout;
      var layout = layoutVM.Layout;
      var oldLayoutType = layout.LayoutType;

      using var vm = new EditLayoutViewModel(layout);
      if (ViewFactory.Instance.ShowDialog(vm) == true)
      {
        if (oldLayoutType != vm.CurrentLayout.LayoutType)
        {
          ChangeLayout(layout);
        }
      }
    }

    private bool EditLayoutCommandCanExecute => CurrentLayout != null;

    #endregion

    #region Edit Sorting and Grouping

    public ICommand EditSortingAndGroupingCommand { get; }

    private void EditSortingAndGroupingCommandExecute()
    {
      using (var vm = new SortingAndGroupingViewModel(Schedule.CurrentLayout))
      {
        _ = ViewFactory.Instance.ShowDialog(vm);
      }

      OnRefreshLayout(false);
    }

    private bool EditSortingAndGroupingCommandCanExecute => !_isBusy && Schedule != null && Schedule.CurrentLayout != null;

    #endregion

    #region Edit Filters

    public ICommand EditFiltersCommand { get; }

    private void EditFiltersCommandExecute()
    {
      using (var vm = new FilterDefinitionsViewModel(Schedule.CurrentLayout))
      {
        _ = ViewFactory.Instance.ShowDialog(vm);
      }

      OnRefreshLayout(false);
    }

    private bool EditFiltersCommandCanExecute => !_isBusy && Schedule != null && Schedule.CurrentLayout != null;

    #endregion

    #region Auto Arrange PERT

    public ICommand AutoArrangePERTCommand { get; }

    private void AutoArrangePERTCommandExecute()
    {
      PERTCanvasHelper.ResetActivityPositions(Schedule);
      OnRefreshLayout(false);
    }

    private bool AutoArrangePERTCommandCanExecute => !_isBusy && CurrentLayout.LayoutType == LayoutType.PERT;

    #endregion

    #endregion

    #region Controlling

    #region Edit Baselines

    public ICommand EditBaselinesCommand { get; }

    private void EditBaselinesCommandExecute()
    {
      using var vm = new BaselinesViewModel(this);
      _ = ViewFactory.Instance.ShowDialog(vm);
    }

    private bool EditBaselinesCommandCanExecute => !_isBusy && Schedule != null;

    #endregion

    #region Edit Fragnets

    public ICommand EditFragnetsCommand { get; }

    private void EditFragnetsCommandExecute()
    {
      using var vm = new FragnetsViewModel(Schedule);
      _ = ViewFactory.Instance.ShowDialog(vm);
    }

    private bool EditFragnetsCommandCanExecute => !_isBusy && Schedule != null;

    #endregion

    #region Set Fragnets Visible

    public ICommand SetFragnetVisibleCommand { get; }

    private void SetFragnetVisibleCommandExecute()
    {
      UserNotificationService.Instance.Question(NASResources.MessageChangingFragnetVisibilityWarning, () =>
      {
        CalculateCommandExecute(Schedule.DataDate);
      });

      InstantHelpManager.Instance.SetHelpTopic(HelpTopic.Fragnets);
    }

    private bool SetFragnetVisibleCommandCanExecute => !_isBusy && Schedule != null;

    #endregion

    #region Compare with Baseline

    public ICommand CompareWithBaselineCommand { get; }

    private void CompareWithBaselineCommandExecute()
    {
      using var vm = new SelectBaselineViewModel(Schedule.Baselines);
      if (ViewFactory.Instance.ShowDialog(vm) == true && vm.SelectedBaseline != null)
      {
        var baseline = vm.SelectedBaseline;
        string headline = string.Format(NASResources.BaselinesCompared, baseline.Name, baseline.ModifiedDate.HasValue ? baseline.ModifiedDate.Value.ToShortDateString() : "");
        var p1 = baseline;
        var p2 = Schedule;
        var vm2 = new CompareResultsViewModel(new ComparisonData(p1, p2) { Headline = headline });
        _ = ViewFactory.Instance.ShowDialog(vm2);
      }
    }

    private bool CompareWithBaselineCommandCanExecute => !_isBusy && Schedule != null;

    #endregion

    #region Compare with Distortions

    public ICommand CompareWithDistortionsCommand { get; }

    private void CompareWithDistortionsCommandExecute()
    {
      InstantHelpManager.Instance.SetHelpTopic(HelpTopic.Compare);
      string headline = NASResources.DistortionsCompared;
      var p1 = new Schedule(Schedule);
      var p2 = new Schedule(Schedule);
      p1.Fragnets.ToList().ForEach(x => x.IsVisible = true);
      p2.Fragnets.ToList().ForEach(x => x.IsVisible = true);
      foreach (var a in p1.Activities)
      {
        a.Distortions?.Clear();
      }

      var d = Schedule.DataDate;
      var s1 = new Scheduler(p1, SchedulingSettingsHelper.LoadSchedulingSettings(p1.SchedulingSettings));
      s1.Calculate(d);
      var s2 = new Scheduler(p2, SchedulingSettingsHelper.LoadSchedulingSettings(p2.SchedulingSettings));
      s2.Calculate(d);
      using var vm = new CompareResultsViewModel(new ComparisonData(p1, p2) { Headline = headline });
      _ = ViewFactory.Instance.ShowDialog(vm);
    }

    private bool CompareWithDistortionsCommandCanExecute => !_isBusy && Schedule != null;

    #endregion

    #region Compare Controlling

    public ICommand CompareCommand { get; }

    private void CompareCommandExecute()
    {
      using var vm = new CompareSchedulesViewModel(Schedule);
      if (ViewFactory.Instance.ShowDialog(vm) == true)
      {
        var data = vm.Compare();
        using var vm2 = new CompareResultsViewModel(data);
        _ = ViewFactory.Instance.ShowDialog(vm2);
      }
    }

    private bool CompareCommandCanExecute => !_isBusy && Schedule != null && Schedule.Fragnets.Count > 0;

    #endregion

    #region Edit Distortions

    public ICommand EditDistortionsCommand { get; }

    private void EditDistortionsCommandExecute()
    {
      using var vm = new DistortionsViewModel(CurrentActivity.Activity);
      _ = ViewFactory.Instance.ShowDialog(vm);
    }

    private bool EditDistortionsCommandCanExecute => CurrentActivity != null && CurrentActivity.Activity.ActivityType == ActivityType.Activity;

    #endregion

    #endregion

    #region Context

    #region Increase Duration

    public ICommand IncreaseDurationCommand { get; }

    private void IncreaseDurationCommandExecute()
    {
      InstantHelpManager.Instance.SetHelpTopic(HelpTopic.Activity);
      CurrentActivity.Activity.OriginalDuration++;
    }

    private bool IncreaseDurationCommandCanExecute => !_isBusy
                                                      && Schedule != null
                                                      && CurrentActivity != null
                                                      && CurrentActivity.Activity.ActivityType == ActivityType.Activity
                                                      && !CurrentActivity.Activity.IsFinished;

    #endregion

    #region Decrease Duration

    public ICommand DecreaseDurationCommand { get; }

    private void DecreaseDurationCommandExecute()
    {
      InstantHelpManager.Instance.SetHelpTopic(HelpTopic.Activity);
      CurrentActivity.Activity.OriginalDuration--;
    }

    private bool DecreaseDurationCommandCanExecute => !_isBusy
                                                      && Schedule != null
                                                      && CurrentActivity != null
                                                      && CurrentActivity.Activity.ActivityType == ActivityType.Activity
                                                      && !CurrentActivity.Activity.IsFinished
                                                      && CurrentActivity.Activity.OriginalDuration > 1;

    #endregion

    #region Split Activity

    public ICommand SplitActivityCommand { get; }

    private void SplitActivityCommandExecute()
    {
      InstantHelpManager.Instance.SetHelpTopic(HelpTopic.Start);
      var newActivity = CurrentActivity.Activity.SplitActivity();
      Activities.Add(new ActivityViewModel(newActivity));
    }

    private bool SplitActivityCommandCanExecute => !_isBusy && CurrentActivity != null && CurrentActivity.Activity.CanSplit();

    #endregion

    #region Combine Activities

    public ICommand CombineActivitiesCommand { get; }

    private void CombineActivitiesCommandExecute()
    {
      InstantHelpManager.Instance.SetHelpTopic(HelpTopic.Activity);
      CurrentActivity.Activity.CombineActivities();
    }

    private bool CombineActivitiesCommandCanExecute => !_isBusy && Schedule != null && CurrentActivity != null && CurrentActivity.Activity.CanCombineActivity();

    #endregion

    #region Change Into Milestone

    public ICommand ChangeIntoMilestoneCommand { get; }

    private void ChangeIntoMilestoneCommandExecute()
    {
      InstantHelpManager.Instance.SetHelpTopic(HelpTopic.Activity);
      UserNotificationService.Instance.Question(NASResources.MessageChangeIntoMilestone, () =>
      {
        var activityVM = CurrentActivity;
        var activity = CurrentActivity.Activity;
        var newMilestone = activity.ChangeToMilestone();
        var newMilestoneVM = new ActivityViewModel(newMilestone);
        _ = Activities.Remove(activityVM);
        Activities.Add(newMilestoneVM);
        OnActivityDeleted(activityVM);
        OnActivityAdded(newMilestoneVM);
        CurrentActivity = newMilestoneVM;
      });
    }

    private bool ChangeIntoMilestoneCommandCanExecute => !_isBusy && Schedule != null && _currentActivity != null && _currentActivity.Activity.ActivityType == ActivityType.Activity;

    #endregion

    #region Change Into Activity

    public ICommand ChangeIntoActivityCommand { get; }

    private void ChangeIntoActivityCommandExecute()
    {
      InstantHelpManager.Instance.SetHelpTopic(HelpTopic.Activity);
      UserNotificationService.Instance.Question(NASResources.MessageChangeIntoActivity, () =>
      {
        var milestoneVM = CurrentActivity;
        var milestone = CurrentActivity.Activity as Milestone;
        var newActivity = milestone.ChangeToActivity();
        var newActivityVM = new ActivityViewModel(newActivity);
        _ = Activities.Remove(milestoneVM);
        Activities.Add(newActivityVM);
        OnActivityDeleted(milestoneVM);
        OnActivityAdded(newActivityVM);
        CurrentActivity = newActivityVM;
      });
    }

    private bool ChangeIntoActivityCommandCanExecute => !_isBusy && Schedule != null && CurrentActivity != null && CurrentActivity.IsMilestone;

    #endregion

    #endregion

    #endregion

    #region Gantt Chart

    #region Edit Columns

    public ICommand EditColumnsCommand { get; }

    private void EditColumnsCommandExecute()
    {
      using var vm = new EditColumnsViewModel(CurrentLayout.Layout);
      if (ViewFactory.Instance.ShowDialog(vm) == true)
      {
        var visibleColumns = new List<ActivityColumn>(CurrentLayout.Layout.ActivityColumns);

        // First remove all columns that are not checked anymore
        foreach (var column in visibleColumns)
        {
          foreach (var c in vm.EditColumns)
          {
            if (!c.IsVisible && c.Property == column.Property && CurrentLayout is Layout layout)
            {
              _ = layout.ActivityColumns.Remove(column);
            }
          }
        }

        foreach (var c in vm.EditColumns)
        {
          if (c.IsVisible)
          {
            var activityColumn = visibleColumns.FirstOrDefault(x => x.Property == c.Property);
            activityColumn ??= new ActivityColumn(c.Property);
            activityColumn.Order = vm.EditColumns.IndexOf(c);
          }
        }

        ColumnsChanged?.Invoke(this, EventArgs.Empty);
      }
    }

    private bool EditColumnsCommandCanExecute => !_isBusy && Schedule != null && Schedule.CurrentLayout != null;

    #endregion

    #region Resources

    public ObservableCollection<ResourceViewModel> Resources { get; private set; } = new ObservableCollection<ResourceViewModel>();

    private void RefreshResources()
    {
      Resources.Clear();
      if (CurrentLayout != null)
      {
        foreach (var resource in CurrentLayout.Layout.VisibleResources)
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
        RefreshLayout?.Invoke(this, new ItemEventArgs<Layout>(CurrentLayout.Layout));
      }
    }

    private IVisibleLayoutViewModel AddLayoutVM(Layout layout, bool makeCurrent)
    {
      var vm = LayoutVMFactory.CreateVM(layout);

      Layouts.Add(vm);

      if (makeCurrent)
      {
        CurrentLayout = vm;
      }

      return vm;
    }

    private void ChangeLayout(Layout layout)
    {
      var oldVM = Layouts.FirstOrDefault(x => x.Layout == layout);
      if (oldVM != null)
      {
        int idx = Layouts.IndexOf(oldVM);
        var newVM = LayoutVMFactory.CreateVM(layout);
        Layouts.Insert(idx, newVM);
        CurrentLayout = newVM;
        _ = Layouts.Remove(oldVM);
      }
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
        foreach (var sortDefinition in Schedule.CurrentLayout.SortingDefinitions)
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
        foreach (var groupDefinition in Schedule.CurrentLayout.GroupingDefinitions)
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

      if (Schedule.CurrentLayout.FilterDefinitions.Count == 0)
      {
        return true;
      }

      foreach (var filter in Schedule.CurrentLayout.FilterDefinitions)
      {
        bool isTrue = filter.Compare(activity);
        if (Schedule.CurrentLayout.FilterCombination == FilterCombinationType.Or && isTrue)
        {
          return true;
        }

        if (Schedule.CurrentLayout.FilterCombination == FilterCombinationType.And && !isTrue)
        {
          return false;
        }
      }
      return Schedule.CurrentLayout.FilterCombination == FilterCombinationType.And;
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

        if (_currentLayout != null)
        {
          _currentLayout.Dispose();
          _currentLayout = null;
        }
      }
    }

    #endregion
  }
}
