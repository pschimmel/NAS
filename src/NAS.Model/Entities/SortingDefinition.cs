using NAS.Model.Enums;

namespace NAS.Model.Entities
{
  public class SortingDefinition : NASObject
  {
    private ActivityProperty property;
    private SortDirection direction;
    private int order;

    public SortingDefinition()
    { }

    public SortingDefinition(SortingDefinition other)
    {
      Property = other.Property;
      Direction = other.Direction;
      Order = other.Order;
    }

    public ActivityProperty Property
    {
      get => property;
      set
      {
        if (property != value)
        {
          property = value;
          OnPropertyChanged(nameof(Property));
          OnPropertyChanged(nameof(Name));
        }
      }
    }

    public SortDirection Direction
    {
      get => direction;
      set
      {
        if (direction != value)
        {
          direction = value;
          OnPropertyChanged(nameof(Direction));
          OnPropertyChanged(nameof(Name));
        }
      }
    }

    public int Order
    {
      get => order;
      set
      {
        if (order != value)
        {
          order = value;
          OnPropertyChanged(nameof(Order));
        }
      }
    }

    public virtual Layout Layout { get; set; }

    public string Name => ActivityPropertyHelper.GetNameOfActivityProperty(Property) + " (" + SortDirectionHelper.GetNameOfSortDirection(Direction) + ")";

    public override string ToString()
    {
      return Name;
    }
  }
}
