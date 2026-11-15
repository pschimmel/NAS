using System;
using System.Windows.Threading;
using ES.Tools.UI;
using NAS.ViewModels.Helpers;

namespace NAS.Views.Helpers
{
  public class UITarget : IUITarget
  {
    public void InvokeIfRequired(Action action, UIPriority priority)
    {
      DispatcherWrapper.Default.InvokeIfRequired(action, ConvertPriority(priority));
    }

    public void BeginInvokeIfRequired(Action action, UIPriority priority)
    {
      DispatcherWrapper.Default.BeginInvokeIfRequired(action, ConvertPriority(priority));
    }

    private static DispatcherPriority ConvertPriority(UIPriority priority)
    {
      return priority switch
      {
        UIPriority.High => DispatcherPriority.Render,
        UIPriority.Medium => DispatcherPriority.Normal,
        UIPriority.Low => DispatcherPriority.Background,
        _ => throw new NotImplementedException(),
      };
    }
  }
}
