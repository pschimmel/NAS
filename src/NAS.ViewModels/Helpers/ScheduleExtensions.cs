using NAS.Models.Entities;

namespace NAS.ViewModels.Helpers
{
  public static class ScheduleExtensions
  {
    public static WBSItem FindWBSItem(this Schedule schedule, Guid id)
    {
      return FindWBSItem(schedule.WBSItem, id);
    }

    private static WBSItem FindWBSItem(WBSItem parent, Guid id)
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
        var foundItem = FindWBSItem(child, id);
        if (foundItem != null)
        {
          return foundItem;
        }
      }
      return null;
    }
  }
}
