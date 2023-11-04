using System;
using NAS.ViewModel.Base;

namespace NAS.ViewModel
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
