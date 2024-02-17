namespace NAS.Model.Entities
{
  public class MaterialResource : Resource
  {
    private string _unit;

    public string Unit
    {
      get => _unit;
      set
      {
        if (_unit != value)
        {
          _unit = value;
          OnPropertyChanged(nameof(Unit));
        }
      }
    }
  }
}
