using NAS.Model.Enums;

namespace NAS.Model.Entities
{
  public class GroupingDefinition : NASObject
  {
    private ActivityProperty _property;
    private int _order;
    private string _color;

    public GroupingDefinition(ActivityProperty property)
    {
      _property = property;
      _order = 0;
    }

    public GroupingDefinition(GroupingDefinition other)
    {
      _order = other.Order;
      _color = other.Color;
      _property = other.Property;
    }

    public ActivityProperty Property
    {
      get => _property;
      set
      {
        if (_property != value)
        {
          _property = value;
          OnPropertyChanged(nameof(Property));
          OnPropertyChanged(nameof(Name));
        }
      }
    }

    public int Order
    {
      get => _order;
      set
      {
        if (_order != value)
        {
          _order = value;
          OnPropertyChanged(nameof(Order));
        }
      }
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

    public string Name => ActivityPropertyHelper.GetNameOfActivityProperty(Property);

    public override string ToString()
    {
      return Name;
    }
  }
}
