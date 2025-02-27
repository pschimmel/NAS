namespace NAS.Models.Entities
{
  public class VisibleBaseline : NASObject
  {
    #region Fields

    private string _color = "LightGray";

    #endregion

    #region Constructors

    public VisibleBaseline(Schedule schedule)
    {
      Schedule = schedule;
    }

    private VisibleBaseline(VisibleBaseline other)
      : this(other.Schedule)
    {
      _color = other.Color;
    }

    #endregion

    #region Properties

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

    #endregion

    #region Cloneable

    public VisibleBaseline Clone()
    {
      return new VisibleBaseline(this);
    }

    #endregion
  }
}
