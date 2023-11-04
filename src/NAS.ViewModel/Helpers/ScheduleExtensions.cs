using NAS.Model.Entities;

namespace NAS.ViewModel.Helpers
{
  public static class ScheduleExtensions
  {
    public static WBSItem FindWBSItem(this Schedule schedule, string id)
    {
      return findWBSItem(schedule.WBSItem, id);
    }

    private static WBSItem findWBSItem(WBSItem parent, string id)
    {
      if (parent == null)
      {
        return null;
      }

      if (parent.ID == id)
      {
        return parent;
      }

      foreach (var child in parent.Children)
      {
        var foundItem = findWBSItem(child, id);
        if (foundItem != null)
        {
          return foundItem;
        }
      }
      return null;
    }
  }
}
