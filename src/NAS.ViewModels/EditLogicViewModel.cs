using System.Collections.ObjectModel;
using System.Windows.Input;
using ES.Tools.Core.MVVM;
using NAS.Models.Entities;
using NAS.Resources;
using NAS.ViewModels.Base;
using NAS.ViewModels.Helpers;

namespace NAS.ViewModels
{
  public class EditLogicViewModel : ViewModelBase
  {
    #region Fields

    private Activity _currentActivity;
    private readonly Schedule _schedule;
    private LogicRelatedViewModel _currentPredecessor;
    private LogicRelatedViewModel _currentSuccessor;

    #endregion

    #region Constructor

    public EditLogicViewModel(Activity activity)
      : base()
    {
      _schedule = activity.Schedule;
      CurrentActivity = activity;
      AddPredecessorCommand = new ActionCommand(AddPredecessorCommandExecute, () => AddPredecessorCommandCanExecute);
      RemovePredecessorCommand = new ActionCommand(RemovePredecessorCommandExecute, () => RemovePredecessorCommandCanExecute);
      EditPredecessorCommand = new ActionCommand(EditPredecessorCommandExecute, () => EditPredecessorCommandCanExecute);
      GotoPredecessorCommand = new ActionCommand(GotoPredecessorCommandExecute, () => GotoPredecessorCommandCanExecute);
      AddSuccessorCommand = new ActionCommand(AddSuccessorCommandExecute, () => AddSuccessorCommandCanExecute);
      RemoveSuccessorCommand = new ActionCommand(RemoveSuccessorCommandExecute, () => RemoveSuccessorCommandCanExecute);
      EditSuccessorCommand = new ActionCommand(EditSuccessorCommandExecute, () => EditSuccessorCommandCanExecute);
      GotoSuccessorCommand = new ActionCommand(GotoSuccessorCommandExecute, () => GotoSuccessorCommandCanExecute);
    }

    #endregion

    #region Public Members

    public override HelpTopic HelpTopicKey => HelpTopic.Relationship;

    public Activity CurrentActivity
    {
      get => _currentActivity;
      private set
      {
        if (!ReferenceEquals(_currentActivity, value))
        {
          _currentActivity = value;
          Predecessors.Clear();
          Successors.Clear();
          if (_currentActivity != null)
          {
            foreach (var relationship in _currentActivity.GetPreceedingRelationships())
            {
              Predecessors.Add(new LogicRelatedViewModel(relationship.Activity1, relationship));
            }

            foreach (var relationship in _currentActivity.GetSucceedingRelationships())
            {
              Successors.Add(new LogicRelatedViewModel(relationship.Activity2, relationship));
            }
          }
        }
      }
    }

    public ObservableCollection<LogicRelatedViewModel> Predecessors { get; } = [];

    public LogicRelatedViewModel CurrentPredecessor
    {
      get => _currentPredecessor;
      set
      {
        if (_currentPredecessor != value)
        {
          _currentPredecessor = value;
          OnPropertyChanged(nameof(CurrentPredecessor));
        }
      }
    }

    public ObservableCollection<LogicRelatedViewModel> Successors { get; } = [];

    public LogicRelatedViewModel CurrentSuccessor
    {
      get => _currentSuccessor;
      set
      {
        if (_currentSuccessor != value)
        {
          _currentSuccessor = value;
          OnPropertyChanged(nameof(CurrentSuccessor));
        }
      }
    }

    #endregion

    #region Add Predecessor

    public ICommand AddPredecessorCommand { get; }

    private void AddPredecessorCommandExecute()
    {
      using var vm = new SelectActivityViewModel(_schedule);
      if (ViewFactory.Instance.ShowDialog(vm) == true)
      {
        if (vm.SelectedActivity == CurrentActivity)
        {
          UserNotificationService.Instance.Error(NASResources.MessageCircularDependency);
        }
        else
        {
          var relationship = _schedule.AddRelationship(vm.SelectedActivity, CurrentActivity);
          var newVM = new LogicRelatedViewModel(vm.SelectedActivity, relationship);
          Predecessors.Add(newVM);
          CurrentPredecessor = newVM;
        }
      }
    }

