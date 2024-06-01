namespace NAS.Models.Entities
{
  public abstract class Distortion : NASObject
  {
    public string _description;
    private Fragnet _fragnet;

    public Distortion(Activity activity)
    {
      Activity = activity;
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

    public Activity Activity { get; }

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
  }
}
