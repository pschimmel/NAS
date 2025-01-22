using NAS.Models.Entities;
using NAS.ViewModels.Base;

namespace NAS.ViewModels.Helpers
{
  public static class LayoutVMFactory
  {
    public static IVisibleLayoutViewModel CreateVM(Schedule schedule, Layout layout)
    {
      return layout.LayoutType switch
      {
        Models.Enums.LayoutType.Gantt => new GanttLayoutViewModel(schedule, layout as GanttLayout),
        Models.Enums.LayoutType.PERT => new PERTLayoutViewModel(schedule, layout as PERTLayout),
        _ => throw new ArgumentException($"Wrong layout type {layout.LayoutType}."),
      };
    }
  }
}
