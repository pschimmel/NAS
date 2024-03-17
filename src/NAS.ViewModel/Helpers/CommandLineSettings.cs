namespace NAS.ViewModel.Helpers
{
  public class CommandLineSettings
  {
    private readonly Dictionary<CommandLineSettingsType, string> _settings = [];

    public void Add(CommandLineSettingsType type, string setting)
    {
      _settings.Add(type, setting);
    }

    public bool Get(CommandLineSettingsType type, out string value)
    {
      value = null;

      if (_settings.TryGetValue(type, out string value1))
      {
        value = value1;
        return true;
      }
      return false;
    }

    public enum CommandLineSettingsType { OpenFile }
  }
}
