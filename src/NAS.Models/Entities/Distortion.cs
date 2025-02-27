namespace NAS.Models.Entities
{
  public abstract class Distortion : NASObject
  {
    public string _description;
    private Fragnet _fragnet;

    protected Distortion()
    { }

    protected Distortion(Distortion other)
    {
      _description = other._description;
      _fragnet = other._fragnet;
    }

    public string Description
    {
      get => _description;
      set
      {
        if (_description != value)
        {
          _description = value;
          OnPropertyChanged(nameof(Description));
        }
      }
    }

    public Fragnet Fragnet
    {
      get => _fragnet;
      set
      {
        if (_fragnet != value)
        {
          _fragnet = value;
          OnPropertyChanged(nameof(Fragnet));
        }
      }
    }

    public abstract Distortion Clone();
  }
}
