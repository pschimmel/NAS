using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media;
using ES.Tools.Core.MVVM;
using NAS.Model.Entities;
using NAS.Model.Enums;
using NAS.Resources;
using NAS.ViewModel.Base;
using NAS.ViewModel.Helpers;

namespace NAS.ViewModel
{
  public class SortingAndGroupingViewModel : ViewModelBase, IApplyable
  {
    #region Fields

    private readonly Layout _layout;
    private SortingDefinition _currentSortingDefinition;
    private GroupingDefinition _currentGroupingDefinition;

    #endregion

    #region Constructor

    public SortingAndGroupingViewModel(Layout layout)
      : base()
    {
      _layout = layout;
      SortingDefinitions = new ObservableCollection<SortingDefinition>(_layout.SortingDefinitions);
      GroupingDefinitions = new ObservableCollection<GroupingDefinition>(_layout.GroupingDefinitions);
      AddSortingDefinitionCommand = new ActionCommand(AddSortingDefinitionCommandExecute);
      RemoveSortingDefinitionCommand = new ActionCommand(RemoveSortingDefinitionCommandExecute, () => RemoveSortingDefinitionCommandCanExecute);
      EditSortingDefinitionCommand = new ActionCommand(EditSortingDefinitionCommandExecute, () => EditSortingDefinitionCommandCanExecute);
      MoveSortingDefinitionUpCommand = new ActionCommand(MoveSortingDefinitionUpCommandExecute, () => MoveSortingDefinitionUpCommandCanExecute);
      MoveSortingDefinitionDownCommand = new ActionCommand(MoveSortingDefinitionDownCommandExecute, () => MoveSortingDefinitionDownCommandCanExecute);
      AddGroupingDefinitionCommand = new ActionCommand(AddGroupingDefinitionCommandExecute);
      RemoveGroupingDefinitionCommand = new ActionCommand(RemoveGroupingDefinitionCommandExecute, () => RemoveGroupingDefinitionCommandCanExecute);
      EditGroupingDefinitionCommand = new ActionCommand(EditGroupingDefinitionCommandExecute, () => EditGroupingDefinitionCommandCanExecute);
      MoveGroupingDefinitionUpCommand = new ActionCommand(MoveGroupingDefinitionUpCommandExecute, () => MoveGroupingDefinitionUpCommandCanExecute);
      MoveGroupingDefinitionDownCommand = new ActionCommand(MoveGroupingDefinitionDownCommandExecute, () => MoveGroupingDefinitionDownCommandCanExecute);
    }

    #endregion

    #region Public Properties

    public override HelpTopic HelpTopicKey => HelpTopic.SortAndGroup;

    public ObservableCollection<SortingDefinition> SortingDefinitions;

    public ObservableCollection<GroupingDefinition> GroupingDefinitions;

    public SortingDefinition CurrentSortingDefinition
    {
      get => _currentSortingDefinition;
      set
      {
        if (_currentSortingDefinition != value)
        {
          _currentSortingDefinition = value;
          OnPropertyChanged(nameof(CurrentSortingDefinition));
        }
      }
    }

    public GroupingDefinition CurrentGroupingDefinition
    {
      get => _currentGroupingDefinition;
      set
      {
        if (_currentGroupingDefinition != value)
        {
          _currentGroupingDefinition = value;
          OnPropertyChanged(nameof(CurrentGroupingDefinition));
        }
      }
    }

    #endregion

    #region Add Sorting Definition

    public ICommand AddSortingDefinitionCommand { get; }

    private void AddSortingDefinitionCommandExecute()
    {
      using var vm = new SortingDefinitionViewModel();
      if (ViewFactory.Instance.ShowDialog(vm) == true && vm.SelectedActivityProperty != ActivityProperty.None)
      {
        var newSortingDefinition = new SortingDefinition(vm.SelectedActivityProperty)
        {
          Direction = vm.SelectedSortDirection,
          Order = SortingDefinitions.Count,
        };
        SortingDefinitions.Add(newSortingDefinition);
        CurrentSortingDefinition = newSortingDefinition;
      }
    }

    #endregion

    #region Remove Sorting Definition

    public ICommand RemoveSortingDefinitionCommand { get; }

    private void RemoveSortingDefinitionCommandExecute()
    {
      UserNotificationService.Instance.Question(NASResources.MessageDeleteSortingDefinition, () =>
      {
        SortingDefinitions.Remove(CurrentSortingDefinition);
        CurrentSortingDefinition = null;
      });
    }

    private bool RemoveSortingDefinitionCommandCanExecute => CurrentSortingDefinition != null;

    #endregion

    #region Edit Sorting Definition

    public ICommand EditSortingDefinitionCommand { get; }

