using System.IO;
using System.Reflection;
using ES.Tools.Core.Infrastructure;
using NAS.Models.Controllers;

namespace NAS.Models
{
  public static class Globals
  {
    public const string ApplicationName = "Network Analysis System";

    public const string ApplicationShortName = "NAS";

    public static Version Version => Assembly.GetEntryAssembly().GetName().Version;

    public static string Copyright => "Engineering Solutions 2012-" + DateTime.Now.Year;

    public const string Website = "http://www.engineeringsolutions.de/";

    public static string SettingsFileName => Path.Combine(GetStoragePath(), "NAS.Settings.xml");

    public const string ReportDefinitionFileName = "NAS.Reports.xml";

    public static string UserReportsDefaultPath => Path.Combine(GetStoragePath(), "Reports");

    public static string IntegratedReportsPath => Path.Combine(ApplicationHelper.StartupPath, "Reports");

    public static string UserName => Environment.UserName;

    public static string GetGlobalStoragePath()
    {
      return GetStoragePath();
    }

    public static string GetStoragePath()
    {
      return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NAS");
    }
  }
}
