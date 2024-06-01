namespace NAS.Models.Entities
{
  public class VisibleBaseline : NASObject
  {
    private string _color = "LightGray";

    public VisibleBaseline(Layout layout, Schedule schedule)
    {
      Layout = layout;
      Schedule = schedule;
    }

    public VisibleBaseline(VisibleBaseline other)
      : this(other.Layout, other.Schedule)
    { }

    public string Color
    {
      get => _color;
      set
      {
        if (_color != value)
        {
          _color = value;
          OnPropertyChanged(nameof(Color));
        }
      }
    }

    public Layout Layout { get; }

    public Schedule Schedule { get; }

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
