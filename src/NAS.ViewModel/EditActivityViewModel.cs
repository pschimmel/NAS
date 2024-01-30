using System.Collections.ObjectModel;
using System.Windows.Input;
using ES.Tools.Core.MVVM;
using NAS.Model.Entities;
using NAS.Model.Enums;
using NAS.Resources;
using NAS.ViewModel.Base;
using NAS.ViewModel.Helpers;

namespace NAS.ViewModel
{
  public class EditActivityViewModel : ViewModelBase, IValidatable, IApplyable
  {
    #region Fields

    private readonly Schedule _schedule;
    private readonly Activity _activity;
    private ResourceAssociation _currentResourceAssociation;

    #endregion

    #region Constructor

    public EditActivityViewModel(Activity activity)
      : base()
    {
      ArgumentNullException.ThrowIfNull(activity);
      _schedule = activity.Schedule;
      _activity = activity;
      ResourceAssociations = [];
      Calendars = new ObservableCollection<Calendar>(_schedule.Calendars);
      Fragnets = new ObservableCollection<Fragnet>(_schedule.Fragnets);
      CustomAttributes1 = new List<CustomAttribute>(_schedule.CustomAttributes1);
      CustomAttributes2 = new List<CustomAttribute>(_schedule.CustomAttributes2);
      CustomAttributes3 = new List<CustomAttribute>(_schedule.CustomAttributes3);
      SelectWBSCommand = new ActionCommand(SelectWBSCommandExecute);
      EditLogicCommand = new ActionCommand(EditLogicCommandExecute, () => EditLogicCommandCanExecute);
      EditDistortionsCommand = new ActionCommand(EditDistortionsCommandExecute, () => EditDistortionsCommandCanExecute);
      AddResourceAssociationCommand = new ActionCommand(AddResourceAssociationCommandExecute);
      RemoveResourceAssociationCommand = new ActionCommand(RemoveResourceAssociationCommandExecute, () => RemoveResourceAssociationCommandCanExecute);
      EditResourceAssociationCommand = new ActionCommand(EditResourceAssociationCommandExecute, () => EditResourceAssociationCommandCanExecute);
    }

    #endregion

    #region Public Properties

    public override HelpTopic HelpTopicKey => HelpTopic.Activity;

    public ObservableCollection<ResourceAssociation> ResourceAssociations { get; }

    public ObservableCollection<Calendar> Calendars { get; }

    public ObservableCollection<Fragnet> Fragnets { get; }

#pragma warning disable CA1822 // Mark members as static
    public List<ConstraintType> ConstraintTypes => Enum.GetValues(typeof(ConstraintType)).Cast<ConstraintType>().ToList();
#pragma warning restore CA1822 // Mark members as static

    public ResourceAssociation CurrentResourceAssociation
    {
      get => _currentResourceAssociation;
      set
      {
        if (_currentResourceAssociation != value)
        {
          _currentResourceAssociation = value;
          OnPropertyChanged(nameof(CurrentResourceAssociation));
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
      using var vm = new SelectWBSViewModel(_activity);
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
      var vm = new EditLogicViewModel(_activity);
      _ = ViewFactory.Instance.ShowDialog(vm);
    }

    private bool EditLogicCommandCanExecute => _activity != null;

    #endregion

    #region Edit Distortions

    public ICommand EditDistortionsCommand { get; }

    private void EditDistortionsCommandExecute()
    {
      using var vm = new DistortionsViewModel(_activity);
      _ = ViewFactory.Instance.ShowDialog(vm);
    }

    private bool EditDistortionsCommandCanExecute => _activity.ActivityType == ActivityType.Activity;

    #endregion

    #region Add Resource Association

    public ICommand AddResourceAssociationCommand { get; }

    private void AddResourceAssociationCommandExecute()
    {
      using var vm = new SelectResourceViewModel(_schedule);
      if (ViewFactory.Instance.ShowDialog(vm) == true && vm.SelectedResource != null)
      {
        var resourceAssociation = new ResourceAssociation(_activity, vm.SelectedResource);
        using var vm2 = new ResourceAssociationViewModel(resourceAssociation);
        if (ViewFactory.Instance.ShowDialog(vm2) == true)
        {
          ResourceAssociations.Add(resourceAssociation);
        }
      }
    }

    #endregion

    #region Remove Resource Association

    public ICommand RemoveResourceAssociationCommand { get; }

    private void RemoveResourceAssociationCommandExecute()
    {
      UserNotificationService.Instance.Question(NASResources.MessageDeleteResourceAssociation, () =>
      {
        _ = ResourceAssociations.Remove(CurrentResourceAssociation);
      });
    }

    private bool RemoveResourceAssociationCommandCanExecute => CurrentResourceAssociation != null;

    #endregion

    #region Edit Resource Association

    public ICommand EditResourceAssociationCommand { get; }

    private void EditResourceAssociationCommandExecute()
    {
      using var vm = new ResourceAssociationViewModel(CurrentResourceAssociation);
      _ = ViewFactory.Instance.ShowDialog(vm);
    }

    private bool EditResourceAssociationCommandCanExecute => CurrentResourceAssociation != null;

    #endregion

    #region Validation

    public string ErrorMessage { get; private set; } = null;

    public bool HasErrors => !string.IsNullOrWhiteSpace(ErrorMessage);

    private void AddError(string message)
    {
      if (!string.IsNullOrWhiteSpace(message))
      {
        if (!string.IsNullOrWhiteSpace(ErrorMessage))
        {
          ErrorMessage += Environment.NewLine;
        }

        ErrorMessage += message;
        OnPropertyChanged(nameof(ErrorMessage));
        OnPropertyChanged(nameof(HasErrors));
      }
    }

    private void ResetErrors()
    {
      ErrorMessage = null;
      OnPropertyChanged(nameof(ErrorMessage));
      OnPropertyChanged(nameof(HasErrors));
    }

    public bool Validate()
    {
      ResetErrors();
      if (string.IsNullOrWhiteSpace(_activity.Number))
      {
        AddError(NASResources.PleaseEnterNumber);
      }

      if (string.IsNullOrWhiteSpace(_activity.Name))
      {
        AddError(NASResources.PleaseEnterName);
      }

      if (_activity.ActivityType == ActivityType.Activity && _activity.OriginalDuration <= 0)
      {
        AddError(NASResources.PleaseEnterPlannedDuration);
      }

      if (_activity.PercentComplete is < 0d or > 100d)
      {
        AddError(string.Format(NASResources.PleaseEnterNumbersFromTo, 0, 100));
      }

      if (_activity.Calendar == null)
      {
        AddError(NASResources.PleaseSelectCalendar);
      }

      if (_activity.Constraint != ConstraintType.None && !_activity.ConstraintDate.HasValue)
      {
        AddError(NASResources.PleaseEnterConstraintDate);
      }

      foreach (var activity in _schedule.Activities)
      {
        if (activity != _activity && activity.Number == _activity.Number)
        {
          AddError(NASResources.NumberMustBeUnique);
        }
      }
      return !HasErrors;
    }

    #endregion

    #region IApplyable

    public void Apply()
    {
      if (Validate() == true)
      {
        _activity.RefreshResourceAssociations(ResourceAssociations);
      }
    }

    #endregion
  }
}
