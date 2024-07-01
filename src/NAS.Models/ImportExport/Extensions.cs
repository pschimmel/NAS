using System.Windows.Media;
using System.Xml;

namespace NAS.Models.ImportExport
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
        // Format: #FF000000
        return "#" + a.ToString("X") + r.ToString("X") + g.ToString("X") + b.ToString("X");
      }
    }

    internal static bool TryGetColor(this XmlNode node, out string value)
    {
      try
      {
        var nullableValue = node.GetColor();
        value = nullableValue;
        return true;
      }
      catch
      {
        value = null;
      }

      return false;
    }

    internal static DateTime? GetDateTime(this XmlNode node)
    {
      return DateTime.TryParse(node.InnerText, out var d) ? d : null;
    }

    internal static bool TryGetDateTime(this XmlNode node, out DateTime value)
    {
      var nullableValue = node.GetDateTime();
      value = nullableValue ?? default;
      return nullableValue.HasValue;
    }

    internal static bool? GetBoolean(this XmlNode node)
    {
      return bool.TryParse(node.InnerText, out bool d) ? d : null;
    }

    internal static bool TryGetBoolean(this XmlNode node, out bool value)
    {
      var nullableValue = node.GetBoolean();
      value = nullableValue ?? default;
      return nullableValue.HasValue;
    }

    internal static int? GetInteger(this XmlNode node)
    {
      return int.TryParse(node.InnerText, out int d) ? d : null;
    }

    internal static bool TryGetInteger(this XmlNode node, out int value)
    {
      var nullableValue = node.GetInteger();
      value = nullableValue ?? default;
      return nullableValue.HasValue;
    }

    internal static double? GetDouble(this XmlNode node)
    {
      return double.TryParse(node.InnerText, out double d) ? d : null;
    }

    internal static bool TryGetDouble(this XmlNode node, out double value)
    {
      var nullableValue = node.GetDouble();
      value = nullableValue ?? default;
      return nullableValue.HasValue;
    }

    internal static T? GetEnum<T>(this XmlNode node) where T : struct
    {
      return Enum.TryParse<T>(node.InnerText, out var d) ? d : null;
    }

    internal static bool TryGetEnum<T>(this XmlNode node, out T value) where T : struct
    {
      var nullableValue = node.GetEnum<T>();
      value = nullableValue ?? default;
      return nullableValue.HasValue;
    }

    internal static decimal? GetDecimal(this XmlNode node)
    {
      return decimal.TryParse(node.InnerText, out decimal d) ? d : null;
    }

    internal static bool TryGetDecimal(this XmlNode node, out decimal value)
    {
      var nullableValue = node.GetDecimal();
      value = nullableValue ?? default;
      return nullableValue.HasValue;
    }
  }
}
