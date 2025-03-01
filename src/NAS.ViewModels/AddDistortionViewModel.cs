using NAS.Resources;
using NAS.ViewModels.Base;
using NAS.ViewModels.Helpers;

namespace NAS.ViewModels
{
  public class AddDistortionViewModel : DialogContentViewModel
  {
    #region Constructor

    public AddDistortionViewModel()
    {
      DistortionType = DistortionType.Delay;
    }

    #endregion

    #region Overwritten Members

    public override string Title => NASResources.AddDistortion;

    public override string Icon => "Distortion";

    public override DialogSize DialogSize => DialogSize.Fixed(300, 250);

    public override HelpTopic HelpTopicKey => HelpTopic.Distortions;

    #endregion

    #region Properties

    public DistortionType DistortionType { get; private set; }

    public bool IsDelay
    {
      get => DistortionType == DistortionType.Delay;
      set
      {
        if (value)
        {
          DistortionType = DistortionType.Delay;
          OnPropertyChanged(nameof(IsDelay));
          OnPropertyChanged(nameof(IsExtension));
          OnPropertyChanged(nameof(IsInhibition));
          OnPropertyChanged(nameof(IsInterruption));
          OnPropertyChanged(nameof(IsReduction));
        }
      }
    }

    public bool IsExtension
    {
      get => DistortionType == DistortionType.Extension;
      set
      {
        if (value)
        {
          DistortionType = DistortionType.Extension;
          OnPropertyChanged(nameof(IsDelay));
          OnPropertyChanged(nameof(IsExtension));
          OnPropertyChanged(nameof(IsInhibition));
          OnPropertyChanged(nameof(IsInterruption));
          OnPropertyChanged(nameof(IsReduction));
        }
      }
    }

    public bool IsInhibition
    {
      get => DistortionType == DistortionType.Inhibition;
      set
      {
        if (value)
        {
          DistortionType = DistortionType.Inhibition;
          OnPropertyChanged(nameof(IsDelay));
          OnPropertyChanged(nameof(IsExtension));
          OnPropertyChanged(nameof(IsInhibition));
          OnPropertyChanged(nameof(IsInterruption));
          OnPropertyChanged(nameof(IsReduction));
        }
      }
    }

    public bool IsInterruption
    {
      get => DistortionType == DistortionType.Interruption;
      set
      {
        if (value)
        {
          DistortionType = DistortionType.Interruption;
          OnPropertyChanged(nameof(IsDelay));
          OnPropertyChanged(nameof(IsExtension));
          OnPropertyChanged(nameof(IsInhibition));
          OnPropertyChanged(nameof(IsInterruption));
          OnPropertyChanged(nameof(IsReduction));
        }
      }
    }

    public bool IsReduction
    {
      get => DistortionType == DistortionType.Reduction;
      set
      {
        if (value)
        {
          DistortionType = DistortionType.Reduction;
          OnPropertyChanged(nameof(IsDelay));
          OnPropertyChanged(nameof(IsExtension));
          OnPropertyChanged(nameof(IsInhibition));
          OnPropertyChanged(nameof(IsInterruption));
          OnPropertyChanged(nameof(IsReduction));
        }
      }
    }

    #endregion
  }
}
