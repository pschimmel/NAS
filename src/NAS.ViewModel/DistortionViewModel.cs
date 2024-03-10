using NAS.Model.Entities;
using NAS.Resources;
using NAS.ViewModel.Base;

namespace NAS.ViewModel
{
  public class DistortionViewModel : ValidatingViewModel
  {
    #region Constructor

    public DistortionViewModel(Distortion currentDistortion)
    {
      CurrentDistortion = currentDistortion;
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

    protected override ValidationResult ValidateImpl()
    {
      if (string.IsNullOrWhiteSpace(CurrentDistortion.Description))
      {
        return ValidationResult.Error(NASResources.PleaseEnterDescription);
      }

      switch (CurrentDistortion)
      {
        case Delay delay:
          if (!delay.Days.HasValue)
          {
            return ValidationResult.Error(NASResources.PleaseEnterNumberOfDays);
          }
          break;
        case Extension extension:
          if (!extension.Days.HasValue)
          {
            return ValidationResult.Error(NASResources.PleaseEnterNumberOfDays);
          }
          break;
        case Inhibition inhibition:
          if (!inhibition.Percent.HasValue)
          {
            return ValidationResult.Error(NASResources.PleaseEnterPercentage);
          }
          break;
        case Interruption interruption:
          if (!interruption.Days.HasValue)
          {
            return ValidationResult.Error(NASResources.PleaseEnterNumberOfDays);
          }
          else if (!interruption.Start.HasValue)
          {
            return ValidationResult.Error(NASResources.PleaseEnterStartDate);
          }
          break;
        case Reduction reduction:
          if (!reduction.Days.HasValue)
          {
            return ValidationResult.Error(NASResources.PleaseEnterNumberOfDays);
          }
          break;
      }
      return ValidationResult.OK();
    }

    #endregion
  }
}
