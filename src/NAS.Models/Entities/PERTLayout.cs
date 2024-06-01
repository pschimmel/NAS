using NAS.Models.Enums;

namespace NAS.Models.Entities
{
  public class PERTLayout : Layout
  {
    public PERTLayout()
    { }

    public PERTLayout(PERTLayout other)
      : base(other)
    { }

    public override LayoutType LayoutType => LayoutType.PERT;
  }
}
