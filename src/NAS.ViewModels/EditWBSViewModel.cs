using System.Windows;
using System.Windows.Input;
using ES.Tools.Core.MVVM;
using GongSolutions.Wpf.DragDrop;
using NAS.Models.Entities;
using NAS.Resources;
using NAS.ViewModels.Base;
using NAS.ViewModels.Helpers;

namespace NAS.ViewModels
{
  public class EditWBSViewModel : DialogContentViewModel, IDropTarget
  {
    #region Fields

    private readonly Schedule _schedule;
    private readonly WBSItemViewModel _wbsHierarchy;
    private Activity _currentWBSActivity;
    private ActionCommand _addWBSItemCommand;
    private ActionCommand _removeWBSItemCommand;
    private ActionCommand _editWBSItemCommand;
    private ActionCommand _moveWBSItemUpCommand;
    private ActionCommand _moveWBSItemDownCommand;
    private ActionCommand _moveWBSItemLeftCommand;
    private ActionCommand _moveWBSItemRightCommand;
    private ActionCommand _showWBSSummaryCommand;
    private ActionCommand _assignWBSCommand;
    private ActionCommand _unassignWBSCommand;

    #endregion

    #region Constructor

    public EditWBSViewModel(Schedule schedule)
      : base()
    {
      schedule.EnsureWBS();
      _schedule = schedule;
      _wbsHierarchy = CreateWBSViewModel(_schedule.WBSItem);
      RefreshChildren(_wbsHierarchy);
    }

    #endregion

    #region Overwritten Members

    public override string Title => NASResources.ProjectProperties;

    public override string Icon => "WBS";

    public override DialogSize DialogSize => DialogSize.Fixed(650, 400);

    public override HelpTopic HelpTopicKey => HelpTopic.WBS;

    #endregion

    #region Properties

    public List<WBSItemViewModel> WBS => [_wbsHierarchy];

    public WBSItemViewModel CurrentWBSItem
    {
      get => FindSelectedItem(_wbsHierarchy);
      private set
      {
        SelectWBSItem(_wbsHierarchy, value);
        OnPropertyChanged(nameof(CurrentWBSItem));
      }
    }

    public List<Activity> AllActivities => _schedule.Activities.ToList();

    public Activity CurrentWBSActivity
    {
      get => _currentWBSActivity;
      set
      {
        if (_currentWBSActivity != value)
        {
          _currentWBSActivity = value;
          OnPropertyChanged(nameof(CurrentWBSActivity));
        }
      }
    }

    #endregion

    #region Add WBS Item

    public ICommand AddWBSItemCommand => _addWBSItemCommand ??= new ActionCommand(AddWBSItem, CanAddWBSItem);

    private void AddWBSItem()
    {
      int newOrder = 0;
      if (CurrentWBSItem.Items.Count > 0)
      {
        newOrder = CurrentWBSItem.Items.Max(x => x.Order) + 1;
      }

      var newWBSItem = new WBSItem(CurrentWBSItem.Item)
      {
        Order = newOrder
      };

      using var vm = new EditWBSItemViewModel(newWBSItem);
      if (ViewFactory.Instance.ShowDialog(vm) == true && !string.IsNullOrWhiteSpace(vm.Name) && !string.IsNullOrWhiteSpace(vm.Number))
      {
        CurrentWBSItem.Item.Children.Add(newWBSItem);
        RefreshChildren(CurrentWBSItem);
        // The VM is recreated in this dialog. Search it and select it.
        var newItemVM = FindWBSItemModel(CurrentWBSItem, newWBSItem);
        CurrentWBSItem = newItemVM;
      }
    }

    private bool CanAddWBSItem()
    {
      return CurrentWBSItem != null;
    }

    #endregion

    #region Remove WBS Item

    public ICommand RemoveWBSItemCommand => _removeWBSItemCommand ??= new ActionCommand(RemoveWBSItem, CanRemoveWBSItem);

