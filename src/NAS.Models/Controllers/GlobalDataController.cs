using System.IO;
using NAS.Models.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NAS.Models.Controllers
{
  public class GlobalDataController
  {
    #region Events

    public event EventHandler<ErrorEventArgs> Error;

    #endregion

    #region Fields

    private static readonly Lazy<GlobalDataController> _lazy = new(new GlobalDataController());
    private readonly string _resourcesFileName;
    private readonly string _PERTDefinitionsFileName;
    private readonly string _calendarsFileName;
    private readonly Lazy<List<Resource>> _lazyResources;
    private readonly Lazy<List<PERTDefinition>> _lazyPERTDefinitions;
    private readonly Lazy<List<Calendar>> _lazyCalendars;

    #endregion

    #region Constructors

    private GlobalDataController()
    {
      var path = Globals.GetGlobalStoragePath();
      var di = new DirectoryInfo(path);
      if (!di.Exists)
      {
        di.Create();
      }

      _resourcesFileName = Path.Combine(path, "NAS.Resources.json");
      _PERTDefinitionsFileName = Path.Combine(path, "NAS.PERTDefinitions.json");
      _calendarsFileName = Path.Combine(path, "NAS.Calendars.json");
      _lazyResources = new(LoadResources);
      _lazyPERTDefinitions = new(LoadPERTDefinitions);
      _lazyCalendars = new(LoadCalendars);
    }

    public static GlobalDataController Instance => _lazy.Value;

    #endregion

    #region Global Resources

    public static List<Resource> Resources => Instance._lazyResources.Value;

    private List<Resource> LoadResources()
    {
      return LoadList<Resource>(_resourcesFileName);
    }

    public static void SaveResources()
    {
      SaveList(Resources, Instance._resourcesFileName);
    }

    #endregion

    #region Global PERT Definitions

    public static List<PERTDefinition> PERTDefinitions => Instance._lazyPERTDefinitions.Value;

    private List<PERTDefinition> LoadPERTDefinitions()
    {
      return LoadList<PERTDefinition>(_PERTDefinitionsFileName);
    }

    public static void SavePERTDefinitions()
    {
      SaveList(PERTDefinitions, Instance._PERTDefinitionsFileName);
    }

    #endregion

    #region Global Calendars

    public static List<Calendar> Calendars => Instance._lazyCalendars.Value;

    private List<Calendar> LoadCalendars()
    {
      var list = LoadList<Calendar>(_calendarsFileName);
      list.ForEach(x => x.IsGlobal = true);
      return list;
    }

    public static void SaveCalendars()
    {
      SaveList(Calendars, Instance._calendarsFileName);
    }

    #endregion

    #region Private Members

    private static List<T> LoadList<T>(string fileName) where T : NASObject
    {
      if (File.Exists(fileName))
      {
        try
        {
          string json = File.ReadAllText(fileName);
          return JsonConvert.DeserializeObject<List<T>>(json);
        }
        catch (Exception ex)
        {
          Instance.Error?.Invoke(Instance, new ErrorEventArgs(ex));
        }
      }

      return [];
    }

    private static void SaveList<T>(List<T> list, string fileName)
    {
      var settings = GetJsonSettings();
      string json = JsonConvert.SerializeObject(list, settings);
      File.WriteAllText(fileName, json);
    }

    private static JsonSerializerSettings GetJsonSettings()
    {
      return new JsonSerializerSettings
      {
        Formatting = Formatting.Indented,
        Converters = new List<JsonConverter> { new StringEnumConverter() }
      };
    }

    #endregion
  }
}
