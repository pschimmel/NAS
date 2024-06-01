namespace NAS.Models.Entities
{
  public partial class Delay : Distortion
  {
    private int? _days;

    public Delay(Activity activity) : base(activity)
    { }

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
  }
}
