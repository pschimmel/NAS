using System.Collections.ObjectModel;
using System.Windows.Input;
using ES.Tools.Core.MVVM;
using NAS.Models.Entities;
using NAS.Resources;
using NAS.ViewModels.Base;
using NAS.ViewModels.Helpers;

namespace NAS.ViewModels
{
  public class EditLogicViewModel : DialogContentViewModel
  {
    #region Fields

    private Activity _selectedActivity;
    private readonly Schedule _schedule;
    private readonly List<Activity> _activities;
    private readonly List<Relationship> _relationships;
    private LogicRelatedViewModel _selectedPredecessor;
    private LogicRelatedViewModel _selectedSuccessor;
    private ActionCommand _addPredecessorCommand;
    private ActionCommand _removePredecessorCommand;
    private ActionCommand _editPredecessorCommand;
    private ActionCommand _gotoPredecessorCommand;
    private ActionCommand _addSuccessorCommand;
    private ActionCommand _removeSuccessorCommand;
    private ActionCommand _editSuccessorCommand;
    private ActionCommand _gotoSuccessorCommand;

    #endregion

    #region Constructor

    public EditLogicViewModel(Schedule schedule, Activity selectedActivity)
      : base()
    {
      _schedule = schedule;
      var clonedActivities = new Dictionary<Activity, Activity>();

      foreach (var activity in schedule.Activities)
      {
        var clone = activity.Clone();
        _activities.Add(clone);
        clonedActivities.Add(activity, clone);
      }

      foreach (var relationship in schedule.Relationships)
      {
        var clone = relationship.Clone(clonedActivities[relationship.Activity1], clonedActivities[relationship.Activity2]);
        _relationships.Add(clone);
      }

      SelectedActivity = clonedActivities[selectedActivity];
    }

    #endregion

    #region Overwritten Members

    public override string Title => NASResources.EditLogic;

    public override string Icon => "EditRelationships";

    public override DialogSize DialogSize => DialogSize.Fixed(350, 350);

    public override HelpTopic HelpTopicKey => HelpTopic.Relationship;

    #endregion

    #region Properties

    public Activity SelectedActivity
    {
      get => _selectedActivity;
      private set
      {
        if (!ReferenceEquals(_selectedActivity, value))
        {
          _selectedActivity = value;
          Predecessors.Clear();
          Successors.Clear();
          if (_selectedActivity != null)
          {
            foreach (var relationship in GetPreceedingRelationships(_selectedActivity))
            {
              Predecessors.Add(new LogicRelatedViewModel(relationship.Activity1, relationship));
            }

            foreach (var relationship in GetSucceedingRelationships(_selectedActivity))
            {
              Successors.Add(new LogicRelatedViewModel(relationship.Activity2, relationship));
            }
          }

          OnPropertyChanged(nameof(SelectedActivity));
        }
      }
    }

    public ObservableCollection<LogicRelatedViewModel> Predecessors { get; } = [];

    public LogicRelatedViewModel SelectedPredecessor
    {
      get => _selectedPredecessor;
      set
      {
        if (_selectedPredecessor != value)
        {
          _selectedPredecessor = value;
          OnPropertyChanged(nameof(SelectedPredecessor));
        }
      }
    }

    public ObservableCollection<LogicRelatedViewModel> Successors { get; } = [];

    public LogicRelatedViewModel SelectedSuccessor
    {
      get => _selectedSuccessor;
      set
      {
        if (_selectedSuccessor != value)
        {
          _selectedSuccessor = value;
          OnPropertyChanged(nameof(SelectedSuccessor));
        }
      }
    }

    #endregion

    #region Add Predecessor

    public ICommand AddPredecessorCommand => _addPredecessorCommand ??= new ActionCommand(AddPredecessor, CanAddPredecessor);

    private void AddPredecessor()
    {
      var otherActivities = _activities.Where(x => x != SelectedActivity && !Predecessors.Select(y => y.Activity).Contains(x));
      using var vm = new SelectActivityViewModel(otherActivities);
      if (ViewFactory.Instance.ShowDialog(vm) == true)
      {
        if (vm.SelectedActivity == SelectedActivity)
        {
          UserNotificationService.Instance.Error(NASResources.MessageCircularDependency);
        }
        else
        {
          var relationship = new Relationship(vm.SelectedActivity, SelectedActivity);
          var newVM = new LogicRelatedViewModel(vm.SelectedActivity, relationship);
          Predecessors.Add(newVM);
          SelectedPredecessor = newVM;
        }
      }
    }

    private bool CanAddPredecessor()
    {
      return SelectedActivity != null;
    }

    #endregion

    #region Remove Predecessor

    public ICommand RemovePredecessorCommand => _removePredecessorCommand ??= new ActionCommand(RemovePredecessor, CanRemovePredecessor);

