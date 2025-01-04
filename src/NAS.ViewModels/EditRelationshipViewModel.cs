using NAS.Models.Entities;
using NAS.Models.Enums;
using NAS.Resources;
using NAS.ViewModels.Base;
using NAS.ViewModels.Helpers;

namespace NAS.ViewModels
{
  public class EditRelationshipViewModel : DialogContentViewModel
  {
    #region Fields

    private readonly Schedule _schedule;
    private Activity _selectedActivity1;
    private Activity _selectedActivity2;
    private RelationshipType _selectedRelationshipType;
    private int _lag;

    #endregion

    #region Constructor

    public EditRelationshipViewModel(Schedule schedule)
    {
      _schedule = schedule ?? throw new ArgumentNullException(nameof(schedule));
    }

    public EditRelationshipViewModel(Schedule schedule, Relationship relationShip = null)
      : this(schedule)
    {
      _selectedActivity1 = relationShip?.Activity1;
      _selectedActivity2 = relationShip?.Activity2;
      _selectedRelationshipType = relationShip?.RelationshipType ?? RelationshipType.FinishStart;
      _lag = relationShip?.Lag ?? 0;
    }

    #endregion

    #region Overwritten Members

    public override string Title => NASResources.Relationship;

    public override string Icon => "Relationship";

    public override DialogSize DialogSize => DialogSize.Fixed(400, 225);

    #endregion

    #region Properties

    public override HelpTopic HelpTopicKey => HelpTopic.Relationship;

    public List<Activity> Activities => _schedule.Activities.ToList();

    public Activity SelectedActivity1
    {
      get => _selectedActivity1;
      set
      {
        if (_selectedActivity1 != value)
        {
          _selectedActivity1 = value;
          OnPropertyChanged(nameof(SelectedActivity1));
        }
      }
    }

    public Activity SelectedActivity2
    {
      get => _selectedActivity2;
      set
      {
        if (_selectedActivity2 != value)
        {
          _selectedActivity2 = value;
          OnPropertyChanged(nameof(SelectedActivity2));
        }
      }
    }

    public List<RelationshipType> RelationshipTypes => Enum.GetValues(typeof(RelationshipType)).Cast<RelationshipType>().ToList();

    public RelationshipType SelectedRelationshipType
    {
      get => _selectedRelationshipType;
      set
      {
        if (_selectedRelationshipType != value)
        {
          _selectedRelationshipType = value;
          OnPropertyChanged(nameof(SelectedRelationshipType));
        }
      }
    }

    public int Lag
    {
      get => _lag;
      set
      {
        if (_lag != value)
        {
          _lag = value;
          OnPropertyChanged(nameof(Lag));
        }
      }
    }

    #endregion

    #region Validation

    protected override ValidationResult OnValidating()
    {
      var result = ValidationResult.OK();

      if (_selectedActivity1 == null || _selectedActivity2 == null)
      {
        result = ValidationResult.Error(NASResources.PleaseSelectTwoActivities);
      }

      if (_selectedActivity1 == _selectedActivity2)
      {
        result = ValidationResult.Error(NASResources.ActivitiesCannotBeEqual);
      }

      return result;
    }

    #endregion
  }
}
