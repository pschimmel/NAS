namespace NAS.Model.Entities
{
  public class Interruption : Distortion
  {
    private int? _days;
    private DateTime? _start;

    public Interruption(Activity activity) : base(activity)
    {
      _start = DateTime.Today;
    }

    public int? Days
    {
      get => _days;
      set
      {
        if (_days != value)
        {
          _days = value;
          OnPropertyChanged(nameof(Days));
        }
      }
    }

    public DateTime? Start
    {
      get => _start;
      set
      {
        if (_start != value)
        {
          _start = value;
          OnPropertyChanged(nameof(Start));
        }
      }
    }
  }
}
