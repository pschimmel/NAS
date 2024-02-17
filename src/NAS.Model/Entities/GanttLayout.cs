using NAS.Model.Enums;

namespace NAS.Model.Entities
{
  public class GanttLayout : Layout
  {
    public GanttLayout()
    { }

    public GanttLayout(GanttLayout other)
      : base(other)
    { }

    public override LayoutType LayoutType => LayoutType.Gantt;
  }
}
