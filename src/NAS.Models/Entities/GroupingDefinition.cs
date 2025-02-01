using NAS.Models.Base;
using NAS.Models.Enums;

namespace NAS.Models.Entities
{
  public class GroupingDefinition : NASObject, IClonable<GroupingDefinition>
  {
    public GroupingDefinition(ActivityProperty property)
    {
      Property = property;
      Order = 0;
    }

    public GroupingDefinition(GroupingDefinition other)
    {
      Order = other.Order;
      Color = other.Color;
      Property = other.Property;
    }

    public ActivityProperty Property { get; set; }

    public int Order { get; set; }

    public string Color { get; set; }

    public string Name => ActivityPropertyHelper.GetNameOfActivityProperty(Property);

    public GroupingDefinition Clone()
    {
      return new GroupingDefinition(this);
    }

    public override string ToString()
    {
      return Name;
    }
  }
}
