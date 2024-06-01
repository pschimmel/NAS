using System;
using NAS.ViewModels.Base;

namespace NAS.ViewModels
{
  public class AddDistortionViewModel : ViewModelBase
  {
    #region Constructor

    public AddDistortionViewModel()
    {
      DistortionType = DistortionType.Delay;
    }

    #endregion

    #region Public Properties

    public DistortionType DistortionType { get; set; }

    #endregion
  }
}
