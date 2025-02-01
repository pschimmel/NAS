using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media;
using ES.Tools.Core.MVVM;
using NAS.Models.Entities;
using NAS.Models.Enums;
using NAS.Resources;
using NAS.ViewModels.Base;
using NAS.ViewModels.Helpers;

namespace NAS.ViewModels
{
  public class EditSortingAndGroupingViewModel : DialogContentViewModel
  {
    #region Fields

    private readonly Layout _layout;
    private SortingDefinition _currentSortingDefinition;
    private GroupingDefinition _currentGroupingDefinition;
    private ActionCommand _addSortingDefinitionCommand;
    private ActionCommand _removeSortingDefinitionCommand;
    private ActionCommand _editSortingDefinitionCommand;
    private ActionCommand _moveSortingDefinitionUpCommand;
    private ActionCommand _moveSortingDefinitionDownCommand;
    private ActionCommand _addGroupingDefinitionCommand;
    private ActionCommand _removeGroupingDefinitionCommand;
    private ActionCommand _editGroupingDefinitionCommand;
    private ActionCommand _moveGroupingDefinitionUpCommand;
    private ActionCommand _moveGroupingDefinitionDownCommand;

    #endregion

    #region Constructor

    public EditSortingAndGroupingViewModel(Layout layout)
      : base()
    {
      _layout = layout;
      SortingDefinitions = new ObservableCollection<SortingDefinition>();
      foreach (var sortingDefinition in layout.SortingDefinitions)
      {
        SortingDefinitions.Add(sortingDefinition.Clone());
      }

      GroupingDefinitions = new ObservableCollection<GroupingDefinition>();
      foreach (var groupingDefinition in layout.GroupingDefinitions)
      {
        GroupingDefinitions.Add(groupingDefinition.Clone());
      }
    }

    #endregion

    #region Overwritten Members

    public override string Title => NASResources.GroupAndSort;

    public override string Icon => "GroupAndSort";

    public override DialogSize DialogSize => DialogSize.Fixed(350, 400);

    public override HelpTopic HelpTopicKey => HelpTopic.GroupAndSort;

    #endregion

    #region Properties

    public ObservableCollection<SortingDefinition> SortingDefinitions { get; }

    public ObservableCollection<GroupingDefinition> GroupingDefinitions { get; }

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

    public ICommand AddSortingDefinitionCommand => _addSortingDefinitionCommand ??= new ActionCommand(AddSortingDefinition);

