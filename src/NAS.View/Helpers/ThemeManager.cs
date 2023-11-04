using System.Diagnostics;
using System.Windows;
using AvalonDock;
using ES.Tools.UI;
using NAS.Model.Enums;

namespace NAS.View.Helpers
{
  public static class ThemeManager
  {
    public static void SetTheme(Themes theme, DockingManager dockingManager = null)
    {
      AvalonDock.Themes.Theme avalonTheme;
      ControlzEx.Theming.Theme fluentTheme;

      switch (theme)
      {
        case Themes.StandardLight:
          avalonTheme = new AvalonDock.Themes.VS2010Theme();
          fluentTheme = ControlzEx.Theming.ThemeManager.Current.GetTheme("Light.Blue");
          break;
        case Themes.StandardDark:
          avalonTheme = new AvalonDock.Themes.VS2010Theme();
          fluentTheme = ControlzEx.Theming.ThemeManager.Current.GetTheme("Dark.Blue");
          break;
        case Themes.AeroLight:
          avalonTheme = new AvalonDock.Themes.AeroTheme();
          fluentTheme = ControlzEx.Theming.ThemeManager.Current.GetTheme("Light.Orange");
          break;
        case Themes.AeroDark:
          avalonTheme = new AvalonDock.Themes.AeroTheme();
          fluentTheme = ControlzEx.Theming.ThemeManager.Current.GetTheme("Dark.Orange");
          break;
        case Themes.MetroLight:
          avalonTheme = new AvalonDock.Themes.MetroTheme();
          fluentTheme = ControlzEx.Theming.ThemeManager.Current.GetTheme("Light.Blue");
          break;
        case Themes.MetroDark:
          avalonTheme = new AvalonDock.Themes.MetroTheme();
          fluentTheme = ControlzEx.Theming.ThemeManager.Current.GetTheme("Dark.Blue");
          break;
        case Themes.Default:
          avalonTheme = new AvalonDock.Themes.MetroTheme();
          fluentTheme = ControlzEx.Theming.ThemeManager.Current.GetTheme("Light.Blue.Colorful");
          break;
        default:
          throw new NotSupportedException($"{theme} not valid.");
      }

      Debug.Assert(avalonTheme != null);
      Debug.Assert(fluentTheme != null);

      DispatcherWrapper.Default.BeginInvokeIfRequired(() => ControlzEx.Theming.ThemeManager.Current.ChangeTheme(Application.Current, fluentTheme));

      if (dockingManager != null)
      {
        DispatcherWrapper.Default.BeginInvokeIfRequired(() => dockingManager.Theme = avalonTheme);
      }
    }
  }
}
