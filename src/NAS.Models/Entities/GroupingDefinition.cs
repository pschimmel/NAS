using NAS.Models.Base;
using NAS.Models.Enums;

namespace NAS.Models.Entities
{
  public class GroupingDefinition : NASObject, IClonable<GroupingDefinition>
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
          OnPropertyChanged();
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
          OnPropertyChanged();
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
          OnPropertyChanged();
        }
      }
    }

    public GroupingDefinition Clone()
    {
      return new GroupingDefinition(this);
    }
  }
}
