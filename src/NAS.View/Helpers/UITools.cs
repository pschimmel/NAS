using System;
using System.Windows.Threading;
using ES.Tools.UI;
using NAS.ViewModel.Helpers;

namespace NAS.View.Helpers
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
      switch (priority)
      {
        case UIPriority.High:
          return DispatcherPriority.Render;
        case UIPriority.Medium:
          return DispatcherPriority.Normal;
        case UIPriority.Low:
          return DispatcherPriority.Background;
        default:
          throw new NotImplementedException();

      }
    }
  }
}
