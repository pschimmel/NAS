namespace NAS.Models.Entities
{
  public class Reduction : Distortion
  {
    private int? days;

    public Reduction() : base()
    { }

    protected Reduction(Reduction other) : base(other)
    {
      days = other.days;
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

    public override Distortion Clone()
    {
      return new Reduction(this);
    }
  }
}
