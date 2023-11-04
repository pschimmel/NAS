
namespace NAS.Model.Entities
{
  public class Reduction : Distortion
  {
    private int? days;

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
