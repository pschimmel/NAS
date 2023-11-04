using System.IO;

namespace NAS.ViewModel
{
  public class StartupSettings
  {
    public FileInfo ImportFileName { get; set; }

    public string UserName { get; set; }

    public string Password { get; set; }

    public string ScheduleToOpen { get; set; }

    public override bool Equals(object obj)
    {
      if (!(obj is StartupSettings other))
      {
        return false;
      }

      return Equals(other.ImportFileName?.FullName ?? "", ImportFileName?.FullName ?? "") &&
        Equals(other.UserName, UserName) &&
        Equals(other.Password, Password) &&
        Equals(other.ScheduleToOpen, ScheduleToOpen);
    }

    public override int GetHashCode()
    {
      return (ImportFileName?.FullName ?? "", UserName, Password, ScheduleToOpen).GetHashCode();
    }
  }
}
