using NAS.Models.Enums;

namespace NAS.Models.Entities
{
  public class PERTLayout : Layout
  {
    private PERTDefinition _pertDefinition;

    public PERTLayout()
    { }

    public PERTLayout(PERTLayout other)
      : base(other)
    {
      PERTDefinition = new PERTDefinition(other.PERTDefinition);
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
  }
}
