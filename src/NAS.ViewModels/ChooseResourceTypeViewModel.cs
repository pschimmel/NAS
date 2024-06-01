using NAS.Resources;
using NAS.ViewModels.Base;

namespace NAS.ViewModels
{
  public class ChooseResourceTypeViewModel : DialogContentViewModel
  {
    public override string Title => NASResources.AddResource;

    public override string Icon => "Resources";

    public bool IsMaterialResourceSelected { get; set; } = true;

    public bool IsWorkResourceSelected { get; set; }

    public bool IsCalendarResourceSelected { get; set; }
  }
}
