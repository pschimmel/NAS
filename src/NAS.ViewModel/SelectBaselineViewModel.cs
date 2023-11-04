using NAS.Model.Entities;
using NAS.ViewModel.Base;
using NAS.ViewModel.Helpers;

namespace NAS.ViewModel
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

    #region Public Properties

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
