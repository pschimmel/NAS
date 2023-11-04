using System;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NAS.Model.Scheduler
{
  public static class SchedulingSettingsHelper
  {
    public static string SaveSchedulingSettings(SchedulingSettings settings)
    {
      if (settings != null)
      {
        try
        {
          return JsonConvert.SerializeObject(settings, Formatting.Indented, new StringEnumConverter());
        }
        catch (Exception ex)
        {
          Debug.Fail(ex.Message);
        }
      }

      return null;
    }

    public static SchedulingSettings LoadSchedulingSettings(string serialized)
    {
      if (!string.IsNullOrWhiteSpace(serialized))
      {
        try
        {
          return JsonConvert.DeserializeObject<SchedulingSettings>(serialized, new StringEnumConverter());
        }
        catch (Exception ex)
        {
          Debug.Fail(ex.Message);
        }
      }

      return new SchedulingSettings();
    }
  }
}
