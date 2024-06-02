using NAS.Models.Entities;
using NAS.ViewModels.Base;
using NAS.ViewModels.Helpers;

namespace NAS.ViewModels
{
  public class SelectBaselineViewModel : ViewModelBase
  {
    #region Fields

    private Schedule selectedBaseline;

    #endregion

    #region Constructor

    public SelectBaselineViewModel(IEnumerable<Schedule> schedules)
      : base()
    {
      Schedules = new List<Schedule>(schedules);
    }

    #endregion

    #region Properties

    public override HelpTopic HelpTopicKey => HelpTopic.Compare;

    public List<Schedule> Schedules { get; private set; }

    public Schedule SelectedBaseline
    {
      get => selectedBaseline;
      set
      {
        if (selectedBaseline != value)
        {
          selectedBaseline = value;
          OnPropertyChanged(nameof(SelectedBaseline));
        }
      }
    }

    #endregion
  }
}
