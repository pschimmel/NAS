using NAS.Models.Enums;

namespace NAS.Models.Entities
{
  public class SortingDefinition : NASObject
  {
    private ActivityProperty _property;
    private SortDirection _direction;
    private int _order;

    public SortingDefinition(ActivityProperty property)
    {
      _property = property;
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    private SortingDefinition(SortingDefinition other)
    {
      _property = other.Property;
      _direction = other.Direction;
      _order = other.Order;
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

    public SortDirection Direction
    {
      get => _direction;
      set
      {
        if (_direction != value)
        {
          _direction = value;
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

    public SortingDefinition Clone()
    {
      return new SortingDefinition(this);
    }
  }
}