    private void RemoveWBSItem()
    {
      UserNotificationService.Instance.Question(NASResources.MessageDeleteWBS, () =>
      {
        var parent = CurrentWBSItem.Item.Parent;
        var parentVM = FindWBSItemModel(_wbsHierarchy, parent);
        parent.Children.Remove(CurrentWBSItem.Item);
        parentVM.Items.Remove(CurrentWBSItem);

        var list = parent.Children.OrderBy(x => x.Order).ToList();
        for (int i = 0; i < parent.Children.Count; i++)
        {
          list[i].Order = i;
        }
      });
    }

    private bool CanRemoveWBSItem()
    {
      return CurrentWBSItem != null && CurrentWBSItem.Item.Parent != null;
    }

    #endregion

    #region Edit WBS Item

    public ICommand EditWBSItemCommand => _editWBSItemCommand ??= new ActionCommand(EditWBSItem, CanEditWBSItem);

    private void EditWBSItem()
    {
      var vm = new EditWBSItemViewModel(CurrentWBSItem.Item);
      ViewFactory.Instance.ShowDialog(vm);
    }

    private bool CanEditWBSItem()
    {
      return CurrentWBSItem != null;
    }

    #endregion

    #region Move WBS Item Up

    public ICommand MoveWBSItemUpCommand => _moveWBSItemUpCommand ??= new ActionCommand(MoveWBSItemUp, CanMoveWBSItemUp);

    private void MoveWBSItemUp()
    {
      var item = CurrentWBSItem.Item;
      var parent = item.Parent;
      var previousItem = parent.Children.FirstOrDefault(x => x.Order == CurrentWBSItem.Order - 1);
      previousItem.Order++;
      item.Order--;
      var buffer = item;
      var parentVM = FindWBSItemModel(_wbsHierarchy, parent);
      RefreshChildren(parentVM);
      CurrentWBSItem = FindWBSItemModel(parentVM, buffer);
    }

    private bool CanMoveWBSItemUp()
    {
      return CurrentWBSItem != null &&
             CurrentWBSItem.Item.Parent != null &&
             CurrentWBSItem.Item.Parent.Children.FirstOrDefault(x => x.Order == CurrentWBSItem.Order - 1) != null;
    }

    #endregion

    #region Move WBS Item Down

    public ICommand MoveWBSItemDownCommand => _moveWBSItemDownCommand ??= new ActionCommand(MoveWBSItemDown, CanMoveWBSItemDown);

    private void MoveWBSItemDown()
    {
      var item = CurrentWBSItem.Item;
      var parent = item.Parent;
      var nextItem = parent.Children.FirstOrDefault(x => x.Order == CurrentWBSItem.Order + 1);
      nextItem.Order--;
      CurrentWBSItem.Order++;
      var buffer = item;
      var parentVM = FindWBSItemModel(_wbsHierarchy, parent);
      RefreshChildren(parentVM);
      CurrentWBSItem = FindWBSItemModel(parentVM, buffer);
    }

    private bool CanMoveWBSItemDown()
    {
      return CurrentWBSItem != null && CurrentWBSItem.Item.Parent != null && CurrentWBSItem.Item.Parent.Children.FirstOrDefault(x => x.Order == CurrentWBSItem.Order + 1) != null;
    }

    #endregion

    #region Move WBS Item Left

    public ICommand MoveWBSItemLeftCommand => _moveWBSItemLeftCommand ??= new ActionCommand(MoveWBSItemLeft, CanMoveWBSItemLeft);

    private void MoveWBSItemLeft()
    {
      var item = CurrentWBSItem.Item;
      var oldParent = item.Parent;
      var newParent = item.Parent.Parent;
      oldParent.Children.Remove(item);
      newParent.Children.Add(item);

      // Refresh order of remaining children
      var items = oldParent.Children.ToList();
      for (int i = 0; i < items.Count; i++)
      {
        items[i].Order = i;
      }

      // Item should be inserted below the old parent
      item.Order = oldParent.Order + 1;

      // Increase the order of all items below the old parent
      items = newParent.Children.Where(x => x.Order >= item.Order && x != item).OrderBy(x => x.Order).ToList();
      for (int i = 0; i < items.Count; i++)
      {
        items[i].Order++;
      }

      var buffer = item;
      var parentVM = FindWBSItemModel(_wbsHierarchy, newParent);
      RefreshChildren(parentVM);
      CurrentWBSItem = FindWBSItemModel(parentVM, buffer);
    }

