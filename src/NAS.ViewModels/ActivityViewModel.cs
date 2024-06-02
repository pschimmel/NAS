using NAS.Models.Entities;
using NAS.Models.Enums;
using NAS.Resources;
using NAS.ViewModels.Base;
using NAS.ViewModels.Helpers;

namespace NAS.ViewModels
{
  public class ActivityViewModel : ViewModelBase
  {
    #region Fields

    private ResourceAssignment currentResourceAssignment;

    #endregion

    #region Constructor

    public ActivityViewModel(Activity activity)
      : base()
    {
      ArgumentNullException.ThrowIfNull(activity);

      Schedule = activity.Schedule;
      Activity = activity;
      Calendars = new List<Calendar>(Schedule.Calendars);
      Fragnets = new List<Fragnet>(Schedule.Fragnets);
      CustomAttributes1 = new List<CustomAttribute>(Schedule.CustomAttributes1);
      CustomAttributes2 = new List<CustomAttribute>(Schedule.CustomAttributes2);
      CustomAttributes3 = new List<CustomAttribute>(Schedule.CustomAttributes3);
    }

    #endregion

    #region Properties

    public override HelpTopic HelpTopicKey => HelpTopic.Activity;

    public Activity Activity { get; }

    public Schedule Schedule { get; }

    public List<Calendar> Calendars { get; }

    public List<Fragnet> Fragnets { get; }

#pragma warning disable CA1822 // Mark members as static
    public List<ConstraintType> ConstraintTypes => Enum.GetValues(typeof(ConstraintType)).Cast<ConstraintType>().ToList();
#pragma warning restore CA1822 // Mark members as static

    public ResourceAssignment CurrentResourceAssignment
    {
      get => currentResourceAssignment;
      set
      {
        if (currentResourceAssignment != value)
        {
          currentResourceAssignment = value;
          OnPropertyChanged(nameof(CurrentResourceAssignment));
        }
      }
    }

    public List<CustomAttribute> CustomAttributes1 { get; }

    public List<CustomAttribute> CustomAttributes2 { get; }

    public List<CustomAttribute> CustomAttributes3 { get; }

    public bool IsActivity => Activity.ActivityType == ActivityType.Activity;

    public bool IsMilestone => Activity.ActivityType == ActivityType.Milestone;

    public bool IsFixed => Activity.IsFixed;

    public bool IsNotFixed => !IsFixed;

    public string DisplayName
    {
      get
      {
        string result = Activity.Number + " " + Activity.Name;
        if (IsFixed)
        {
          result += " (" + NASResources.Fixed + ")";
        }

        return result;
      }
    }

    #endregion
  }
}
