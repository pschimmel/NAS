using System.Collections.ObjectModel;
using System.Globalization;
using System.Resources;
using System.Xml.Serialization;
using NAS.Model.Enums;
using NAS.Resources;

namespace NAS.Model.Settings
{
  public class Settings
  {
    private readonly Lazy<List<string>> _lazyLanguages = new(InitLanguages);
    private string _language;

    public Theme Theme { get; set; } = Theme.Default;

    public bool ShowInstantHelpOnStartUp { get; set; } = true;

    public bool AutoCheckForUpdates { get; set; } = true;

    public string UserReportsFolderPath { get; set; }

    public string Language
    {
      get
      {
        if (string.IsNullOrWhiteSpace(_language))
        {
          var culture = CultureInfo.CurrentCulture;

          while (culture.Parent != null && culture.Parent.IsNeutralCulture)
          {
            culture = culture.Parent;
          }

          if (Languages.Contains(culture.NativeName))
          {
            _language = culture.NativeName;
          }
          else if (Languages.Count != 0)
          {
            Language = Languages.First();
          }
        }

        return _language;
      }
      set
      {
        if (Languages.Contains(value))
        {
          _language = value;
        }
      }
    }

    [XmlIgnore]
    public List<string> Languages => _lazyLanguages.Value;

    public ObservableCollection<RecentSchedule> RecentlyOpenedSchedules { get; } = [];

    private static List<string> InitLanguages()
    {
      var languages = new List<string>();
      var rm = new ResourceManager(typeof(NASResources));
      var cultures = CultureInfo.GetCultures(CultureTypes.NeutralCultures);
      languages.Add(new CultureInfo("en").NativeName);

      foreach (var cultureInfo in cultures)
      {
        if (!string.IsNullOrWhiteSpace(cultureInfo.Name))
        {
          var resourceSet = rm.GetResourceSet(cultureInfo, true, false);
          if (resourceSet != null && !languages.Contains(cultureInfo.NativeName))
          {
            languages.Add(cultureInfo.NativeName);
          }
        }
      }

      languages.Sort();
      return languages;
    }

    public static Settings Default => new();
  }
}