    private bool CanMoveWBSItemLeft()
    {
      return CurrentWBSItem != null && CurrentWBSItem.Item.Parent != null && CurrentWBSItem.Item.Parent.Parent != null;
    }

    #endregion

    #region Move WBS Item Right

    public ICommand MoveWBSItemRightCommand => _moveWBSItemRightCommand ??= new ActionCommand(MoveWBSItemRight, CanMoveWBSItemRight);

    private void MoveWBSItemRight()
    {
      var item = CurrentWBSItem.Item;
      var newParent = item.Parent.Children.FirstOrDefault(x => x.Order == CurrentWBSItem.Order - 1) ?? item.Parent.Children.FirstOrDefault(x => x.Order == CurrentWBSItem.Order + 1);
      if (newParent == null)
      {
        return;
      }

      int newOrder = 0;
      if (newParent.Children.Any())
      {
        newOrder = newParent.Children.Max(x => x.Order) + 1;
      }

      var oldParent = item.Parent;
      oldParent.Children.Remove(item);
      newParent.Children.Add(item);
      var items = oldParent.Children.ToList();
      for (int i = 0; i < items.Count; i++)
      {
        items[i].Order = i;
      }

      item.Order = newOrder;
      var buffer = item;
      var parentVM = FindWBSItemModel(WBS.First(), oldParent);
      RefreshChildren(parentVM);
      CurrentWBSItem = FindWBSItemModel(parentVM, buffer);
    }

    private bool CanMoveWBSItemRight()
    {
      return CurrentWBSItem != null && CurrentWBSItem.Item.Parent != null && CurrentWBSItem.Item.Parent.Children.Count > 1;
    }

    #endregion

    #region Show WBS Summary

    public ICommand ShowWBSSummaryCommand => _showWBSSummaryCommand ??= new ActionCommand(ShowWBSSummary);

    private void ShowWBSSummary()
    {
      using var vm = new ShowWBSSummaryViewModel(_schedule);
      ViewFactory.Instance.ShowDialog(vm);
    }

    #endregion

    #region Assign WBS

    public ICommand AssignWBSCommand => _assignWBSCommand ??= new ActionCommand(AssignWBS, CanAssignWBS);

    private void AssignWBS()
    {
      var buffer = CurrentWBSActivity;
      CurrentWBSActivity.WBSItem = CurrentWBSItem.Item;
      CurrentWBSActivity = buffer;
    }

    private bool CanAssignWBS()
    {
      return CurrentWBSActivity != null && CurrentWBSItem != null;
    }

    #endregion

    #region Unassign WBS

    public ICommand UnassignWBSCommand => _unassignWBSCommand ??= new ActionCommand(UnassignWBS, CanUnassignWBS);

    private void UnassignWBS()
    {
      var buffer = CurrentWBSActivity;
      CurrentWBSActivity.WBSItem = null;
      CurrentWBSActivity = buffer;
    }

    private bool CanUnassignWBS()
    {
      return CurrentWBSActivity != null && CurrentWBSActivity.WBSItem != null;
    }

    #endregion

    #region Private Members

    private void RefreshChildren(WBSItemViewModel parent)
    {
      parent.Items.Clear();
      foreach (var childItem in parent.Item.Children.OrderBy(x => x.Order))
      {
        var childVM = CreateWBSViewModel(childItem);
        parent.Items.Add(childVM);
        RefreshChildren(childVM);
      }
    }

    private WBSItemViewModel CreateWBSViewModel(WBSItem item)
    {
      var itemViewModel = new WBSItemViewModel(item);
      itemViewModel.SelectionChanged += (sender, args) =>
      {
        if ((sender as WBSItemViewModel).IsSelected)
        {
          CurrentWBSItem = sender as WBSItemViewModel;
        }
      };
      return itemViewModel;
    }

