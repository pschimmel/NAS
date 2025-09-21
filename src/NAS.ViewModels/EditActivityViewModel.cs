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
    private WBSItem _wbsItem;
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
        schedule.ResourceAssignments?.Where(x => x.Activity == activity) ?? Enumerable.Empty<ResourceAssignment>()
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
      OriginalDuration = activity.OriginalDuration;
      Calendar = activity.Calendar ?? Calendars.FirstOrDefault();

      // Initialize dates with proper null checks
      EarlyStartDate = activity.EarlyStartDate;
      EarlyFinishDate = activity.EarlyFinishDate;
      LateStartDate = activity.LateStartDate;
      LateFinishDate = activity.LateFinishDate;
      ActualStartDate = activity.ActualStartDate;
      ActualFinishDate = activity.ActualFinishDate;

      // Initialize numeric values
      TotalFloat = activity.TotalFloat;
      FreeFloat = activity.FreeFloat;
      PercentComplete = activity.PercentComplete;
      RemainingDuration = activity.RemainingDuration;
      ActualDuration = activity.ActualDuration;
      AtCompletionDuration = activity.AtCompletionDuration;

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

    public bool IsFixed { get; }

    public bool IsNotFixed => !IsFixed;

    public int OriginalDuration { get; set; }

    public ObservableCollection<Calendar> Calendars { get; }

    public Calendar Calendar { get; set; }

    public DateTime EarlyStartDate { get; set; }

    public DateTime EarlyFinishDate { get; set; }

    public DateTime LateStartDate { get; }

    public DateTime LateFinishDate { get; }

    public int TotalFloat { get; }

    public int FreeFloat { get; }

    public DateTime? ActualStartDate { get; set; }

    public DateTime? ActualFinishDate { get; set; }

    public double PercentComplete { get; set; }

    public int RemainingDuration { get; set; }

    public int ActualDuration { get; }

    public int AtCompletionDuration { get; }

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
