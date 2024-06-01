namespace NAS.Models.Entities
{
  public class Inhibition : Distortion
  {
    private double? _percent;

    public Inhibition(Activity activity) : base(activity)
    { }

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
  }
}
