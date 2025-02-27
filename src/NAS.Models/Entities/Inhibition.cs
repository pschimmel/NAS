namespace NAS.Models.Entities
{
  public class Inhibition : Distortion
  {
    private double? _percent;

    public Inhibition() : base()
    { }

    protected Inhibition(Inhibition other) : base(other)
    {
      _percent = other._percent;
    }

    public double? Percent
    {
      get => _percent;
      set
      {
        if (_percent != value)
        {
          _percent = value;
          OnPropertyChanged(nameof(Percent));
        }
      }
    }

    public override Distortion Clone()
    {
      return new Inhibition(this);
    }
  }
}
