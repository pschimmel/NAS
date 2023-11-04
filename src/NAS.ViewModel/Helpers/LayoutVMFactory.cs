using System;
using NAS.ViewModel.Base;

namespace NAS.ViewModel.Helpers
{
  public static class LayoutVMFactory
  {
    public static IVisibleLayoutViewModel CreateVM(Model.Entities.Layout layout)
    {
      return layout.LayoutType switch
      {
        Model.Enums.LayoutType.Gantt => new GanttLayoutViewModel(layout),
        Model.Enums.LayoutType.PERT => new PERTLayoutViewModel(layout),
        _ => throw new ArgumentException($"Wrong Layout type {layout.LayoutType}."),
      };
    }
  }
}
