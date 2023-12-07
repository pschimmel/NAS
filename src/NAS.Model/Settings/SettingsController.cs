namespace NAS.Model.Settings
{
  public class SettingsController
  {
    private static readonly Lazy<Settings> _lazySettings = new(() => LoadProgramSettings());
    private const int maxRecentFiles = 15;

    private SettingsController()
    { }

    private static Settings LoadProgramSettings()
    {
      return SettingsHelper.Load();
    }

    public static Settings Settings => _lazySettings.Value;

    public static void Save()
    {
      if (_lazySettings.IsValueCreated)
      {
        SettingsHelper.Save(Settings);
      }
    }

    public static void RemoveRecentlyOpenedFile(Guid id)
    {
      foreach (var schedule in Settings.RecentlyOpenedSchedules.Where(x => x.ID == id).ToList())
      {
        _ = Settings.RecentlyOpenedSchedules.Remove(schedule);
      }
    }

    public static void AddRecentlyOpenedFile(Guid id, string name)
    {
      RemoveRecentlyOpenedFile(id);
      Settings.RecentlyOpenedSchedules.Insert(0, new RecentSchedule() { ID = id, Name = name });
      while (Settings.RecentlyOpenedSchedules.Count > maxRecentFiles)
      {
        Settings.RecentlyOpenedSchedules.RemoveAt(Settings.RecentlyOpenedSchedules.Count - 1);
      }
    }
  }
}
