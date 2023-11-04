using System;

namespace NAS.ViewModel.Helpers
{
  /// <summary>
  /// Handles UI related notifications from the ViewModel.
  /// </summary>
  public class UITools
  {
    private static readonly Lazy<UITools> _lazy = new(new UITools());
    private IUITarget _uiTarget;

    private UITools()
    { }

    public static UITools Instance => _lazy.Value;

    /// <summary>
    /// Registers a target that handles UI notifications.
    /// </summary>
    public void RegisterTarget(IUITarget target)
    {
      if (_uiTarget != null)
      {
        throw new ApplicationException("Target is already registered!");
      }

      _uiTarget = target;
    }

    /// <summary>
    /// Removes a target.
    /// </summary>
    public void UnregisterTarget()
    {
      _uiTarget = null;
    }

    public void InvokeIfRequired(Action action, UIPriority priority = UIPriority.Medium)
    {
      if (_uiTarget == null)
      {
        throw new ApplicationException("Target is not registered!");
      }

      _uiTarget.InvokeIfRequired(action, priority);
    }

    public void BeginInvokeIfRequired(Action action, UIPriority priority = UIPriority.Medium)
    {
      if (_uiTarget == null)
      {
        throw new ApplicationException("Target is not registered!");
      }

      _uiTarget.BeginInvokeIfRequired(action, priority);
    }
  }
}
