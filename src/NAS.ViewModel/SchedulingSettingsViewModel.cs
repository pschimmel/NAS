using NAS.Model.Scheduler;
using NAS.Resources;
using NAS.ViewModel.Base;
using NAS.ViewModel.Helpers;

namespace NAS.ViewModel
{
  public class SchedulingSettingsViewModel : DialogContentViewModel
  {
    #region Constructor

    public SchedulingSettingsViewModel(string settingsString)
      : base()
    {
      Settings = SchedulingSettingsHelper.LoadSchedulingSettings(settingsString);
    }

    #endregion

    #region Overwritten Members

    public override string Title => NASResources.SchedulingSettings;

    public override string Icon => "Settings";

    #endregion

    #region Public Properties

    public SchedulingSettings Settings { get; }

    public override HelpTopic HelpTopicKey => HelpTopic.Calculate;

    public string GetScheduleSettingsString()
    {
      return SchedulingSettingsHelper.SaveSchedulingSettings(Settings);
    }

    #endregion
  }
}
