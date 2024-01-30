namespace NAS.Model.Entities
{
  public class PERTActivityData : NASObject
  {
    private int locationX;
    private int locationY;

    public PERTActivityData()
    { }

    public PERTActivityData(PERTActivityData other)
    {
      ActivityID = other.ActivityID;
      ID = other.ID;
      LocationX = other.LocationX;
      LocationY = other.LocationY;
      Schedule = other.Schedule;
    }

    public virtual Schedule Schedule { get; set; }

    public Guid ActivityID { get; set; } = Guid.Empty;

    public int LocationX
    {
      get => locationX;
      set
      {
        if (locationX != value)
        {
          locationX = value;
          OnPropertyChanged(nameof(LocationX));
        }
      }
    }

    public int LocationY
    {
      get => locationY;
      set
      {
        if (locationY != value)
        {
          locationY = value;
          OnPropertyChanged(nameof(LocationY));
        }
      }
    }

    public Activity GetActivity()
    {
      return Schedule.GetActivity(ActivityID);
    }
  }
}
