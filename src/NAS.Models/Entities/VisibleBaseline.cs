using NAS.Models.Base;

namespace NAS.Models.Entities
{
  public class VisibleBaseline : NASObject, IClonable<VisibleBaseline>
  {
    private string _color = "LightGray";

    public VisibleBaseline(Schedule schedule)
    {
      Schedule = schedule;
    }

    public VisibleBaseline(VisibleBaseline other)
      : this(other.Schedule)
    {
      _color = other.Color;
    }

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

    public VisibleBaseline Clone()
    {
      return new VisibleBaseline(this);
    }
  }
}
