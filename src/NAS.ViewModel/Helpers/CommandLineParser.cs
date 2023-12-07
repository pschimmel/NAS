using System.IO;

namespace NAS.ViewModel.Helpers
{
  public static class CommandLineParser
  {
    public static StartupSettings GetStartupSettings()
    {
      var settings = new StartupSettings();
      ParseCommandLineArgs(Environment.GetCommandLineArgs(), settings);
      return settings;
    }

    internal static void ParseCommandLineArgs(string[] args, StartupSettings settings)
    {
      if (args.Length == 2 && !string.IsNullOrWhiteSpace(args[1]))
      {
        settings.ImportFileName = new FileInfo(args[1]);
      }
      else
      {
        for (int i = 1; i < args.Length; i += 2)
        {
          if (!args[i].StartsWith("-"))
          {
            i--;
            continue;
          }
          else
          {
            settings.ScheduleToOpen = args[i] switch
            {
              "-o" => args[i + 1]?.Trim()?.Trim(new char[] { '"' }),
              _ => throw new NotImplementedException("Not recognized command line parameter."),
            };
          }
        }
      }
    }
  }
}
