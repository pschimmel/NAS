using NAS.Models.Entities;
using NAS.Resources;
using NAS.ViewModels.Base;
using NAS.ViewModels.Helpers;

namespace NAS.ViewModels
{
  public class SelectBaselineViewModel : DialogContentViewModel
  {
    #region Constructor

    public SelectBaselineViewModel(IEnumerable<Schedule> schedules)
      : base()
    {
      Baselines = new List<Schedule>(schedules);
    }

    #endregion

    #region Overwritten Members

    public override string Title => NASResources.Baseline;

    public override string Icon => "Baseline";

    public override DialogSize DialogSize => DialogSize.Fixed(400, 350);

    public override HelpTopic HelpTopicKey => HelpTopic.Baseline;

    #endregion

    #region Properties

    public List<Schedule> Baselines { get; }

    public Schedule SelectedBaseline { get; set; }

    #endregion
  }
}