    private bool AddPredecessorCommandCanExecute => CurrentActivity != null;

    #endregion

    #region Remove Predecessor

    public ICommand RemovePredecessorCommand { get; }

    private void RemovePredecessorCommandExecute()
    {
      UserNotificationService.Instance.Question(NASResources.MessageDeleteRelationship, () =>
      {
        _schedule.RemoveRelationship(CurrentPredecessor.Relationship);
      });
    }

    private bool RemovePredecessorCommandCanExecute => CurrentPredecessor != null;

    #endregion

    #region Edit Predecessor

    public ICommand EditPredecessorCommand { get; }

    private void EditPredecessorCommandExecute()
    {
      using var vm = new EditRelationshipViewModel(CurrentActivity.Schedule)
      {
        SelectedActivity1 = CurrentPredecessor.Activity,
        SelectedActivity2 = CurrentActivity,
        Lag = CurrentPredecessor.Lag,
        SelectedRelationshipType = CurrentPredecessor.RelationshipType
      };
      if (ViewFactory.Instance.ShowDialog(vm) == true)
      {
        CurrentPredecessor.Lag = vm.Lag;
        CurrentPredecessor.RelationshipType = vm.SelectedRelationshipType;
      }
    }

    private bool EditPredecessorCommandCanExecute => CurrentPredecessor != null;

    #endregion

    #region Goto Predecessor

    public ICommand GotoPredecessorCommand { get; }

    private void GotoPredecessorCommandExecute()
    {
      CurrentActivity = CurrentPredecessor.Activity;
    }

    private bool GotoPredecessorCommandCanExecute => CurrentPredecessor != null;

    #endregion

    #region Add Successor

    public ICommand AddSuccessorCommand { get; }

    private void AddSuccessorCommandExecute()
    {
      using var vm = new SelectActivityViewModel(_schedule);
      if (ViewFactory.Instance.ShowDialog(vm) == true)
      {
        if (vm.SelectedActivity == CurrentActivity)
        {
          UserNotificationService.Instance.Error(NASResources.MessageCircularDependency);
        }
        else
        {
          var relationship = _schedule.AddRelationship(CurrentActivity, vm.SelectedActivity);
          var newVM = new LogicRelatedViewModel(vm.SelectedActivity, relationship);
          Successors.Add(newVM);
          CurrentSuccessor = newVM;
        }
      }
    }

    private bool AddSuccessorCommandCanExecute => CurrentActivity != null;

    #endregion

    #region Remove Successor

    public ICommand RemoveSuccessorCommand { get; }

    private void RemoveSuccessorCommandExecute()
    {
      UserNotificationService.Instance.Question(NASResources.MessageDeleteRelationship, () =>
      {
        _schedule.RemoveRelationship(CurrentSuccessor.Relationship);
      });
    }

    private bool RemoveSuccessorCommandCanExecute => CurrentSuccessor != null;

    #endregion

    #region Edit Successor

    public ICommand EditSuccessorCommand { get; }

    private void EditSuccessorCommandExecute()
    {
      using var vm = new EditRelationshipViewModel(CurrentActivity.Schedule)
      {
        SelectedActivity1 = CurrentActivity,
        SelectedActivity2 = CurrentSuccessor.Activity,
        Lag = CurrentSuccessor.Lag,
        SelectedRelationshipType = CurrentSuccessor.RelationshipType
      };
      if (ViewFactory.Instance.ShowDialog(vm) == true)
      {
        CurrentSuccessor.Lag = vm.Lag;
        CurrentSuccessor.RelationshipType = vm.SelectedRelationshipType;
      }
    }

    private bool EditSuccessorCommandCanExecute => CurrentSuccessor != null;

    #endregion

    #region Goto Successor

    public ICommand GotoSuccessorCommand { get; }

    private void GotoSuccessorCommandExecute()
    {
      CurrentActivity = CurrentSuccessor.Activity;
    }

    private bool GotoSuccessorCommandCanExecute => CurrentSuccessor != null;

    #endregion
  }
}
