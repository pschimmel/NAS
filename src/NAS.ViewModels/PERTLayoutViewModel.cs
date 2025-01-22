using NAS.Models.Entities;
using NAS.Models.Enums;

namespace NAS.ViewModels
{
  public class PERTLayoutViewModel : VisibleLayoutViewModel
  {
    #region Constructor

    public PERTLayoutViewModel(Schedule schedule, PERTLayout layout)
      : base(schedule, layout)
    {
      if (layout.LayoutType != LayoutType.PERT)
      {
        throw new ArgumentException("Layout is not a PERT layout!");
      }
    }

    protected override void Initialize()
    { }

    #endregion

    #region Properties

    public override LayoutType LayoutType => LayoutType.PERT;

    public PERTDefinition PertDefinition => (Layout as PERTLayout).PERTDefinition;

    #endregion
  }
}
