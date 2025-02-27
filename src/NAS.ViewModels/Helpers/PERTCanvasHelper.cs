using NAS.Models.Entities;

namespace NAS.ViewModels.Helpers
{
  public static class PERTCanvasHelper
  {
    public static void ResetActivityPositions(Schedule schedule)
    {
      var activities = schedule.Activities.OrderBy(x => x.StartDate);
      var matrix = new PERTActivityData[activities.Count(), activities.Count()];
      var usedActivities = new List<Activity>();

      foreach (var item in activities.Where(x => schedule.GetVisiblePredecessorCount(x) == 0))
      {
        ArrangeActivity(schedule, matrix, item, usedActivities, 0, 0);
      }

      for (int x = 0; x < matrix.GetLength(0); x++)
      {
        for (int y = 0; y < matrix.GetLength(1); y++)
        {
          if (matrix[x, y] != null)
          {
            matrix[x, y].LocationX = x;
            matrix[x, y].LocationY = y;
          }
        }
      }
    }

    private static void ArrangeActivity(Schedule schedule, PERTActivityData[,] matrix, Activity item, List<Activity> usedActivities, int requestedX, int requestedY)
    {
      var activityData = schedule.GetOrAddActivityData(item);

      int y = requestedY;
      if (!usedActivities.Contains(item))
      {
        while (y < 0 || matrix[requestedX, y] != null)
        {
          y++;
        }
        matrix[requestedX, y] = activityData;
        MovePredecessors(schedule, matrix, item, y - requestedY);
        usedActivities.Add(item);
      }
      int newY = requestedY - schedule.GetVisibleSuccessorCount(item) / 2;
      if (newY < 0)
      {
        newY = 0;
      }

      for (int i = 0; i < schedule.GetVisibleSuccessorCount(item); i++)
      {
        ArrangeActivity(schedule, matrix, schedule.GetVisibleSuccessors(item).ElementAt(i), usedActivities, requestedX + 1, newY + i);
      }
    }

    private static void MovePredecessors(Schedule schedule, PERTActivityData[,] matrix, Activity item, int amount)
    {
      if (amount <= 0)
      {
        return;
      }

      foreach (var predecessor in schedule.GetVisiblePredecessors(item))
      {
        var data = schedule.GetOrAddActivityData(predecessor);
        int x = data.LocationX;
        int y = data.LocationY;
        if (x > -1 && y > -1 && x < matrix.GetLength(0) - 1 && y + amount < matrix.GetLength(1) - 1 && matrix[x, y + amount] == null)
        {
          matrix[x, y + amount] = data;
          data.LocationX = x;
          data.LocationY = y + amount;
          matrix[x, y] = null;
          MovePredecessors(schedule, matrix, predecessor, amount);
        }
      }
    }
  }
}
