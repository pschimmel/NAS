using System.IO;
using System.Reflection;

namespace NAS.Model
{
  public static class Globals
  {
    public const string ApplicationName = "Network Analysis System";

    public const string ApplicationShortName = "NASPro";

    public static Version Version => Assembly.GetEntryAssembly().GetName().Version;

    public static string CopyRight => "Engineering Solutions 2012-" + DateTime.Now.Year;

    public static string SettingsFileName => Path.Combine(GetStoragePath(), "NAS.Settings.xml");

    public static string ReportsFileName => "NAS.Reports.xml";

    public static string UserReportsPath => Path.Combine(GetStoragePath(), ReportsFileName);

    public static string ReportsPathName => "Reports";

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
