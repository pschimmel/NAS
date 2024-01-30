namespace NAS.Model.Entities
{
  public class CustomAttribute : NASObject
  {
    private string _name;

    public string Name
    {
      get => _name;
      set
      {
        if (_name != value)
        {
          _name = value;
          OnPropertyChanged(nameof(Name));
        }
      }
    }

    public override string ToString()
    {
      return _name;
    }
  }
}
