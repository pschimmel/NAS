using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace NAS.Model.Settings
{
  internal static class SettingsHelper
  {
    public static void Save(Settings settings)
    {
      try
      {
        var emptyNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
        var serializer = new XmlSerializer(typeof(Settings));
        var writerSettings = new XmlWriterSettings();
        writerSettings.Indent = true;
        writerSettings.OmitXmlDeclaration = true;

        Directory.CreateDirectory(Path.GetDirectoryName(Globals.SettingsFileName));
        using var writer = XmlWriter.Create(Globals.SettingsFileName, writerSettings);
        serializer.Serialize(writer, settings, emptyNamespaces);
      }
      catch (Exception ex)
      {
        Debug.Fail(ex.Message);
      }
    }

    public static Settings Load()
    {
      if (File.Exists(Globals.SettingsFileName))
      {
        try
        {
          var serializer = new XmlSerializer(typeof(Settings));
          using var reader = XmlReader.Create(Globals.SettingsFileName);
          var settings = (Settings)serializer.Deserialize(reader);
          if (settings != null)
          {
            return settings;
          }
        }
        catch (Exception ex)
        {
          Debug.Fail(ex.Message);
        }
      }
      return Settings.Default;
    }
  }
}
