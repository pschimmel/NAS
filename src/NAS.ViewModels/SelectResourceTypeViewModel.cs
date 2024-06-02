using NAS.Resources;
using NAS.ViewModels.Base;
using NAS.ViewModels.Helpers;

namespace NAS.ViewModels
{
  public class SelectResourceTypeViewModel : DialogContentViewModel
  {
    #region Overwritten Members

    public override string Title => NASResources.AddResource;

    public override string Icon => "Resources";

    public override DialogSize DialogSize => DialogSize.Fixed(400, 300);

    public override HelpTopic HelpTopicKey => HelpTopic.Resources;

    #endregion

    #region Properties

    public bool IsMaterialResourceSelected { get; set; } = true;

    public bool IsWorkResourceSelected { get; set; }

    public bool IsCalendarResourceSelected { get; set; }

    #endregion
  }
}
