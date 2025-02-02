using System.Collections.ObjectModel;
using System.Windows.Input;
using ES.Tools.Core.MVVM;
using NAS.Models.Entities;
using NAS.ViewModels.Base;

namespace NAS.ViewModels
{
  public class SelectWBSViewModel : ViewModelBase
  {
    #region Fields

    private WBSItemViewModel _wbsRoot;

    #endregion

    #region Constructor

    public SelectWBSViewModel(Activity activity)
      : base()
    {
      Schedule = activity.Schedule;
      CurrentActivity = activity;
      RemoveSelectionCommand = new ActionCommand(param => RemoveSelectionCommandExecute(), param => RemoveSelectionCommandCanExecute);
    }

    #endregion

    #region Properties

    public Activity CurrentActivity { get; private set; }

    public Schedule Schedule { get; private set; }

    public List<WBSItemViewModel> WBS
    {
      get
      {
        if (_wbsRoot == null)
        {
          if (Schedule.WBSItem == null)
          {
            var wbs = new WBSItem
            {
              Name = Schedule.Name,
              Number = "1"
            };
            Schedule.WBSItem = wbs;
          }
          RefreshWBS();
        }
        return [_wbsRoot];
      }
    }

    public WBSItemViewModel CurrentWBSItem
    {
      get => FindSelectedItem(_wbsRoot);
      private set
      {
        SelectWBSItem(_wbsRoot, value);
        OnPropertyChanged(nameof(CurrentWBSItem));
      }
    }

    #endregion

    #region Remove Selection

    public ICommand RemoveSelectionCommand { get; }

    private void RemoveSelectionCommandExecute()
    {
      DeselectWBSItem(_wbsRoot);
    }

    private bool RemoveSelectionCommandCanExecute => CurrentActivity != null && CurrentActivity.WBSItem != null;

    #endregion

    #region Private Members

    private void RefreshWBS()
    {
      _wbsRoot = GetWBSModel(Schedule.WBSItem);
      OnPropertyChanged(nameof(WBS));
    }

    private WBSItemViewModel GetWBSModel(WBSItem item)
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
        var subModel = GetWBSModel(subItem);
        model.Items.Add(subModel);
      }
      item.Children.CollectionChanged += (sender, e) =>
      {
        if (e.Action is NotifyCollectionChangedAction.Add or NotifyCollectionChangedAction.Replace)
        {
          var list = model.Items.ToList();
          model.Items.Clear();
          foreach (object newItem in e.NewItems)
          {
            list.Add(GetWBSModel(newItem as WBSItem));
          }
          foreach (var listItem in list.OrderBy(x => x.Order))
          {
            model.Items.Add(listItem);
          }
        }
        if (e.Action is NotifyCollectionChangedAction.Remove or NotifyCollectionChangedAction.Replace)
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

    private void DeselectWBSItem(WBSItemViewModel wbs)
    {
      wbs.IsSelected = false;
      foreach (var child in wbs.Items)
      {
        DeselectWBSItem(child);
      }
    }

    private WBSItemViewModel FindSelectedItem(WBSItemViewModel wbs)
    {
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

    #region IDisposable

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);
      if (disposing)
      {
        _wbsRoot.Dispose();
      }
    }

    #endregion
  }
}
