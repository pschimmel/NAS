namespace NAS.Model.Entities
{
  public class VisibleBaseline : NASObject
  {
    private string color;

    internal VisibleBaseline()
    {
      color = "LightGray";
    }

    public VisibleBaseline(VisibleBaseline other)
      : this(other.Layout, other.Schedule)
    { }

    public VisibleBaseline(Layout layout, Schedule schedule)
      : this()
    {
      Layout = layout;
      Layout_ID = layout.ID;
      Schedule = schedule;
      Schedule_ID = schedule.ID;
    }

    public string Color
    {
      get => color;
      set
      {
        if (color != value)
        {
          color = value;
          OnPropertyChanged(nameof(Color));
        }
      }
    }

    public string Layout_ID { get; set; }

    public string Schedule_ID { get; set; }

    public virtual Layout Layout { get; set; }

    public virtual Schedule Schedule { get; set; }

    /// <summary>
    /// Returns activity <see cref="string"/> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="string"/> that represents this instance.
    /// </returns>
    public override string ToString()
    {
      return Schedule != null ? Schedule.Name : "VisibleBaseline";
    }
  }
}
