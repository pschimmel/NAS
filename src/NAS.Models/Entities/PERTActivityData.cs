namespace NAS.Models.Entities
{
  public class PERTActivityData : NASObject
  {
    private int _locationX;
    private int _locationY;

    public PERTActivityData(Activity activity)
    {
      Activity = activity;
    }

    public PERTActivityData(PERTActivityData other)
    {
      Activity = other.Activity;
      ID = other.ID;
      LocationX = other.LocationX;
      LocationY = other.LocationY;
    }

    public Activity Activity { get; }

    public int LocationX
    {
      get => _locationX;
      set
      {
        if (_locationX != value)
        {
          _locationX = value;
          OnPropertyChanged(nameof(LocationX));
        }
      }
    }

    public int LocationY
    {
      get => _locationY;
      set
      {
        if (_locationY != value)
        {
          _locationY = value;
          OnPropertyChanged(nameof(LocationY));
        }
      }
    }
  }
}
