using NAS.Models.Enums;

namespace NAS.Models.Entities
{
  public class GanttLayout : Layout
  {
    private ActivityProperty _leftText;
    private ActivityProperty _centerText;
    private ActivityProperty _rightText;

    public GanttLayout()
    { }

    protected GanttLayout(GanttLayout other)
      : base(other)
    {
      CenterText = other.CenterText;
      LeftText = other.LeftText;
      RightText = other.RightText;
    }

    protected GanttLayout(GanttLayout other, Dictionary<Resource, Resource> resourceMapping)
      : base(other, resourceMapping)
    {
      CenterText = other.CenterText;
      LeftText = other.LeftText;
      RightText = other.RightText;
    }

    public override LayoutType LayoutType => LayoutType.Gantt;

    public ActivityProperty LeftText
    {
      get => _leftText;
      set
      {
        if (_leftText != value)
        {
          _leftText = value;
          OnPropertyChanged(nameof(LeftText));
        }
      }
    }

    public ActivityProperty CenterText
    {
      get => _centerText;
      set
      {
        if (_centerText != value)
        {
          _centerText = value;
          OnPropertyChanged(nameof(CenterText));
        }
      }
    }

    public ActivityProperty RightText
    {
      get => _rightText;
      set
      {
        if (_rightText != value)
        {
          _rightText = value;
          OnPropertyChanged(nameof(RightText));
        }
      }
    }

    public override Layout Clone(Dictionary<Resource, Resource> resourceMapping)
    {
      return new GanttLayout(this, resourceMapping);
    }

    public override Layout Clone()
    {
      return new GanttLayout(this);
    }
  }
}
