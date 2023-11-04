using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ES.Tools.Core.MVVM;
using GongSolutions.Wpf.DragDrop;
using NAS.Model.Entities;
using NAS.Resources;
using NAS.ViewModel.Base;
using NAS.ViewModel.Helpers;

namespace NAS.ViewModel
{
  public class WBSViewModel : ViewModelBase, IDropTarget, IApplyable
  {
    #region Fields

    private readonly Schedule _schedule;
    private WBSItemViewModel _root;
    private Activity _currentWBSActivity;

    #endregion

    #region Constructor

    public WBSViewModel(Schedule schedule)
      : base()
    {
      _schedule = schedule;
      EnsureWBS(schedule);
      RefreshWBS();
      AddWBSItemCommand = new ActionCommand(param => AddWBSItemCommandExecute(), param => AddWBSItemCommandCanExecute);
      RemoveWBSItemCommand = new ActionCommand(param => RemoveWBSItemCommandExecute(), param => RemoveWBSItemCommandCanExecute);
      EditWBSItemCommand = new ActionCommand(param => EditWBSItemCommandExecute(), param => EditWBSItemCommandCanExecute);
      MoveWBSItemUpCommand = new ActionCommand(param => MoveWBSItemUpCommandExecute(), param => MoveWBSItemUpCommandCanExecute);
      MoveWBSItemDownCommand = new ActionCommand(param => MoveWBSItemDownCommandExecute(), param => MoveWBSItemDownCommandCanExecute);
      MoveWBSItemLeftCommand = new ActionCommand(param => MoveWBSItemLeftCommandExecute(), param => MoveWBSItemLeftCommandCanExecute);
      MoveWBSItemRightCommand = new ActionCommand(param => MoveWBSItemRightCommandExecute(), param => MoveWBSItemRightCommandCanExecute);
      ShowWBSSummaryCommand = new ActionCommand(param => ShowWBSSummaryCommandExecute());
      AssignWBSCommand = new ActionCommand(param => AssignWBSCommandExecute(), param => AssignWBSCommandCanExecute);
      UnassignWBSCommand = new ActionCommand(param => UnassignWBSCommandExecute(), param => UnassignWBSCommandCanExecute);
    }

    #endregion

    #region Properties

    public override HelpTopic HelpTopicKey => HelpTopic.WBS;

    public List<WBSItemViewModel> WBS { get; }

