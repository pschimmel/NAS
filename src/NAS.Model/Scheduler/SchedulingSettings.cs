using NAS.Model.Enums;

namespace NAS.Model.Scheduler
{
  public class SchedulingSettings
  {
    public CriticalPathDefinition CriticalPath { get; set; } = CriticalPathDefinition.FloatZeroOrLess;
  }
}