    private void EditSortingDefinitionCommandExecute()
    {
      var item = CurrentSortingDefinition;
      using var vm = new SortingDefinitionViewModel(item.Property, item.Direction);
      if (ViewFactory.Instance.ShowDialog(vm) == true && vm.SelectedActivityProperty != ActivityProperty.None)
      {
        item.Property = vm.SelectedActivityProperty;
        item.Direction = vm.SelectedSortDirection;
      }
    }

    private bool EditSortingDefinitionCommandCanExecute => CurrentSortingDefinition != null;

    #endregion

    #region Move Sorting Definition Up

    public ICommand MoveSortingDefinitionUpCommand { get; }

    private void MoveSortingDefinitionUpCommandExecute()
    {
      var previousItem = SortingDefinitions.FirstOrDefault(x => x.Order == CurrentSortingDefinition.Order - 1);
      if (previousItem != null)
      {
        previousItem.Order++;
        CurrentSortingDefinition.Order--;
      }
    }

    private bool MoveSortingDefinitionUpCommandCanExecute => CurrentSortingDefinition != null && CurrentSortingDefinition.Order > 0;

    #endregion

    #region Move Sorting Definition Down

    public ICommand MoveSortingDefinitionDownCommand { get; }

    private void MoveSortingDefinitionDownCommandExecute()
    {
      var nextItem = SortingDefinitions.FirstOrDefault(x => x.Order == CurrentSortingDefinition.Order + 1);
      if (nextItem != null)
      {
        nextItem.Order--;
        CurrentSortingDefinition.Order++;
      }
    }

    private bool MoveSortingDefinitionDownCommandCanExecute => CurrentSortingDefinition != null && CurrentSortingDefinition.Order < SortingDefinitions.Count - 1;

    #endregion

    #region Add Grouping Definition

    public ICommand AddGroupingDefinitionCommand { get; }

    private void AddGroupingDefinitionCommandExecute()
    {
      using var vm = new GroupingDefinitionViewModel();
      if (ViewFactory.Instance.ShowDialog(vm) == true && vm.SelectedActivityProperty != ActivityProperty.None)
      {
        var newGroupingDefinition = new GroupingDefinition(vm.SelectedActivityProperty)
        {
          Order = GroupingDefinitions.Count,
          Color = vm.SelectedColor.ToString()
        };
        CurrentGroupingDefinition = newGroupingDefinition;
      }
    }

    #endregion

    #region Remove Grouping Definition

    public ICommand RemoveGroupingDefinitionCommand { get; }

    private void RemoveGroupingDefinitionCommandExecute()
    {
      UserNotificationService.Instance.Question(NASResources.MessageDeleteGroupingDefinition, () =>
      {
        GroupingDefinitions.Remove(CurrentGroupingDefinition);
        CurrentGroupingDefinition = null;
      });
    }

    private bool RemoveGroupingDefinitionCommandCanExecute => CurrentGroupingDefinition != null;

    #endregion

    #region Edit Grouping Definition

    public ICommand EditGroupingDefinitionCommand { get; }

    private void EditGroupingDefinitionCommandExecute()
    {
      var item = CurrentGroupingDefinition;
      using var vm = new GroupingDefinitionViewModel(item.Property, (Color)ColorConverter.ConvertFromString(item.Color));
      if (ViewFactory.Instance.ShowDialog(vm) == true && vm.SelectedActivityProperty != ActivityProperty.None)
      {
        item.Property = vm.SelectedActivityProperty;
        item.Color = vm.SelectedColor.ToString();
      }
    }

    private bool EditGroupingDefinitionCommandCanExecute => CurrentGroupingDefinition != null;

    #endregion

    #region Move Sorting Definition Up

    public ICommand MoveGroupingDefinitionUpCommand { get; }

    private void MoveGroupingDefinitionUpCommandExecute()
    {
      var previousItem = GroupingDefinitions.FirstOrDefault(x => x.Order == CurrentGroupingDefinition.Order - 1);
      if (previousItem != null)
      {
        previousItem.Order++;
        CurrentGroupingDefinition.Order--;
      }
    }

    private bool MoveGroupingDefinitionUpCommandCanExecute => CurrentGroupingDefinition != null && CurrentGroupingDefinition.Order > 0;

    #endregion

    #region Move Sorting Definition Down

    public ICommand MoveGroupingDefinitionDownCommand { get; }

    private void MoveGroupingDefinitionDownCommandExecute()
    {
      var nextItem = GroupingDefinitions.FirstOrDefault(x => x.Order == CurrentGroupingDefinition.Order + 1);
      if (nextItem != null)
      {
        nextItem.Order--;
        CurrentGroupingDefinition.Order++;
      }
    }

    private bool MoveGroupingDefinitionDownCommandCanExecute => CurrentGroupingDefinition != null && CurrentGroupingDefinition.Order < GroupingDefinitions.Count - 1;

    #endregion

    #region IApplyable

    public void Apply()
    {
      _layout.RefreshSortingAndGrouping(SortingDefinitions, GroupingDefinitions);
    }

    #endregion
  }
}
