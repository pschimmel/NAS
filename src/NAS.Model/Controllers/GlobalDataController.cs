using System.IO;
using NAS.Model.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NAS.Model.Controllers
{
  public class GlobalDataController
  {
    private static readonly string _resourcesFileName;
    private static readonly string _PERTDefinitionsFileName;
    private static readonly Lazy<List<Resource>> _lazyResources = new(LoadResources);
    private static readonly Lazy<List<PERTDefinition>> _lazyPERTDefinitions = new(LoadPERTDefinitions);

    static GlobalDataController()
    {
      var path = Globals.GetGlobalStoragePath();
      var di = new DirectoryInfo(path);
      if (!di.Exists)
      {
        di.Create();
      }

      _resourcesFileName = Path.Combine(path, "NAS.Resources.json");
      _PERTDefinitionsFileName = Path.Combine(path, "NAS.PERTDefinitions.json");
    }

    public static List<Resource> Resources => _lazyResources.Value;

    private static List<Resource> LoadResources()
    {
      string json = File.ReadAllText(_resourcesFileName);
      return JsonConvert.DeserializeObject<List<Resource>>(json);
    }

    public static void SaveResources()
    {
      var settings = GetJsonSettings();
      string json = JsonConvert.SerializeObject(Resources, settings);
      File.WriteAllText(_resourcesFileName, json);
    }

    public static List<PERTDefinition> PERTDefinitions => _lazyPERTDefinitions.Value;

    private static List<PERTDefinition> LoadPERTDefinitions()
    {
      string json = File.ReadAllText(_PERTDefinitionsFileName);
      return JsonConvert.DeserializeObject<List<PERTDefinition>>(json);
    }

    public static void SavePERTDefinitions()
    {
      var settings = GetJsonSettings();
      string json = JsonConvert.SerializeObject(PERTDefinitions, settings);
      File.WriteAllText(_PERTDefinitionsFileName, json);
    }

    private static JsonSerializerSettings GetJsonSettings()
    {
      return new JsonSerializerSettings
      {
        Formatting = Formatting.Indented,
        Converters = new List<JsonConverter> { new StringEnumConverter() }
      };
    }
  }
}
