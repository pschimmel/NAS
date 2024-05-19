using System;
using System.Collections.Generic;
using System.Linq;

namespace NAS.ViewModel.Helpers
{
  public class InstantHelpManager
  {
    public event EventHandler HelpWindowsChanged;

    private static readonly Lazy<InstantHelpManager> _lazy = new(() => new InstantHelpManager());
    private readonly ISet<Type> _viewTypes;
    private readonly ISet<IHelpWindow> _activeViews;
    private HelpTopic _currentHelpTopic;

    private InstantHelpManager()
    {
      _viewTypes = new HashSet<Type>();
      _activeViews = new HashSet<IHelpWindow>();
    }

    public static InstantHelpManager Instance => _lazy.Value;

    public void Register(Type type)
    {
      if (type == null)
      {
        throw new ArgumentNullException(nameof(type), "Type cannot be empty.");
      }

      if (!typeof(IHelpWindow).IsAssignableFrom(type))
      {
        throw new ArgumentException($"Can only register types that implement {nameof(IHelpWindow)}.");
      }

      _viewTypes.Add(type);
    }

    public void ShowHelpWindow()
    {
      foreach (var viewType in _viewTypes.ToList())
      {
        var view = _activeViews.FirstOrDefault(y => y.GetType() == viewType);

        if (view != null)
        {
          view = (IHelpWindow)Activator.CreateInstance(viewType);
          _activeViews.Add(view);
          view.SelectTopic(_currentHelpTopic);
        }

        view.Closed += View_Closed;
        view.Show();
        HelpWindowsChanged?.Invoke(this, EventArgs.Empty);
      }
    }

    private void View_Closed(object sender, EventArgs e)
    {
      var view = sender as IHelpWindow;
      view.Closed -= View_Closed;

      if (_activeViews.Remove(view))
      {
        HelpWindowsChanged?.Invoke(this, EventArgs.Empty);
      }
    }

    public void HideHelpWindow()
    {
      bool changed = false;

      foreach (var view in _activeViews)
      {
        view.Closed -= View_Closed;
        if (_activeViews.Remove(view))
        {
          changed = true;
        }
      }

      if (changed)
      {
        HelpWindowsChanged?.Invoke(this, EventArgs.Empty);
      }
    }

    public void SetHelpTopic(HelpTopic helpTopicKey)
    {
      _currentHelpTopic = helpTopicKey;
      foreach (var view in _activeViews)
      {
        view.SelectTopic(_currentHelpTopic);
      }
    }

    public bool InstantHelpVisible => _activeViews.Count > 0;
  }
}
