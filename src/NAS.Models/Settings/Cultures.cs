using System;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;

namespace NAS.Models.Controllers
{
  public static class Cultures
  {
    public static CultureInfo GetCultureInfoFromEnglishName(string englishName)
    {
      foreach (var info in CultureInfo.GetCultures(CultureTypes.AllCultures))
      {
        if (info.EnglishName == englishName)
        {
          return new CultureInfo(info.Name);
        }
      }
      return null;
    }

    public static CultureInfo GetCultureInfoFromNativeName(string nativeName)
    {
      foreach (var info in CultureInfo.GetCultures(CultureTypes.AllCultures))
      {
        if (info.NativeName == nativeName)
        {
          return new CultureInfo(info.Name);
        }
      }
      return null;
    }

    public static List<CultureInfo> GetInstalledCultures(Type resourceSetName)
    {
      var result = new List<CultureInfo>();
      var rm = new ResourceManager(resourceSetName);
      var cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
      foreach (var culture in cultures)
      {
        try
        {
          var rs = rm.GetResourceSet(culture, true, false);
          if (rs != null)
          {
            result.Add(culture);
          }
        }
        catch
        {
        }
      }
      return result;
    }
  }
}
