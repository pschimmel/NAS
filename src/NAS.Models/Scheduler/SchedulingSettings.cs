using NAS.Models.Enums;

namespace NAS.Models.Scheduler
{
  public class SchedulingSettings
  {
    public CriticalPathDefinition CriticalPath { get; set; } = CriticalPathDefinition.FloatZeroOrLess;
  }
}
