using NAS.Models.Entities;
using NAS.Resources;
using NAS.ViewModels.Base;
using NAS.ViewModels.Helpers;

namespace NAS.ViewModels
{
  public class EditDistortionViewModel : DialogContentViewModel
  {
    #region Fields

    private readonly Distortion _currentDistortion;

    #endregion

    #region Constructor

    public EditDistortionViewModel(Schedule schedule, Distortion currentDistortion)
    {
      ArgumentNullException.ThrowIfNull(schedule);
      ArgumentNullException.ThrowIfNull(currentDistortion);

      Fragnets = new List<Fragnet>(schedule.Fragnets);
      SelectedFragnet = currentDistortion.Fragnet;
      _currentDistortion = currentDistortion;
      StartVisible = false;
      DaysVisible = false;
      PercentVisible = false;

      Description = currentDistortion.Description;

      if (currentDistortion is Delay delay)
      {
        Title = NASResources.EditDelay;
        DaysVisible = true;
        Days = delay.Days;
      }
      else if (currentDistortion is Interruption interruption)
      {
        Title = NASResources.EditInterruption;
        DaysVisible = true;
        StartVisible = true;
        Start = interruption.Start;
        Days = interruption.Days;
      }
      else if (currentDistortion is Inhibition inhibition)
      {
        Title = NASResources.EditInhibition;
        PercentVisible = true;
        Percent = inhibition.Percent;
      }
      else if (currentDistortion is Extension extension)
      {
        Title = NASResources.EditExtension;
        DaysVisible = true;
        Days = extension.Days;
      }
      else if (currentDistortion is Reduction reduction)
      {
        Title = NASResources.EditReduction;
        DaysVisible = true;
        Days = reduction.Days;
      }
    }

    #endregion

    #region Overwritten Members

    public override string Title { get; }

    public override string Icon => "Distortion";

    public override DialogSize DialogSize => DialogSize.Fixed(400, 250);

    public override HelpTopic HelpTopicKey => HelpTopic.Distortions;

    #endregion

    #region Properties

    public List<Fragnet> Fragnets { get; }

    public Fragnet SelectedFragnet { get; set; }

    public string Description { get; set; }

    public DateTime? Start { get; set; }

    public int? Days { get; set; }

    public double? Percent { get; set; }

    public bool StartVisible { get; }

    public bool DaysVisible { get; }

    public bool PercentVisible { get; }

    #endregion

    #region Validation

    protected override ValidationResult OnValidating()
    {
      return string.IsNullOrWhiteSpace(Description)
        ? ValidationResult.Error(NASResources.PleaseEnterDescription)
        : DaysVisible && !Days.HasValue
        ? ValidationResult.Error(NASResources.PleaseEnterNumberOfDays)
        : StartVisible && !Start.HasValue
        ? ValidationResult.Error(NASResources.PleaseEnterStartDate)
        : PercentVisible && !Percent.HasValue
        ? ValidationResult.Error(NASResources.PleaseEnterPercentage)
        : ValidationResult.OK();
    }

    #endregion

    #region Apply

    protected override void OnApply()
    {
      _currentDistortion.Description = Description;
      if (_currentDistortion is Delay delay)
      {
        delay.Days = Days.Value;
      }
      else if (_currentDistortion is Interruption interruption)
      {
        interruption.Start = Start.Value;
        interruption.Days = Days.Value;
      }
      else if (_currentDistortion is Inhibition inhibition)
      {
        inhibition.Percent = Percent.Value;
      }
      else if (_currentDistortion is Extension extension)
      {
        extension.Days = Days.Value;
      }
      else if (_currentDistortion is Reduction reduction)
      {
        reduction.Days = Days.Value;
      }

      _currentDistortion.Fragnet = SelectedFragnet;
    }

    #endregion
  }
}
