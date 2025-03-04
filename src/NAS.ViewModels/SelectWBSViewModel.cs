using NAS.Models.Entities;
using NAS.Resources;
using NAS.ViewModels.Base;
using NAS.ViewModels.Helpers;

namespace NAS.ViewModels
{
  public class SelectWBSViewModel : DialogContentViewModel
  {
    #region Constructor

    public SelectWBSViewModel(Schedule schedule, WBSItem selection)
      : base()
    {
      if (schedule.WBSItem == null)
      {
        WBS = [];
        return;
      }

      var root = GetWBSViewModel(schedule.WBSItem);
      WBS = [root];
      SelectWBSItem(WBS, FindWBSViewModel(WBS, selection));
    }

    #endregion

    #region Overwritten Members

    public override string Title => NASResources.WBS;

    public override string Icon => "WBS";

    public override DialogSize DialogSize => DialogSize.Fixed(400, 350);

    public override HelpTopic HelpTopicKey => HelpTopic.WBS;

    public override IEnumerable<IButtonViewModel> Buttons
    {
      get
      {
        return new List<IButtonViewModel>
        {
          ButtonViewModel.CreateButton(NASResources.ResetSelection, RemoveSelection, CanRemoveSelection),
          ButtonViewModel.CreateCancelButton(),
          ButtonViewModel.CreateOKButton()
        };
      }
    }

    #endregion

    #region Properties

    public List<WBSItemViewModel> WBS { get; }

    public WBSItemViewModel SelectedWBSItem
    {
      get => FindSelectedItem(WBS);
      private set
      {
        SelectWBSItem(WBS, value);
        OnPropertyChanged(nameof(SelectedWBSItem));
      }
    }

    #endregion

    #region Remove Selection

    private void RemoveSelection()
    {
      DeselectWBSItems(WBS);
    }

    private bool CanRemoveSelection()
    {
      return SelectedWBSItem == null;
    }

    #endregion

    #region Private Members

    private WBSItemViewModel GetWBSViewModel(WBSItem item)
    {
      var vm = new WBSItemViewModel(item);
      vm.SelectionChanged += (sender, args) =>
      {
        if ((sender as WBSItemViewModel).IsSelected)
        {
          SelectedWBSItem = sender as WBSItemViewModel;
        }
      };
      foreach (var subItem in item.Children.OrderBy(x => x.Order))
      {
        var subModel = GetWBSViewModel(subItem);
        vm.Items.Add(subModel);
      }
      item.Children.CollectionChanged += (sender, e) =>
      {
        if (e.Action is NotifyCollectionChangedAction.Add or NotifyCollectionChangedAction.Replace)
        {
          var list = vm.Items.ToList();
          vm.Items.Clear();
          foreach (object newItem in e.NewItems)
          {
            list.Add(GetWBSViewModel(newItem as WBSItem));
          }
          foreach (var listItem in list.OrderBy(x => x.Order))
          {
            vm.Items.Add(listItem);
          }
        }
        if (e.Action is NotifyCollectionChangedAction.Remove or NotifyCollectionChangedAction.Replace)
        {
          var list = vm.Items.ToList();
          vm.Items.Clear();
          foreach (object oldItem in e.OldItems)
          {
            list.RemoveAll(x => x.Item == (WBSItem)oldItem);
          }
          foreach (var listItem in list)
          {
            vm.Items.Add(listItem);
          }
        }
      };
      return vm;
    }

    private static WBSItemViewModel FindWBSViewModel(IEnumerable<WBSItemViewModel> wbsItems, WBSItem item)
    {
      foreach (var vm in wbsItems)
      {
        if (vm.Item == item)
        {
          return vm;
        }

        var child = FindWBSViewModel(vm.Items, item);
        if (child != null)
        {
          return child;
        }
      }

      return null;
    }

    private static void SelectWBSItem(IEnumerable<WBSItemViewModel> wbsItems, WBSItemViewModel selectedItem)
    {
      foreach (var item in wbsItems)
      {
        item.IsSelected = item == selectedItem;
        SelectWBSItem(item.Items, selectedItem);
      }
    }

    private static void DeselectWBSItems(IEnumerable<WBSItemViewModel> wbsItems)
    {
      foreach (var item in wbsItems)
      {
        item.IsSelected = false;
        DeselectWBSItems(item.Items);
      }
    }

    private static WBSItemViewModel FindSelectedItem(IEnumerable<WBSItemViewModel> wbsItems)
    {
      foreach (var item in wbsItems)
      {
        if (item.IsSelected)
        {
          return item;
        }

        var child = FindSelectedItem(item.Items);
        if (child != null)
        {
          return child;
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
        WBS.ToList().ForEach(x => x.Dispose());
        WBS.Clear();
      }
    }

    #endregion
  }
}
