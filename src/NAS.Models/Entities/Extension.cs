namespace NAS.Models.Entities
{
  public class Extension : Distortion
  {
    private int? days;

    public Extension()
      : base()
    { }

    protected Extension(Extension other)
      : base(other)
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
      return new Extension(this);
    }
  }
}
