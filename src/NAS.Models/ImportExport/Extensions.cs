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
      if (string.IsNullOrWhiteSpace(node.InnerText))
      {
        return null;
      }

      try
      {
        // Input has format {255|}23|131|255
        short a = 255, r = 0, g = 0, b = 0;
        string[] array = node.InnerText.Split(new char[] { '|' }, 4, StringSplitOptions.RemoveEmptyEntries);

        if (array.Length is 3 or 4)
        {
          var idx = array.Length is 3 ? 0 : 1;
          if (short.TryParse(array[idx], out short x1) && short.TryParse(array[idx + 1], out short x2) && short.TryParse(array[idx + 2], out short x3))
          {
            r = x1;
            g = x2;
            b = x3;
          }

          if (array.Length == 4 && short.TryParse(array[0], out short x0))
          {
            a = x0;
          }

          // Format: #FF000000
          return "#" + a.ToString("X2") + r.ToString("X2") + g.ToString("X2") + b.ToString("X2");
        }

        return ColorConverter.ConvertFromString(node.InnerText).ToString();
      }
      catch
      {
        return null;
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