    private void RemovePredecessor()
    {
      UserNotificationService.Instance.Question(NASResources.MessageDeleteRelationship, () =>
      {
        _relationships.Remove(SelectedPredecessor.Relationship);
      });
    }

    private bool CanRemovePredecessor()
    {
      return SelectedPredecessor != null;
    }

    #endregion

    #region Edit Predecessor

    public ICommand EditPredecessorCommand => _editPredecessorCommand ??= new ActionCommand(EditPredecessor, CanEditPredecessor);

    private void EditPredecessor()
    {
      using var vm = new EditRelationshipViewModel(_activities)
      {
        SelectedActivity1 = SelectedPredecessor.Activity,
        SelectedActivity2 = SelectedActivity,
        Lag = SelectedPredecessor.Lag,
        SelectedRelationshipType = SelectedPredecessor.RelationshipType
      };
      if (ViewFactory.Instance.ShowDialog(vm) == true)
      {
        SelectedPredecessor.Lag = vm.Lag;
        SelectedPredecessor.RelationshipType = vm.SelectedRelationshipType;
      }
    }

    private bool CanEditPredecessor()
    {
      return SelectedPredecessor != null;
    }

    #endregion

    #region Goto Predecessor

    public ICommand GotoPredecessorCommand => _gotoPredecessorCommand ??= new ActionCommand(GotoPredecessor, CanGotoPredecessor);

    private void GotoPredecessor()
    {
      SelectedActivity = SelectedPredecessor.Activity;
    }

    private bool CanGotoPredecessor()
    {
      return SelectedPredecessor != null;
    }

    #endregion

    #region Add Successor

    public ICommand AddSuccessorCommand => _addSuccessorCommand ??= new ActionCommand(AddSuccessor, CanAddSuccessor);

    private void AddSuccessor()
    {
      var otherActivities = _activities.Where(x => x != SelectedActivity && !Successors.Select(y => y.Activity).Contains(x));
      using var vm = new SelectActivityViewModel(otherActivities);
      if (ViewFactory.Instance.ShowDialog(vm) == true)
      {
        if (vm.SelectedActivity == SelectedActivity)
        {
          UserNotificationService.Instance.Error(NASResources.MessageCircularDependency);
        }
        else
        {
          var relationship = new Relationship(SelectedActivity, vm.SelectedActivity);
          var newVM = new LogicRelatedViewModel(vm.SelectedActivity, relationship);
          Successors.Add(newVM);
          SelectedSuccessor = newVM;
        }
      }
    }

    private bool CanAddSuccessor()
    {
      return SelectedActivity != null;
    }

    #endregion

    #region Remove Successor

    public ICommand RemoveSuccessorCommand => _removeSuccessorCommand ??= new ActionCommand(RemoveSuccessor, CanRemoveSuccessor);

    private void RemoveSuccessor()
    {
      UserNotificationService.Instance.Question(NASResources.MessageDeleteRelationship, () =>
      {
        _relationships.Remove(SelectedSuccessor.Relationship);
      });
    }

    private bool CanRemoveSuccessor()
    {
      return SelectedSuccessor != null;
    }

    #endregion

    #region Edit Successor

    public ICommand EditSuccessorCommand => _editSuccessorCommand ??= new ActionCommand(EditSuccessor, CanEditSuccessor);

    private void EditSuccessor()
    {
      using var vm = new EditRelationshipViewModel(_activities)
      {
        SelectedActivity1 = SelectedActivity,
        SelectedActivity2 = SelectedSuccessor.Activity,
        Lag = SelectedSuccessor.Lag,
        SelectedRelationshipType = SelectedSuccessor.RelationshipType
      };
      if (ViewFactory.Instance.ShowDialog(vm) == true)
      {
        SelectedSuccessor.Lag = vm.Lag;
        SelectedSuccessor.RelationshipType = vm.SelectedRelationshipType;
      }
    }

    private bool CanEditSuccessor()
    {
      return SelectedSuccessor != null;
    }

    #endregion

    #region Goto Successor

    public ICommand GotoSuccessorCommand => _gotoSuccessorCommand ??= new ActionCommand(GotoSuccessor, CanGotoSuccessor);

    private void GotoSuccessor()
    {
      SelectedActivity = SelectedSuccessor.Activity;
    }

    private bool CanGotoSuccessor()
    {
      return SelectedSuccessor != null;
    }

    #endregion

    #region Private Members

    private IEnumerable<Relationship> GetPreceedingRelationships(Activity activity)
    {
      return _relationships.Where(x => x.Activity2 == activity);
    }

    private IEnumerable<Relationship> GetSucceedingRelationships(Activity activity)
    {
      return _relationships.Where(x => x.Activity1 == activity);
    }

    #endregion

    #region Apply

    protected override void OnApply()
    {
      _schedule.Relationships.Clear();
      _schedule.Relationships.AddRange(_relationships);
      _schedule.Activities.Clear();
      _schedule.Activities.AddRange(_activities);
    }

    #endregion
  }
}
