using NAS.Models.Entities;

namespace NAS.Models.Scheduler
{
  internal class PathItem
  {
    public PathItem(Activity activity)
    {
      ActivityID = activity.ID;
    }

    public Guid ActivityID { get; }

    public int Length { get; set; }

    public List<PathItem> Children { get; } = [];
  }
}
