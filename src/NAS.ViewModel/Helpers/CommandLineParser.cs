namespace NAS.ViewModel.Helpers
{
  public static class CommandLineParser
  {
    public static CommandLineSettings GetStartupSettings()
    {
      var settings = new CommandLineSettings();
      ParseCommandLineArgs(Environment.GetCommandLineArgs(), settings);
      return settings;
    }

    private static void ParseCommandLineArgs(string[] args, CommandLineSettings settings)
    {
      // First arguement is the startup application path
      for (int i = 1; i < args.Length; i++)
      {
        if (args[i] == null)
        {
          continue;
        }

        switch (args[i])
        {
          case "--open":
          case "-o":
            if (i <= args.Length - 2 && !string.IsNullOrWhiteSpace(args[i + 1]))
            {
              settings.Add(CommandLineSettings.CommandLineSettingsType.OpenFile, args[i + 1]);
              i++; // Skip the following argument
            }
            break;
          default:
            // As backup use single argument as filename
            if (i == 1 && args.Length == 2)
            {
              settings.Add(CommandLineSettings.CommandLineSettingsType.OpenFile, args[i]);
            }

            break;
        }
      }
    }
  }
}
