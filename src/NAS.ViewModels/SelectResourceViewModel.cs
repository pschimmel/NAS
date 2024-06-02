using NAS.Models.Entities;
using NAS.Resources;
using NAS.ViewModels.Base;
using NAS.ViewModels.Helpers;

namespace NAS.ViewModels
{
  public class SelectResourceViewModel : DialogContentViewModel
  {
    #region Fields


    #endregion

    #region Constructor

    public SelectResourceViewModel(IEnumerable<Resource> resources)
      : base()
    {
      Resources = new List<Resource>(resources);
    }

    #endregion

    #region Overwritten Members

    public override string Title => NASResources.ResourceAssignment;

    public override string Icon => "Resources";

    public override DialogSize DialogSize => DialogSize.Fixed(400, 300);

    public override HelpTopic HelpTopicKey => HelpTopic.Resources;

    #endregion

    #region Properties


    public List<Resource> Resources { get; }

    public Resource SelectedResource { get; set; }

    #endregion
  }
}
