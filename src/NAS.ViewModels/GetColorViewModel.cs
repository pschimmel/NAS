using System.Drawing;
using NAS.Resources;
using NAS.ViewModels.Base;

namespace NAS.ViewModels
{
  public class GetColorViewModel : DialogContentViewModel
  {
    #region Constructor

    public GetColorViewModel(Color? color)
    {
      Color = color;
    }

    #endregion

    #region Overwritten Members

    public override string Title => NASResources.Color;

    public override string Icon => "Edit";

    public override DialogSize DialogSize => DialogSize.Fixed(275, 350);

    #endregion

    #region Properties

    public Color? Color { get; set; }

    #endregion
  }
}
