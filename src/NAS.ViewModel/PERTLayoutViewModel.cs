using System;
using NAS.Model.Entities;
using NAS.Model.Enums;

namespace NAS.ViewModel
{
  public class PERTLayoutViewModel : VisibleLayoutViewModel
  {
    #region Constructor

    public PERTLayoutViewModel(Layout layout)
      : base(layout)
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

    public PERTDefinition PertDefinition => Layout.PERTDefinition;

    #endregion
  }
}
