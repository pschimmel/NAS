using NAS.Models.Enums;
using NAS.Resources;
using NAS.ViewModels.Base;

namespace NAS.ViewModels
{
  public class SelectActivityPropertyViewModel : DialogContentViewModel
  {
    #region Constructor

    public SelectActivityPropertyViewModel()
      : base()
    {
      SelectedActivityProperty = ActivityProperties.First();
    }

    #endregion

    #region Properties

#pragma warning disable CA1822 // Mark members as static
    public IEnumerable<ActivityProperty> ActivityProperties => Enum.GetValues<ActivityProperty>().Cast<ActivityProperty>();
#pragma warning restore CA1822 // Mark members as static

    public ActivityProperty SelectedActivityProperty { get; set; }

    #endregion

    #region Overwritten Members

    public override string Title => NASResources.PleaseSelectProperty;

    public override string Icon => "Activity";

    public override DialogSize DialogSize => DialogSize.Fixed(350, 150);

    #endregion
  }
}
