using ES.Tools.Core.MVVM;
using NAS.Models.Entities;
using NAS.Models.Enums;
using NAS.Resources;
using NAS.ViewModels.Base;
using NAS.ViewModels.Helpers;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace NAS.ViewModels
{
  public class EditActivityViewModel : DialogContentViewModel
  {
    #region Fields

    private readonly Schedule _schedule;
    private readonly Activity _activity;
    private ResourceAssignment _currentResourceAssignment;
    private DateTime _earlyStartDate;
    private DateTime _earlyFinishDate;
    private DateTime _lateStartDate;
    private DateTime _lateFinishDate;
    private DateTime? _actualStartDate;
    private DateTime? _actualFinishDate;
    private double _percentComplete;
    private WBSItem _wbsItem;
    private int _originalDuration;
    private int _remainingDuration;
    private ActionCommand _selectWBSCommand;
    private ActionCommand _editLogicCommand;
    private ActionCommand _editDistortionsCommand;
    private ActionCommand _addResourceAssignmentCommand;
    private ActionCommand _removeResourceAssignmentCommand;
    private ActionCommand _editResourceAssignmentCommand;

    #endregion

    #region Constructor

    public EditActivityViewModel(Schedule schedule, Activity activity)
      : base()
    {
      ArgumentNullException.ThrowIfNull(schedule, nameof(schedule));
      ArgumentNullException.ThrowIfNull(activity, nameof(activity));

      _schedule = schedule;
      _activity = activity;

      // Initialize collections with null checks
      ResourceAssignments = new ObservableCollection<ResourceAssignment>(
        schedule.ResourceAssignments?.Where(x => x.Activity == activity) ?? []
      );

      Calendars = new ObservableCollection<Calendar>(schedule.Calendars ?? Enumerable.Empty<Calendar>());
      Fragnets = new List<Fragnet>(schedule.Fragnets ?? Enumerable.Empty<Fragnet>());
      CustomAttributes1 = new List<CustomAttribute>(schedule.CustomAttributes1 ?? Enumerable.Empty<CustomAttribute>());
      CustomAttributes2 = new List<CustomAttribute>(schedule.CustomAttributes2 ?? Enumerable.Empty<CustomAttribute>());
      CustomAttributes3 = new List<CustomAttribute>(schedule.CustomAttributes3 ?? Enumerable.Empty<CustomAttribute>());

      // Initialize basic properties
      Number = activity.Number;
      Name = activity.Name;
      IsActivity = activity.ActivityType == ActivityType.Activity;
      IsFixed = activity.IsFixed;
      _originalDuration = activity.OriginalDuration;
      Calendar = activity.Calendar ?? Calendars.FirstOrDefault();

      // Initialize dates with proper null checks
      _earlyStartDate = activity.EarlyStartDate;
      _earlyFinishDate = activity.EarlyFinishDate;
      _lateStartDate = activity.LateStartDate;
      _lateFinishDate = activity.LateFinishDate;
      _actualStartDate = activity.ActualStartDate;
      _actualFinishDate = activity.ActualFinishDate;

      // Initialize numeric values
      TotalFloat = activity.TotalFloat;
      FreeFloat = activity.FreeFloat;
      _percentComplete = activity.PercentComplete;
      _remainingDuration = activity.RemainingDuration;

      // Initialize references
      _wbsItem = activity.WBSItem;
      Fragnet = activity.Fragnet;
      Constraint = activity.Constraint;
      ConstraintDate = activity.ConstraintDate;
      CustomAttribute1 = activity.CustomAttribute1;
      CustomAttribute2 = activity.CustomAttribute2;
      CustomAttribute3 = activity.CustomAttribute3;

      // Initialize costs
      CalculateResourceCosts();
    }

    #endregion

    #region Overwritten Members

    public override string Title => NASResources.EditActivity;

    public override string Icon => "Activity";

    public override DialogSize DialogSize => DialogSize.Fixed(600, 450);

    public override HelpTopic HelpTopicKey => HelpTopic.Activity;

    #endregion

    #region Properties

    public string Number { get; set; }

    public string Name { get; set; }

    public bool IsActivity { get; }

    public bool IsMilestone => !IsActivity;

    public bool IsFixed { get; }

    public bool IsNotFixed => !IsFixed;

    public ObservableCollection<Calendar> Calendars { get; }

    public Calendar Calendar { get; set; }

    public DateTime EarlyStartDate
    {
      get => _earlyStartDate;
      set
      {
        if (_earlyStartDate != value)
        {
          _earlyStartDate = value;
          OnPropertyChanged(nameof(EarlyStartDate));
          if (Calendar != null)
          {
            EarlyFinishDate = Calendar.GetEndDate(EarlyStartDate, RemainingDuration);
          }
        }
      }
    }

    public DateTime EarlyFinishDate
    {
      get => _earlyFinishDate;
      set
      {
        if (_earlyFinishDate != value)
        {
          _earlyFinishDate = value;
          OnPropertyChanged(nameof(EarlyFinishDate));
          if (Calendar != null)
          {
            EarlyStartDate = Calendar.GetStartDate(EarlyFinishDate, RemainingDuration);
          }
        }
      }
    }

    public DateTime LateStartDate
    {
      get => _lateStartDate;
      set
      {
        if (_lateStartDate != value)
        {
          _lateStartDate = value;
          OnPropertyChanged(nameof(LateStartDate));
          if (Calendar != null)
          {
            LateFinishDate = Calendar.GetEndDate(LateStartDate, RemainingDuration);
          }
        }
      }
    }

    public DateTime LateFinishDate
    {
      get => _lateFinishDate;
      set
      {
        if (_lateFinishDate != value)
        {
          _lateFinishDate = value;
          OnPropertyChanged(nameof(LateFinishDate));
          if (Calendar != null)
          {
            LateStartDate = Calendar.GetStartDate(LateFinishDate, RemainingDuration);
          }
        }
      }
    }


    public DateTime? ActualStartDate
    {
      get => _actualStartDate;
      set
      {
        if (_actualStartDate != value)
        {
          _actualStartDate = value;
          OnPropertyChanged(nameof(ActualStartDate));
          OnPropertyChanged(nameof(ActualDuration));
        }
      }
    }

    public DateTime? ActualFinishDate
    {
      get => _actualFinishDate;
      set
      {
        if (_actualFinishDate != value)
        {
          _actualFinishDate = value;
          OnPropertyChanged(nameof(ActualFinishDate));
          OnPropertyChanged(nameof(ActualDuration));
        }
      }
    }

    public int OriginalDuration
    {
      get => IsActivity ? _originalDuration : 0;
      set
      {
        if (_originalDuration != value && IsActivity)
        {
          _originalDuration = value;
          OnPropertyChanged(nameof(OriginalDuration));
          OnPropertyChanged(nameof(RemainingDuration));
          OnPropertyChanged(nameof(AtCompletionDuration));
          RemainingDuration = Convert.ToInt32(Convert.ToDouble(DelayedDuration) * (100d - PercentComplete) / 100d);
          if (Calendar != null)
          {
            EarlyFinishDate = Calendar.GetEndDate(EarlyStartDate, RemainingDuration);
          }
        }
      }
    }

    public int RemainingDuration
    {
      get => IsActivity ? _remainingDuration : 0;
      set
      {
        if (_remainingDuration != value && IsActivity)
        {
          _remainingDuration = value;
          PercentComplete = DelayedDuration != 0
            ? (Convert.ToDouble(DelayedDuration) - Convert.ToDouble(RemainingDuration)) * 100d / Convert.ToDouble(DelayedDuration)
            : 0;

          OnPropertyChanged(nameof(RemainingDuration));
          OnPropertyChanged(nameof(AtCompletionDuration));
        }
      }
    }

    public int AtCompletionDuration => ActualDuration + RemainingDuration;

    public int DelayedDuration
    {
      get
      {
        if (IsMilestone)
        {
          return 0;
        }

        int result = OriginalDuration;
        foreach (var d in _activity.Distortions)
        {
          if (d.Fragnet == null || d.Fragnet.IsVisible)
          {
            if (d is Delay && (d as Delay).Days.HasValue)
            {
              result += (d as Delay).Days.Value;
            }
            else if (d is Interruption && (d as Interruption).Days.HasValue)
            {
              result += (d as Interruption).Days.Value;
            }
            else if (d is Inhibition && (d as Inhibition).Percent.HasValue)
            {
              result += Convert.ToInt32(Math.Round(OriginalDuration * (d as Inhibition).Percent.Value / 100));
            }
            else if (d is Extension && (d as Extension).Days.HasValue)
            {
              result += (d as Extension).Days.Value;
            }
            else if (d is Reduction && (d as Reduction).Days.HasValue)
            {
              result -= (d as Reduction).Days.Value;
            }
          }
        }
        if (result < 1)
        {
          result = 1;
        }

        return result;
      }
    }

    public double PercentComplete
    {
      get => _percentComplete;
      set
      {
        if (_percentComplete != value)
        {
          if (value < 0)
          {
            _percentComplete = 0;
          }
          else if (value > 100)
          {
            _percentComplete = 100;
          }
          else if (IsMilestone)
          {
            _percentComplete = value < 50 ? 0 : 100;
          }
          else
          {
            _percentComplete = value;
            if (_percentComplete > 0 && !ActualStartDate.HasValue)
            {
              ActualStartDate = EarlyStartDate;
            }

            if (_percentComplete == 100 && !ActualFinishDate.HasValue)
            {
              ActualFinishDate = EarlyFinishDate;
            }
          }

          OnPropertyChanged(nameof(PercentComplete));
        }
      }
    }

    public int TotalFloat { get; }

    public int FreeFloat { get; }

    public int ActualDuration
    {
      get => IsActivity && ActualStartDate.HasValue
            ? ActualStartDate.HasValue && ActualFinishDate.HasValue && Calendar != null
              ? Calendar.GetWorkDays(ActualStartDate.Value, ActualFinishDate.Value, false)
              : DelayedDuration
            : 0;
    }

    public WBSItem WBSItem
    {
      get => _wbsItem;
      set
      {
        if (_wbsItem != value)
        {
          _wbsItem = value;
          OnPropertyChanged(nameof(WBSItem));
        }
      }
    }

    public List<Fragnet> Fragnets { get; }

    public Fragnet Fragnet { get; set; }

    public List<ConstraintType> ConstraintTypes => Enum.GetValues<ConstraintType>().Cast<ConstraintType>().ToList();

    public ConstraintType Constraint { get; set; }

    public DateTime? ConstraintDate { get; set; }

    public ObservableCollection<ResourceAssignment> ResourceAssignments { get; }

    public ResourceAssignment CurrentResourceAssignment
    {
      get => _currentResourceAssignment;
      set
      {
        if (_currentResourceAssignment != value)
        {
          _currentResourceAssignment = value;
          OnPropertyChanged(nameof(CurrentResourceAssignment));
        }
      }
    }

    public string CustomAttribute1Header => _schedule.CustomAttribute1Header;

    public List<CustomAttribute> CustomAttributes1 { get; }

    public CustomAttribute CustomAttribute1 { get; set; }

    public string CustomAttribute2Header => _schedule.CustomAttribute2Header;

    public List<CustomAttribute> CustomAttributes2 { get; }

    public CustomAttribute CustomAttribute2 { get; set; }

    public string CustomAttribute3Header => _schedule.CustomAttribute3Header;

    public List<CustomAttribute> CustomAttributes3 { get; }

    public CustomAttribute CustomAttribute3 { get; set; }

    public decimal TotalBudget { get; private set; }

    public decimal TotalPlannedCosts { get; private set; }

    public decimal TotalActualCosts { get; private set; }

    #endregion

    #region Select WBS

    public ICommand SelectWBSCommand => _selectWBSCommand ??= new ActionCommand(SelectWBS);

    private void SelectWBS()
    {
      using var vm = new SelectWBSViewModel(_schedule, _activity.WBSItem);
      if (ViewFactory.Instance.ShowDialog(vm) == true)
      {
        WBSItem = vm.SelectedWBSItem?.Item;
      }
    }

    #endregion

    #region Edit Logic

    public ICommand EditLogicCommand => _editLogicCommand ??= new ActionCommand(EditLogic, CanEditLogic);

    private void EditLogic()
    {
      var vm = new EditLogicViewModel(_schedule, _activity);
      ViewFactory.Instance.ShowDialog(vm);
    }

    private bool CanEditLogic()
    {
      return _activity != null;
    }

    #endregion

    #region Edit Distortions

    public ICommand EditDistortionsCommand => _editDistortionsCommand ??= new ActionCommand(EditDistortions, CanEditDistortions);

    private void EditDistortions()
    {
      using var vm = new EditDistortionsViewModel(_schedule, _activity);
      ViewFactory.Instance.ShowDialog(vm);
    }

    private bool CanEditDistortions()
    {
      return _activity.ActivityType == ActivityType.Activity;
    }

    #endregion

    #region Add Resource Association

    public ICommand AddResourceAssignmentCommand => _addResourceAssignmentCommand ??= new ActionCommand(AddResourceAssignment);

    private void AddResourceAssignment()
    {
      using var vm = new SelectResourceViewModel(_schedule.Resources);
      if (ViewFactory.Instance.ShowDialog(vm) == true && vm.SelectedResource != null)
      {
        var ResourceAssignment = new ResourceAssignment(_activity, vm.SelectedResource);
        using var vm2 = new ResourceAssignmentViewModel(ResourceAssignment);
        if (ViewFactory.Instance.ShowDialog(vm2) == true)
        {
          ResourceAssignments.Add(ResourceAssignment);
          CalculateResourceCosts();
        }
      }
    }

    #endregion

    #region Remove Resource Association

    public ICommand RemoveResourceAssignmentCommand => _removeResourceAssignmentCommand ??= new ActionCommand(RemoveResourceAssignment, CanRemoveResourceAssignment);

    private void RemoveResourceAssignment()
    {
      UserNotificationService.Instance.Question(NASResources.MessageDeleteResourceAssignment, () =>
      {
        ResourceAssignments.Remove(CurrentResourceAssignment);
        CalculateResourceCosts();
      });
    }

    private bool CanRemoveResourceAssignment()
    {
      return CurrentResourceAssignment != null;
    }

    #endregion

    #region Edit Resource Association

    public ICommand EditResourceAssignmentCommand => _editResourceAssignmentCommand ??= new ActionCommand(EditResourceAssignment, CanEditResourceAssignment);

    private void EditResourceAssignment()
    {
      using var vm = new ResourceAssignmentViewModel(CurrentResourceAssignment);
      ViewFactory.Instance.ShowDialog(vm);
      CalculateResourceCosts();
    }

    private bool CanEditResourceAssignment()
    {
      return CurrentResourceAssignment != null;
    }

    #endregion

    #region Private members

    private void CalculateResourceCosts()
    {
      TotalBudget = ResourceAssignments?.Sum(x => (x.Budget + x.FixedCosts)) ?? 0m;
      TotalPlannedCosts = ResourceAssignments?.Sum(x => x.PlannedCosts) ?? 0m;
      TotalActualCosts = ResourceAssignments?.Sum(x => x.ActualCosts) ?? 0m;

      OnPropertyChanged(nameof(TotalBudget));
      OnPropertyChanged(nameof(TotalPlannedCosts));
      OnPropertyChanged(nameof(TotalActualCosts));
    }

    #endregion

    #region Validation

    protected override ValidationResult OnValidating()
    {
      if (string.IsNullOrWhiteSpace(_activity.Number))
      {
        return ValidationResult.Error(NASResources.PleaseEnterNumber);
      }

      if (string.IsNullOrWhiteSpace(_activity.Name))
      {
        return ValidationResult.Error(NASResources.PleaseEnterName);
      }

      if (_activity.ActivityType == ActivityType.Activity && _activity.OriginalDuration <= 0)
      {
        return ValidationResult.Error(NASResources.PleaseEnterPlannedDuration);
      }

      if (_activity.PercentComplete is < 0d or > 100d)
      {
        return ValidationResult.Error(string.Format(NASResources.PleaseEnterNumbersFromTo, 0, 100));
      }

      if (_activity.Calendar == null)
      {
        return ValidationResult.Error(NASResources.PleaseSelectCalendar);
      }

      if (_activity.Constraint != ConstraintType.None && !_activity.ConstraintDate.HasValue)
      {
        return ValidationResult.Error(NASResources.PleaseEnterConstraintDate);
      }

      foreach (var activity in _schedule.Activities)
      {
        if (activity != _activity && activity.Number == _activity.Number)
        {
          return ValidationResult.Error(NASResources.NumberMustBeUnique);
        }
      }
      return ValidationResult.OK();
    }

    #endregion

    #region Apply

    protected override void OnApply()
    {
      _activity.Number = Number;
      _activity.Name = Name;
      _activity.OriginalDuration = OriginalDuration;
      _activity.Calendar = Calendar;
      _activity.EarlyStartDate = EarlyStartDate;
      _activity.EarlyFinishDate = EarlyFinishDate;
      _activity.ActualStartDate = ActualStartDate;
      _activity.ActualFinishDate = ActualFinishDate;
      _activity.PercentComplete = PercentComplete;
      _activity.RemainingDuration = RemainingDuration;
      _activity.WBSItem = WBSItem;
      _activity.Fragnet = Fragnet;
      _activity.Constraint = Constraint;
      _activity.ConstraintDate = ConstraintDate;
      _activity.CustomAttribute1 = CustomAttribute1;
      _activity.CustomAttribute2 = CustomAttribute2;
      _activity.CustomAttribute3 = CustomAttribute3;
      foreach (var ra in _schedule.ResourceAssignments.Where(x => x.Activity == _activity).ToList())
      {
        if (!ResourceAssignments.Contains(ra))
        {
          _schedule.ResourceAssignments.Remove(ra);
        }
      }
      foreach (var ra in ResourceAssignments)
      {
        if (!_schedule.ResourceAssignments.Contains(ra))
        {
          _schedule.ResourceAssignments.Add(ra);
        }
      }
    }

    #endregion
  }
}
