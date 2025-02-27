namespace NAS.Models.Entities
{
  public class Delay : Distortion
  {
    private int? _days;

    public Delay() : base()
    { }

    protected Delay(Delay other) : base(other)
    {
      _days = other._days;
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

    public override Distortion Clone()
    {
      return new Delay(this);
    }
  }
}
