
namespace NAS.Model.Entities
{
  public class MaterialResource : Resource
  {
    private string unit;

    public string Unit
    {
      get => unit;
      set
      {
        if (unit != value)
        {
          unit = value;
          OnPropertyChanged(nameof(Unit));
        }
      }
    }
  }
}
