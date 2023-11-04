using System;
using NAS.Model.Entities;
using NAS.Resources;
using NAS.ViewModel.Base;
using NAS.ViewModel.Helpers;

namespace NAS.ViewModel
{
  public class SchedulePropertiesViewModel : ViewModelBase, IValidatable
  {
    #region Constructor

    public SchedulePropertiesViewModel(Schedule schedule)
    {
      Schedule = schedule;
    }

    #endregion

    #region Public Properties

    public override HelpTopic HelpTopicKey => HelpTopic.Properties;

    public Schedule Schedule { get; private set; }

    public string ErrorMessage { get; set; } = null;

    public bool HasErrors => !string.IsNullOrWhiteSpace(ErrorMessage);

    private void addError(string message)
    {
      if (!string.IsNullOrWhiteSpace(message))
      {
        if (!string.IsNullOrWhiteSpace(ErrorMessage))
        {
          ErrorMessage += Environment.NewLine;
        }

        ErrorMessage += message;
        OnPropertyChanged(nameof(ErrorMessage));
        OnPropertyChanged(nameof(HasErrors));
      }
    }

    public bool Validate()
    {
      ErrorMessage = null;
      if (string.IsNullOrWhiteSpace(Schedule.Name))
      {
        addError(NASResources.PleaseEnterName);
      }

      return !HasErrors;
    }

    #endregion
  }
}
