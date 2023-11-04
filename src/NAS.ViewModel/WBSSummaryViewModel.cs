using System.Collections.Generic;
using System.Linq;
using NAS.Model.Entities;
using NAS.ViewModel.Base;

namespace NAS.ViewModel
{
  public class WBSSummaryViewModel : ViewModelBase
  {
    #region Constructors

    public WBSSummaryViewModel(Schedule schedule)
    {
      Schedule = schedule;
    }

    #endregion

    #region Public Properties

    public Schedule Schedule { get; private set; }

    public List<WBSItem> WBSItems
    {
      get
      {
        if (Schedule == null)
        {
          return null;
        }

        var items = new List<WBSItem>();
        if (Schedule.WBSItem != null)
        {
          items.Add(Schedule.WBSItem);
          addSubItems(Schedule.WBSItem, items);
        }
        return items;
      }
    }

    private void addSubItems(WBSItem parent, List<WBSItem> items)
    {
      foreach (var item in parent.Children)
      {
        items.Add(item);
        addSubItems(item, items);
      }
    }

    public List<WBSSummaryItem> WBSSummaryItems
    {
      get
      {
        var items = new List<WBSSummaryItem>();
        foreach (var item in WBSItems)
        {
          items.Add(new WBSSummaryItem(Schedule.Activities.ToList(), item));
        }

        return items;
      }
    }

    #endregion
  }
}
