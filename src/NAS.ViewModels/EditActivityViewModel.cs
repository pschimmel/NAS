using System.Collections.ObjectModel;
using System.Windows.Input;
using ES.Tools.Core.MVVM;
using NAS.Models.Entities;
using NAS.Models.Enums;
using NAS.Resources;
using NAS.ViewModels.Base;
using NAS.ViewModels.Helpers;

namespace NAS.ViewModels
{
  public class EditActivityViewModel : DialogContentViewModel
  {
    #region Fields

    private readonly Schedule _schedule;
    private readonly Activity _activity;
    private ResourceAssignment _currentResourceAssignment;

    #endregion

    #region Constructor

    public EditActivityViewModel(Schedule schedule, Activity activity)
      : base()
    {
      ArgumentNullException.ThrowIfNull(activity);
      ArgumentNullException.ThrowIfNull(schedule);
      _schedule = schedule;
      _activity = activity;
      ResourceAssignments = [];
      Calendars = new ObservableCollection<Calendar>(_schedule.Calendars);
      Fragnets = new ObservableCollection<Fragnet>(_schedule.Fragnets);
      CustomAttributes1 = new List<CustomAttribute>(_schedule.CustomAttributes1);
      CustomAttributes2 = new List<CustomAttribute>(_schedule.CustomAttributes2);
      CustomAttributes3 = new List<CustomAttribute>(_schedule.CustomAttributes3);
      SelectWBSCommand = new ActionCommand(SelectWBSCommandExecute);
      EditLogicCommand = new ActionCommand(EditLogicCommandExecute, () => EditLogicCommandCanExecute);
      EditDistortionsCommand = new ActionCommand(EditDistortionsCommandExecute, () => EditDistortionsCommandCanExecute);
      AddResourceAssignmentCommand = new ActionCommand(AddResourceAssignmentCommandExecute);
      RemoveResourceAssignmentCommand = new ActionCommand(RemoveResourceAssignmentCommandExecute, () => RemoveResourceAssignmentCommandCanExecute);
      EditResourceAssignmentCommand = new ActionCommand(EditResourceAssignmentCommandExecute, () => EditResourceAssignmentCommandCanExecute);
    }

    #endregion

    #region Overwritten Members

    public override string Title => NASResources.EditActivity;

    public override string Icon => "Activity";

    public override DialogSize DialogSize => DialogSize.Fixed(600, 450);

    #endregion

    #region Properties

    public override HelpTopic HelpTopicKey => HelpTopic.Activity;

    public ObservableCollection<ResourceAssignment> ResourceAssignments { get; }

    public ObservableCollection<Calendar> Calendars { get; }

    public ObservableCollection<Fragnet> Fragnets { get; }

    public List<ConstraintType> ConstraintTypes => Enum.GetValues<ConstraintType>().Cast<ConstraintType>().ToList();

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

    public List<CustomAttribute> CustomAttributes1 { get; }

    public List<CustomAttribute> CustomAttributes2 { get; }

    public List<CustomAttribute> CustomAttributes3 { get; }

    public bool IsActivity => _activity.ActivityType == ActivityType.Activity;

    public bool IsFixed => _activity.IsFixed;

    public bool IsNotFixed => !IsFixed;

    public string DisplayName
    {
      get
      {
        string result = _activity.Number + " " + _activity.Name;
        if (IsFixed)
        {
          result += " (" + NASResources.Fixed + ")";
        }

        return result;
      }
    }

    #endregion

    #region Select WBS

    public ICommand SelectWBSCommand { get; }

    private void SelectWBSCommandExecute()
    {
      using var vm = new SelectWBSViewModel(_schedule, _activity);
      if (ViewFactory.Instance.ShowDialog(vm) == true)
      {
        _activity.WBSItem = vm.CurrentWBSItem?.Item;
      }
    }

    #endregion

    #region Edit Logic

    public ICommand EditLogicCommand { get; }

    private void EditLogicCommandExecute()
    {
      var vm = new EditLogicViewModel(_schedule, _activity);
      ViewFactory.Instance.ShowDialog(vm);
    }

    private bool EditLogicCommandCanExecute => _activity != null;

    #endregion

    #region Edit Distortions

    public ICommand EditDistortionsCommand { get; }

    private void EditDistortionsCommandExecute()
    {
      using var vm = new EditDistortionsViewModel(_schedule, _activity);
      ViewFactory.Instance.ShowDialog(vm);
    }

    private bool EditDistortionsCommandCanExecute => _activity.ActivityType == ActivityType.Activity;

    #endregion

    #region Add Resource Association

    public ICommand AddResourceAssignmentCommand { get; }

    private void AddResourceAssignmentCommandExecute()
    {
      using var vm = new SelectResourceViewModel(_schedule.Resources);
      if (ViewFactory.Instance.ShowDialog(vm) == true && vm.SelectedResource != null)
      {
        var ResourceAssignment = new ResourceAssignment(_activity, vm.SelectedResource);
        using var vm2 = new ResourceAssignmentViewModel(ResourceAssignment);
        if (ViewFactory.Instance.ShowDialog(vm2) == true)
        {
          ResourceAssignments.Add(ResourceAssignment);
        }
      }
    }

    #endregion

    #region Remove Resource Association

    public ICommand RemoveResourceAssignmentCommand { get; }

    private void RemoveResourceAssignmentCommandExecute()
    {
      UserNotificationService.Instance.Question(NASResources.MessageDeleteResourceAssignment, () =>
      {
        ResourceAssignments.Remove(CurrentResourceAssignment);
      });
    }

    private bool RemoveResourceAssignmentCommandCanExecute => CurrentResourceAssignment != null;

    #endregion

    #region Edit Resource Association

    public ICommand EditResourceAssignmentCommand { get; }

    private void EditResourceAssignmentCommandExecute()
    {
      using var vm = new ResourceAssignmentViewModel(CurrentResourceAssignment);
      ViewFactory.Instance.ShowDialog(vm);
    }

    private bool EditResourceAssignmentCommandCanExecute => CurrentResourceAssignment != null;

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
  }
}
