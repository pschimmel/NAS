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
    public SortingDefinition(SortingDefinition other)
    {
      Property = other.Property;
      Direction = other.Direction;
      Order = other.Order;
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

    public SortDirection Direction
    {
      get => _direction;
      set
      {
        if (_direction != value)
        {
          _direction = value;
          OnPropertyChanged(nameof(Direction));
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

    public string Name => ActivityPropertyHelper.GetNameOfActivityProperty(Property) + " (" + SortDirectionHelper.GetNameOfSortDirection(Direction) + ")";

    public override string ToString()
    {
      return Name;
    }
  }
}