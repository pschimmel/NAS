
namespace NAS.Model.Entities
{
  public class Inhibition : Distortion
  {
    private double? percent;

    public double? Percent
    {
      get => percent;
      set
      {
        if (percent != value)
        {
          percent = value;
          OnPropertyChanged(nameof(Percent));
        }
      }
    }
  }
}
