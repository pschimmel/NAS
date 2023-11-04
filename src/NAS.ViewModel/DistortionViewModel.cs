using System;
using NAS.Model.Entities;
using NAS.Resources;
using NAS.ViewModel.Base;

namespace NAS.ViewModel
{
  public class DistortionViewModel : ViewModelBase, IValidatable
  {
    #region Fields

    private readonly Schedule schedule;

    #endregion

    #region Constructor

    public DistortionViewModel(Distortion currentDistortion)
    {
      CurrentDistortion = currentDistortion;
      schedule = currentDistortion.Activity.Schedule;
      StartVisible = false;
      DaysVisible = false;
      PercentVisible = false;
      FragnetVisible = false;
      if (currentDistortion is Delay)
      {
        Title = NASResources.EditDelay;
        DaysVisible = true;
      }
      else if (currentDistortion is Interruption)
      {
        Title = NASResources.EditInterruption;
        DaysVisible = true;
        StartVisible = true;
      }
      else if (currentDistortion is Inhibition)
      {
        Title = NASResources.EditInhibition;
        PercentVisible = true;
      }
      else if (currentDistortion is Extension)
      {
        Title = NASResources.EditExtension;
        DaysVisible = true;
      }
      else if (currentDistortion is Reduction)
      {
        Title = NASResources.EditReduction;
        DaysVisible = true;
      }
    }

    #endregion

    #region Public Properties

    public Distortion CurrentDistortion { get; private set; }

    public string Title { get; private set; }

    public bool StartVisible { get; private set; }

    public bool DaysVisible { get; private set; }

    public bool PercentVisible { get; private set; }

    public bool FragnetVisible { get; private set; }

    #endregion

    #region Validation

    private string errorMessage = null;

    public string ErrorMessage
    {
      get => errorMessage;
      set
      {
        errorMessage = value;
        OnPropertyChanged(nameof(ErrorMessage));
        OnPropertyChanged(nameof(HasErrors));
      }
    }

    public bool HasErrors => !string.IsNullOrWhiteSpace(ErrorMessage);

    public bool Validate()
    {
      errorMessage = null;
      if (string.IsNullOrWhiteSpace(CurrentDistortion.Description))
      {
        errorMessage = NASResources.PleaseEnterDescription;
        return false;
      }
      if (CurrentDistortion is Delay)
      {
        if (!(CurrentDistortion as Delay).Days.HasValue)
        {
          if (errorMessage != null)
          {
            errorMessage += Environment.NewLine;
          }

          errorMessage = NASResources.PleaseEnterNumberOfDays;
          return false;
        }
      }
      else if (CurrentDistortion is Extension)
      {
        if (!(CurrentDistortion as Extension).Days.HasValue)
        {
          if (errorMessage != null)
          {
            errorMessage += Environment.NewLine;
          }

          errorMessage = NASResources.PleaseEnterNumberOfDays;
          return false;
        }
      }
      else if (CurrentDistortion is Inhibition)
      {
        if (!(CurrentDistortion as Inhibition).Percent.HasValue)
        {
          if (errorMessage != null)
          {
            errorMessage += Environment.NewLine;
          }

          errorMessage = NASResources.PleaseEnterPercentage;
          return false;
        }
      }
      else if (CurrentDistortion is Interruption)
      {
        if (!(CurrentDistortion as Interruption).Days.HasValue)
        {
          if (errorMessage != null)
          {
            errorMessage += Environment.NewLine;
          }

          errorMessage = NASResources.PleaseEnterNumberOfDays;
          return false;
        }
        if (!(CurrentDistortion as Interruption).Start.HasValue)
        {
          if (errorMessage != null)
          {
            errorMessage += Environment.NewLine;
          }

          errorMessage = NASResources.PleaseEnterStartDate;
          return false;
        }
      }
      else if (CurrentDistortion is Reduction)
      {
        if (!(CurrentDistortion as Reduction).Days.HasValue)
        {
          if (errorMessage != null)
          {
            errorMessage += Environment.NewLine;
          }

          errorMessage = NASResources.PleaseEnterNumberOfDays;
          return false;
        }
      }
      return !HasErrors;
    }

    #endregion
  }
}