    private static WBSItemViewModel FindWBSItemModel(WBSItemViewModel parent, WBSItem item)
    {
      if (parent.Item == item)
      {
        return parent;
      }

      foreach (var child in parent.Items)
      {
        var model = FindWBSItemModel(child, item);
        if (model != null)
        {
          return model;
        }
      }
      return null;
    }

    private static void SelectWBSItem(WBSItemViewModel wbs, WBSItemViewModel selectedItem)
    {
      foreach (var subItem in wbs.Items)
      {
        SelectWBSItem(subItem, selectedItem);
      }

      wbs.IsSelected = wbs == selectedItem;
    }

    private static WBSItemViewModel FindSelectedItem(WBSItemViewModel wbs)
    {
      if (wbs == null)
      {
        return null;
      }

      if (wbs.IsSelected)
      {
        return wbs;
      }

      foreach (var item in wbs.Items)
      {
        var childModel = FindSelectedItem(item);
        if (childModel != null)
        {
          return childModel;
        }
      }
      return null;
    }

    #endregion

    #region Drag and Drop

    public void DragOver(IDropInfo dropInfo)
    {
      if (dropInfo.Data is WBSItemViewModel sourceItem
        && dropInfo.TargetItem is WBSItemViewModel targetItem
        && sourceItem != targetItem)
      {
        var sourceParent = sourceItem.Item.Parent;
        var targetParent = targetItem.Item.Parent;

        // Cannot drop to topmost item
        if (targetParent == null)
        {
          return;
        }

        // Cannot drop to a child element
        if (IsChild(sourceItem, targetItem))
        {
          return;
        }

        // Cannot drop to same position
        if (sourceParent == targetParent &&
          targetItem.Order == sourceItem.Order + 1 &&
          dropInfo.InsertPosition == RelativeInsertPosition.BeforeTargetItem)
        {
          return;
        }

        dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
        dropInfo.Effects = DragDropEffects.Copy;
      }
    }

    private static bool IsChild(WBSItemViewModel parent, WBSItemViewModel child)
    {
      return parent.Items.Contains(child) || parent.Items.Any(x => IsChild(parent, x));
    }

    public void Drop(IDropInfo dropInfo)
    {
      var sourceItem = (dropInfo.Data as WBSItemViewModel).Item;
      var targetItem = (dropInfo.TargetItem as WBSItemViewModel).Item;

      var oldParent = sourceItem.Parent;
      var newParent = targetItem.Parent;

      oldParent.Children.Remove(sourceItem);

      var newSiblings = newParent.Children.OrderBy(x => x.Order).ToList();
      int newIndex = newSiblings.IndexOf(targetItem);

      if (dropInfo.InsertPosition == RelativeInsertPosition.AfterTargetItem)
      {
        newIndex++;
      }

      newParent.Children.Add(sourceItem);
      sourceItem.Order = newIndex;

      var items = oldParent.Children.ToList();
      for (int i = 0; i < items.Count; i++)
      {
        items[i].Order = i;
      }

      items = newParent.Children.OrderBy(x => x.Order).ToList();
      for (int i = 0; i < items.Count; i++)
      {
        items[i].Order = i;
      }

      var buffer = sourceItem;
      // RefreshWBS();
      CurrentWBSItem = FindWBSItemModel(_wbsHierarchy, buffer);
    }

    #endregion

    #region Apply

    protected override void OnApply()
    {
      base.OnApply();
      _schedule.WBSItem = AssignWBS(_wbsHierarchy);
    }

    private static WBSItem AssignWBS(WBSItemViewModel vm)
    {
      vm.Item.Children.Clear();

      foreach (var childVM in vm.Items.OrderBy(x => x.Order))
      {
        vm.Item.Children.Add(AssignWBS(childVM));
      }

      return vm.Item;
    }

    #endregion

    #region IDisposable

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);
      if (disposing)
      {
        _wbsHierarchy.Dispose();
      }
    }

    #endregion
  }
}
