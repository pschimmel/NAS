using NAS.ViewModels.Base;

namespace NAS.ViewModels.Helpers
{
  public static class LayoutVMFactory
  {
    public static IVisibleLayoutViewModel CreateVM(Models.Entities.Layout layout)
    {
      return layout.LayoutType switch
      {
        Models.Enums.LayoutType.Gantt => new GanttLayoutViewModel(layout),
        Models.Enums.LayoutType.PERT => new PERTLayoutViewModel(layout),
        _ => throw new ArgumentException($"Wrong Layout type {layout.LayoutType}."),
      };
    }
  }
}