    private void AddSortingDefinition()
    {
      using var vm = new SelectSortingDefinitionViewModel();
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

    public ICommand RemoveSortingDefinitionCommand => _removeSortingDefinitionCommand ??= new ActionCommand(RemoveSortingDefinition, CanRemoveSortingDefinition);

    private void RemoveSortingDefinition()
    {
      UserNotificationService.Instance.Question(NASResources.MessageDeleteSortingDefinition, () =>
      {
        SortingDefinitions.Remove(CurrentSortingDefinition);
        CurrentSortingDefinition = null;
      });
    }

    private bool CanRemoveSortingDefinition()
    {
      return CurrentSortingDefinition != null;
    }

    #endregion

    #region Edit Sorting Definition

    public ICommand EditSortingDefinitionCommand => _editSortingDefinitionCommand ??= new ActionCommand(EditSortingDefinition, CanEditSortingDefinition);

    private void EditSortingDefinition()
    {
      var item = CurrentSortingDefinition;
      using var vm = new SelectSortingDefinitionViewModel(item.Property, item.Direction);
      if (ViewFactory.Instance.ShowDialog(vm) == true && vm.SelectedActivityProperty != ActivityProperty.None)
      {
        item.Property = vm.SelectedActivityProperty;
        item.Direction = vm.SelectedSortDirection;
      }
    }

    private bool CanEditSortingDefinition()
    {
      return CurrentSortingDefinition != null;
    }

    #endregion

    #region Move Sorting Definition Up

    public ICommand MoveSortingDefinitionUpCommand => _moveSortingDefinitionUpCommand ??= new ActionCommand(MoveSortingDefinitionUp, CanMoveSortingDefinitionUp);

    private void MoveSortingDefinitionUp()
    {
      var previousItem = SortingDefinitions.FirstOrDefault(x => x.Order == CurrentSortingDefinition.Order - 1);
      if (previousItem != null)
      {
        previousItem.Order++;
        CurrentSortingDefinition.Order--;
      }
    }

    private bool CanMoveSortingDefinitionUp()
    {
      return CurrentSortingDefinition != null && CurrentSortingDefinition.Order > 0;
    }

    #endregion

    #region Move Sorting Definition Down

    public ICommand MoveSortingDefinitionDownCommand => _moveSortingDefinitionDownCommand ??= new ActionCommand(MoveSortingDefinitionDown, CanMoveSortingDefinitionDown);

    private void MoveSortingDefinitionDown()
    {
      var nextItem = SortingDefinitions.FirstOrDefault(x => x.Order == CurrentSortingDefinition.Order + 1);
      if (nextItem != null)
      {
        nextItem.Order--;
        CurrentSortingDefinition.Order++;
      }
    }

    private bool CanMoveSortingDefinitionDown()
    {
      return CurrentSortingDefinition != null && CurrentSortingDefinition.Order < SortingDefinitions.Count - 1;
    }

    #endregion

    #region Add Grouping Definition

    public ICommand AddGroupingDefinitionCommand => _addGroupingDefinitionCommand ??= new ActionCommand(AddGroupingDefinition);

    private void AddGroupingDefinition()
    {
      using var vm = new SelectGroupingDefinitionViewModel();
      if (ViewFactory.Instance.ShowDialog(vm) == true && vm.SelectedActivityProperty != ActivityProperty.None)
      {
        var newGroupingDefinition = new GroupingDefinition(vm.SelectedActivityProperty)
        {
          Order = GroupingDefinitions.Count,
          Color = vm.SelectedColor.ToString()
        };
        GroupingDefinitions.Add(newGroupingDefinition);
        CurrentGroupingDefinition = newGroupingDefinition;
      }
    }

    #endregion

    #region Remove Grouping Definition

    public ICommand RemoveGroupingDefinitionCommand => _removeGroupingDefinitionCommand ??= new ActionCommand(RemoveGroupingDefinition, CanRemoveGroupingDefinition);

    private void RemoveGroupingDefinition()
    {
      UserNotificationService.Instance.Question(NASResources.MessageDeleteGroupingDefinition, () =>
      {
        GroupingDefinitions.Remove(CurrentGroupingDefinition);
        CurrentGroupingDefinition = null;
      });
    }

    private bool CanRemoveGroupingDefinition()
    {
      return CurrentGroupingDefinition != null;
    }

    #endregion

    #region Edit Grouping Definition

    public ICommand EditGroupingDefinitionCommand => _editGroupingDefinitionCommand ??= new ActionCommand(EditGroupingDefinition, CanEditGroupingDefinition);

    private void EditGroupingDefinition()
    {
      var item = CurrentGroupingDefinition;
      using var vm = new SelectGroupingDefinitionViewModel(item.Property, (Color)ColorConverter.ConvertFromString(item.Color));
      if (ViewFactory.Instance.ShowDialog(vm) == true && vm.SelectedActivityProperty != ActivityProperty.None)
      {
        item.Property = vm.SelectedActivityProperty;
        item.Color = vm.SelectedColor.ToString();
      }
    }

    private bool CanEditGroupingDefinition()
    {
      return CurrentGroupingDefinition != null;
    }

    #endregion

    #region Move Grouping Definition Up

    public ICommand MoveGroupingDefinitionUpCommand => _moveGroupingDefinitionUpCommand ??= new ActionCommand(MoveGroupingDefinitionUp, CanMoveGroupingDefinitionUp);

    private void MoveGroupingDefinitionUp()
    {
      var previousItem = GroupingDefinitions.FirstOrDefault(x => x.Order == CurrentGroupingDefinition.Order - 1);
      if (previousItem != null)
      {
        previousItem.Order++;
        CurrentGroupingDefinition.Order--;
      }
    }

    private bool CanMoveGroupingDefinitionUp()
    {
      return CurrentGroupingDefinition != null && CurrentGroupingDefinition.Order > 0;
    }

    #endregion

    #region Move Grouping Definition Down

    public ICommand MoveGroupingDefinitionDownCommand => _moveGroupingDefinitionDownCommand ??= new ActionCommand(MoveGroupingDefinitionDown, CanMoveGroupingDefinitionDown);

    private void MoveGroupingDefinitionDown()
    {
      var nextItem = GroupingDefinitions.FirstOrDefault(x => x.Order == CurrentGroupingDefinition.Order + 1);
      if (nextItem != null)
      {
        nextItem.Order--;
        CurrentGroupingDefinition.Order++;
      }
    }

    private bool CanMoveGroupingDefinitionDown()
    {
      return CurrentGroupingDefinition != null && CurrentGroupingDefinition.Order < GroupingDefinitions.Count - 1;
    }

    #endregion

    #region Apply

    protected override void OnApply()
    {
      _layout.SortingDefinitions.Clear();
      _layout.GroupingDefinitions.Clear();

      foreach (var sortingDefinition in SortingDefinitions)
      {
        _layout.SortingDefinitions.Add(sortingDefinition);
      }

      foreach (var groupingDefinition in GroupingDefinitions)
      {
        _layout.GroupingDefinitions.Add(groupingDefinition);
      }
    }

    #endregion
  }
}
