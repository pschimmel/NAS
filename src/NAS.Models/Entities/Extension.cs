namespace NAS.Models.Entities
{
  public partial class Extension : Distortion
  {
    private int? days;

    public Extension(Activity activity) : base(activity)
    { }

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
  }
}
