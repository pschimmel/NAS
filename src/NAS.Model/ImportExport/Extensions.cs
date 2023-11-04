using System.Windows.Media;
using System.Xml;

namespace NAS.Model.ImportExport
{
  internal static class Extensions
  {
    public static void AppendTextChild(this XmlElement parentElement, string name, object obj)
    {
      var newElement = parentElement.OwnerDocument.CreateElement(name);
      if (obj != null)
      {
        newElement.InnerText = obj is DateTime date ? date.ToString("yyyy-MM-dd") : obj.ToString();
        parentElement.AppendChild(newElement);
      }
    }

    internal static string GetColor(this XmlNode node)
    {
      try
      {
        return ColorConverter.ConvertFromString(node.InnerText).ToString();
      }
      catch
      {
        short a = 0, r = 0, g = 0, b = 0;
        if (node.InnerText != null)
        {
          string[] array = node.InnerText.Split(new char[] { '|' }, 4, StringSplitOptions.RemoveEmptyEntries);
          if (array.Length != 4)
          {
            return null;
          }

          if (!short.TryParse(array[0], out a))
          {
            return null;
          }

          if (!short.TryParse(array[1], out r))
          {
            return null;
          }

          if (!short.TryParse(array[2], out g))
          {
            return null;
          }

          if (!short.TryParse(array[3], out b))
          {
            return null;
          }
        }
        // #FF000000
        return "#" + a.ToString("X") + r.ToString("X") + g.ToString("X") + b.ToString("X");
      }
    }

    internal static DateTime? GetDateTime(this XmlNode node)
    {
      return DateTime.TryParse(node.InnerText, out var d) ? d : null;
    }

    internal static bool? GetBoolean(this XmlNode node)
    {
      return bool.TryParse(node.InnerText, out bool d) ? d : null;
    }

    internal static int? GetInteger(this XmlNode node)
    {
      return int.TryParse(node.InnerText, out int d) ? d : null;
    }

    internal static double? GetDouble(this XmlNode node)
    {
      return double.TryParse(node.InnerText, out double d) ? d : null;
    }

    internal static T? GetEnum<T>(this XmlNode node) where T : struct
    {
      return Enum.TryParse<T>(node.InnerText, out var d) ? d : null;
    }

    internal static decimal? GetDecimal(this XmlNode node)
    {
      return decimal.TryParse(node.InnerText, out decimal d) ? d : null;
    }
  }
}
