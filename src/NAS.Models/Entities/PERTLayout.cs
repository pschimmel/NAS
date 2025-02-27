using NAS.Models.Enums;

namespace NAS.Models.Entities
{
  public class PERTLayout : Layout
  {
    private PERTDefinition _pertDefinition;

    public PERTLayout()
    { }

    protected PERTLayout(PERTLayout other, Dictionary<Resource, Resource> resourceMapping)
      : base(other, resourceMapping)
    {
      PERTDefinition = other.PERTDefinition.Clone();
    }

    protected PERTLayout(PERTLayout other)
      : base(other)
    {
      PERTDefinition = other.PERTDefinition.Clone();
    }

    public override LayoutType LayoutType => LayoutType.PERT;

    public PERTDefinition PERTDefinition
    {
      get => _pertDefinition;
      set
      {
        if (_pertDefinition != value)
        {
          _pertDefinition = value;
          OnPropertyChanged(nameof(PERTDefinition));
        }
      }
    }

    public override Layout Clone(Dictionary<Resource, Resource> resourceMapping)
    {
      return new PERTLayout(this, resourceMapping);
    }

    public override Layout Clone()
    {
      return new PERTLayout(this);
    }
  }
}
