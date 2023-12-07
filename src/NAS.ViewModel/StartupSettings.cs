using System.IO;

namespace NAS.ViewModel
{
  public class StartupSettings
  {
    public FileInfo ImportFileName { get; set; }

    public string ScheduleToOpen { get; set; }

    public override bool Equals(object obj)
    {
      return obj is StartupSettings other
             && Equals(other.ImportFileName?.FullName ?? "", ImportFileName?.FullName ?? "")
             && Equals(other.ScheduleToOpen, ScheduleToOpen);
    }

    public override int GetHashCode()
    {
      return new { FileName = ImportFileName?.FullName ?? "", ScheduleToOpen }.GetHashCode();
    }
  }
}
