﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using NAS.Model.Entities;
using NAS.ViewModel.Base;
using NAS.ViewModel.Helpers;

namespace NAS.ViewModel
{
  public class WBSItemViewModel : ViewModelBase
  {
    public event EventHandler SelectionChanged;
    private ObservableCollection<WBSItemViewModel> items;
    private bool isExpanded;
    private bool isSelected;

    public WBSItemViewModel(WBSItem item)
    {
      isExpanded = true;
      Item = item;
      Item.PropertyChanged += (sender, e) =>
      {
        if (e.PropertyName == nameof(WBSItem.FullName))
        {
          OnPropertyChanged(nameof(FullName));
          RaiseChildPropertyChanged(Items, nameof(FullName));
        }
        else if (e.PropertyName == nameof(WBSItem.Order))
        {
          OnPropertyChanged(nameof(Order));
        }
      };
    }

    private void RaiseChildPropertyChanged(IEnumerable<WBSItemViewModel> items, string propertyName)
    {
      if (items == null || !items.Any())
      {
        return;
      }

      foreach (var item in items)
      {
        item.OnPropertyChanged(propertyName);
        RaiseChildPropertyChanged(item.Items, propertyName);
      }
    }

    public override HelpTopic HelpTopicKey => HelpTopic.WBS;

    public WBSItem Item { get; }

    public WBSItemViewModel Parent { get; private set; }

    public bool IsExpanded
    {
      get => isExpanded;
      set
      {
        if (isExpanded != value)
        {
          isExpanded = value;
          OnPropertyChanged(nameof(IsExpanded));
        }
      }
    }

    public bool IsSelected
    {
      get => isSelected;
      set
      {
        if (isSelected != value)
        {
          isSelected = value;
          OnPropertyChanged(nameof(IsSelected));
          SelectionChanged?.Invoke(this, EventArgs.Empty);
        }
      }
    }

    public string FullName => Item.FullName;

    public int Order
    {
      get => Item.Order;
      set
      {
        if (Item.Order != value)
        {
          Item.Order = value;
          OnPropertyChanged(nameof(Order));
        }
      }
    }

    public ObservableCollection<WBSItemViewModel> Items
    {
      get
      {
        if (items == null)
        {
          items = new ObservableCollection<WBSItemViewModel>();
          items.CollectionChanged += (sender, e) =>
          {
            if (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Replace)
            {
              foreach (WBSItemViewModel item in e.NewItems)
              {
                item.Parent = this;
              }
            }
            if (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Replace)
            {
              foreach (WBSItemViewModel item in e.OldItems)
              {
                if (item.Parent == this)
                {
                  item.Parent = null;
                }
              }
            }
          };
        }
        return items;
      }
    }

    public override string ToString()
    {
      return FullName;
    }
  }
}