    public WBSItemViewModel CurrentWBSItem
    {
      get => FindSelectedItem(_root);
      private set
      {
        SelectWBSItem(_root, value);
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

    public ICommand AddWBSItemCommand { get; }

    private void AddWBSItemCommandExecute()
    {
      int newOrder = 0;
      if (CurrentWBSItem.Items.Count > 0)
      {
        newOrder = CurrentWBSItem.Items.Max(x => x.Order) + 1;
      }

      var newWBSItem = new WBSItem
      {
        Parent = CurrentWBSItem.Item,
        Order = newOrder
      };

      using var vm = new WBSItemViewModel(newWBSItem);
      if (ViewFactory.Instance.ShowDialog(vm) == true)
      {
        CurrentWBSItem.Items.Add(vm);
        CurrentWBSItem = vm;
      }
    }

    private bool AddWBSItemCommandCanExecute => CurrentWBSItem != null;

    #endregion

    #region Remove WBS Item

    public ICommand RemoveWBSItemCommand { get; }

    private void RemoveWBSItemCommandExecute()
    {
      UserNotificationService.Instance.Question(NASResources.MessageDeleteWBS, () =>
      {
        var parent = CurrentWBSItem.Item.Parent;
        parent.Children.Remove(CurrentWBSItem.Item);
        CurrentWBSItem.Items.Remove(CurrentWBSItem);

        var list = parent.Children.OrderBy(x => x.Order).ToList();
        for (int i = 0; i < parent.Children.Count; i++)
        {
          list[i].Order = i;
        }
      });
    }

    private bool RemoveWBSItemCommandCanExecute => CurrentWBSItem != null && CurrentWBSItem.Parent != null;

    #endregion

    #region Edit WBS Item

    public ICommand EditWBSItemCommand { get; }

    private void EditWBSItemCommandExecute()
    {
      ViewFactory.Instance.ShowDialog(CurrentWBSItem);
    }

    private bool EditWBSItemCommandCanExecute => CurrentWBSItem != null;

    #endregion

    #region Move WBS Item Up

    public ICommand MoveWBSItemUpCommand { get; }

    private void MoveWBSItemUpCommandExecute()
    {
      var item = CurrentWBSItem.Item;
      var parent = item.Parent;
      var previousItem = parent.Children.FirstOrDefault(x => x.Order == CurrentWBSItem.Order - 1);
      previousItem.Order++;
      item.Order--;
      var buffer = item;
      RefreshWBS();
      CurrentWBSItem = FindWBSItemModel(_root, buffer);
    }

    private bool MoveWBSItemUpCommandCanExecute => CurrentWBSItem != null && CurrentWBSItem.Parent != null && CurrentWBSItem.Parent.Items.FirstOrDefault(x => x.Order == CurrentWBSItem.Order - 1) != null;

    #endregion

    #region Move WBS Item Down

    public ICommand MoveWBSItemDownCommand { get; }

    private void MoveWBSItemDownCommandExecute()
    {
      var item = CurrentWBSItem.Item;
      var parent = item.Parent;
      var nextItem = parent.Children.FirstOrDefault(x => x.Order == CurrentWBSItem.Order + 1);
      nextItem.Order--;
      CurrentWBSItem.Order++;
      var buffer = item;
      RefreshWBS();
      CurrentWBSItem = FindWBSItemModel(_root, buffer);
    }

    private bool MoveWBSItemDownCommandCanExecute => CurrentWBSItem != null && CurrentWBSItem.Parent != null && CurrentWBSItem.Parent.Items.FirstOrDefault(x => x.Order == CurrentWBSItem.Order + 1) != null;

    #endregion

    #region Move WBS Item Left

    public ICommand MoveWBSItemLeftCommand { get; }

    private void MoveWBSItemLeftCommandExecute()
    {
      var item = CurrentWBSItem.Item;
      var oldParent = item.Parent;
      var newParent = item.Parent.Parent;
      oldParent.Children.Remove(item);
      newParent.Children.Add(item);
      var items = oldParent.Children.ToList();
      for (int i = 0; i < items.Count; i++)
      {
        items[i].Order = i;
      }

      item.Order = oldParent.Order + 1;
      items = newParent.Children.Where(x => x.Order >= item.Order && x != item).OrderBy(x => x.Order).ToList();
      for (int i = 0; i < items.Count; i++)
      {
        items[i].Order++;
      }

      var buffer = item;
      RefreshWBS();
      CurrentWBSItem = FindWBSItemModel(_root, buffer);
    }

    private bool MoveWBSItemLeftCommandCanExecute => CurrentWBSItem != null && CurrentWBSItem.Parent != null && CurrentWBSItem.Parent.Parent != null;

    #endregion

    #region Move WBS Item Right

    public ICommand MoveWBSItemRightCommand { get; }

    private void MoveWBSItemRightCommandExecute()
    {
      var item = CurrentWBSItem.Item;
      var newParent = item.Parent.Children.FirstOrDefault(x => x.Order == CurrentWBSItem.Order - 1);
      if (newParent == null)
      {
        newParent = item.Parent.Children.FirstOrDefault(x => x.Order == CurrentWBSItem.Order + 1);
      }

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
      item.Parent.Children.Remove(item);
      newParent.Children.Add(item);
      var items = oldParent.Children.ToList();
      for (int i = 0; i < items.Count; i++)
      {
        items[i].Order = i;
      }

      item.Order = newOrder;
      var buffer = item;
      RefreshWBS();
      CurrentWBSItem = FindWBSItemModel(_root, buffer);
    }

    private bool MoveWBSItemRightCommandCanExecute => CurrentWBSItem != null && CurrentWBSItem.Parent != null && CurrentWBSItem.Parent.Items.Count > 1;

    #endregion

    #region Show WBS Summary

    public ICommand ShowWBSSummaryCommand { get; }

    private void ShowWBSSummaryCommandExecute()
    {
      using var vm = new WBSSummaryViewModel(_schedule);
      ViewFactory.Instance.ShowDialog(vm);
    }

    #endregion

    #region Assign WBS

    public ICommand AssignWBSCommand { get; }

    private void AssignWBSCommandExecute()
    {
      var buffer = CurrentWBSActivity;
      CurrentWBSActivity.WBSItem = CurrentWBSItem.Item;
      CurrentWBSActivity = buffer;
    }

    private bool AssignWBSCommandCanExecute => CurrentWBSActivity != null && CurrentWBSItem != null;

    #endregion

    #region Unassign WBS

    public ICommand UnassignWBSCommand { get; }

    private void UnassignWBSCommandExecute()
    {
      var buffer = CurrentWBSActivity;
      CurrentWBSActivity.WBSItem = null;
      CurrentWBSActivity = buffer;
    }

    private bool UnassignWBSCommandCanExecute => CurrentWBSActivity != null && CurrentWBSActivity.WBSItem != null;

    #endregion

    #region Private Members

    private void RefreshWBS()
    {
      _root = GetWBSViewModel(_schedule.WBSItem);
      OnPropertyChanged(nameof(WBS));
    }

    private WBSItemViewModel GetWBSViewModel(WBSItem item)
    {
      var model = new WBSItemViewModel(item);
      model.SelectionChanged += (sender, args) =>
      {
        if ((sender as WBSItemViewModel).IsSelected)
        {
          CurrentWBSItem = sender as WBSItemViewModel;
        }
      };
      foreach (var subItem in item.Children.OrderBy(x => x.Order))
      {
        var subModel = GetWBSViewModel(subItem);
        model.Items.Add(subModel);
      }
      (item.Children as ObservableCollection<WBSItem>).CollectionChanged += (sender, e) =>
      {
        if (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Replace)
        {
          var list = model.Items.ToList();
          model.Items.Clear();
          foreach (object newItem in e.NewItems)
          {
            list.Add(GetWBSViewModel(newItem as WBSItem));
          }
          foreach (var listItem in list.OrderBy(x => x.Order))
          {
            model.Items.Add(listItem);
          }
        }
        if (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Replace)
        {
          var list = model.Items.ToList();
          model.Items.Clear();
          foreach (object oldItem in e.OldItems)
          {
            list.RemoveAll(x => x.Item == (WBSItem)oldItem);
          }
          foreach (var listItem in list)
          {
            model.Items.Add(listItem);
          }
        }
      };
      return model;
    }

    private WBSItemViewModel FindWBSItemModel(WBSItemViewModel parent, WBSItem item)
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

    private void SelectWBSItem(WBSItemViewModel wbs, WBSItemViewModel selectedItem)
    {
      foreach (var subItem in wbs.Items)
      {
        SelectWBSItem(subItem, selectedItem);
      }

      wbs.IsSelected = wbs == selectedItem;
    }

    private WBSItemViewModel FindSelectedItem(WBSItemViewModel wbs)
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
        var sourceParent = sourceItem.Parent;
        var targetParent = targetItem.Parent;

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

    private bool IsChild(WBSItemViewModel parent, WBSItemViewModel child)
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
      RefreshWBS();
      CurrentWBSItem = FindWBSItemModel(_root, buffer);
    }

    private static void EnsureWBS(Schedule schedule)
    {
      if (schedule.WBSItem == null)
      {
        var item = new WBSItem();
        item.Name = schedule.Name;
        item.Number = "1";
        schedule.WBSItem = item;
      }
    }

    #endregion

    #region IApplyableViewModel Implementation

    public void Apply()
    {
      _schedule.WBSItem = AssignWBS(_root);
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
        _root.Dispose();
      }
    }

    #endregion
  }
}
