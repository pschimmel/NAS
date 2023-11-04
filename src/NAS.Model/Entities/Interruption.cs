using System;

namespace NAS.Model.Entities
{
  public class Interruption : Distortion
  {
    private int? days;
    private DateTime? start;

    public Interruption()
    {
      start = DateTime.Today;
    }

    public int? Days
    {
      get => days;
      set
      {
        if (days != value)
        {
          days = value;
          OnPropertyChanged(nameof(Days));
        }
      }
    }

    public DateTime? Start
    {
      get => start;
      set
      {
        if (start != value)
        {
          start = value;
          OnPropertyChanged(nameof(Start));
        }
      }
    }
  }
}
