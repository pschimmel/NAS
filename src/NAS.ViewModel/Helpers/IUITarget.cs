using System;

namespace NAS.ViewModel.Helpers
{
  public interface IUITarget
  {
    void InvokeIfRequired(Action action, UIPriority priority);

    void BeginInvokeIfRequired(Action action, UIPriority priority);
  }
}
