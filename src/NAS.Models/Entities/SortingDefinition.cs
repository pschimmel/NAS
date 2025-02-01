using NAS.Models.Base;
using NAS.Models.Enums;

namespace NAS.Models.Entities
{
  public class SortingDefinition : NASObject, IClonable<SortingDefinition>
  {
    public SortingDefinition(ActivityProperty property)
    {
      Property = property;
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

    public ActivityProperty Property { get; set; }

    public SortDirection Direction { get; set; }

    public int Order { get; set; }

    public string Name => ActivityPropertyHelper.GetNameOfActivityProperty(Property) + " (" + SortDirectionHelper.GetNameOfSortDirection(Direction) + ")";

    public SortingDefinition Clone()
    {
      return new SortingDefinition(this);
    }

    public override string ToString()
    {
      return Name;
    }
  }
}
