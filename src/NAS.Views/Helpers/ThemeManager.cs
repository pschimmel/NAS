using System.Diagnostics;
using System.Windows;
using AvalonDock;
using ES.Tools.UI;
using NAS.Models.Enums;
using NAS.Resources;

namespace NAS.Views.Helpers
{
  public static class ThemeManager
  {
    public static string GetNameOfTheme(Theme theme)
    {
      return theme switch
      {
        Theme.Default => NASResources.Default,
        Theme.DefaultLight => $"{NASResources.Default} {NASResources.Light}",
        Theme.DefaultDark => $"{NASResources.Default} {NASResources.Dark}",
        Theme.AeroLight => $"Aero {NASResources.Light}",
        Theme.AeroDark => $"Aero {NASResources.Dark}",
        Theme.MetroLight => $"Metro {NASResources.Light}",
        Theme.MetroDark => $"Metro {NASResources.Dark}",
        _ => throw new ApplicationException($"Unknown theme {theme}."),
      };
    }

    public static void SetTheme(Theme theme, DockingManager dockingManager = null)
    {
      AvalonDock.Themes.Theme avalonTheme;
      ControlzEx.Theming.Theme fluentTheme;

      switch (theme)
      {
        case Theme.DefaultLight:
          avalonTheme = new AvalonDock.Themes.VS2010Theme();
          fluentTheme = ControlzEx.Theming.ThemeManager.Current.GetTheme("Light.Blue");
          break;
        case Theme.DefaultDark:
          avalonTheme = new AvalonDock.Themes.VS2010Theme();
          fluentTheme = ControlzEx.Theming.ThemeManager.Current.GetTheme("Dark.Blue");
          break;
        case Theme.AeroLight:
          avalonTheme = new AvalonDock.Themes.AeroTheme();
          fluentTheme = ControlzEx.Theming.ThemeManager.Current.GetTheme("Light.Orange");
          break;
        case Theme.AeroDark:
          avalonTheme = new AvalonDock.Themes.AeroTheme();
          fluentTheme = ControlzEx.Theming.ThemeManager.Current.GetTheme("Dark.Orange");
          break;
        case Theme.MetroLight:
          avalonTheme = new AvalonDock.Themes.MetroTheme();
          fluentTheme = ControlzEx.Theming.ThemeManager.Current.GetTheme("Light.Blue");
          break;
        case Theme.MetroDark:
          avalonTheme = new AvalonDock.Themes.MetroTheme();
          fluentTheme = ControlzEx.Theming.ThemeManager.Current.GetTheme("Dark.Blue");
          break;
        case Theme.Default:
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
