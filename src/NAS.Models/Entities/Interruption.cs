namespace NAS.Models.Entities
{
  public class Interruption : Distortion
  {
    private int? _days;
    private DateTime? _start;

    public Interruption() : base()
    {
      _start = DateTime.Today;
    }

    protected Interruption(Interruption other) : base(other)
    {
      _days = other._days;
      _start = other._start;
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

    public override Distortion Clone()
    {
      return new Interruption(this);
    }
  }
}
