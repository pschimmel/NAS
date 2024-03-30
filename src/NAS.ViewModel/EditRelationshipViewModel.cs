using NAS.Model.Entities;
using NAS.Model.Enums;
using NAS.Resources;
using NAS.ViewModel.Base;
using NAS.ViewModel.Helpers;

namespace NAS.ViewModel
{
  public class EditRelationshipViewModel : ValidatingViewModel
  {
    #region Fields

    private readonly Schedule _schedule;
    private readonly Relationship _relationship;
    private Activity _selectedActivity1;
    private Activity _selectedActivity2;
    private RelationshipType _selectedRelationshipType;
    private int _lag;

    #endregion

    #region Constructor

    public EditRelationshipViewModel(Relationship relationShip, Schedule schedule)
    {
      _schedule = schedule ?? throw new ArgumentNullException(nameof(schedule));
      _relationship = relationShip ?? throw new ArgumentNullException(nameof(relationShip));
      _selectedActivity1 = relationShip.Activity1;
      _selectedActivity2 = relationShip.Activity2;
      _selectedRelationshipType = relationShip.RelationshipType;
      _lag = relationShip.Lag;
    }

    #endregion

    #region Public Properties

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

    #region Apply

    //public void Apply()
    //{
    //  _relationship.Activity1 = _selectedActivity1; // Todo: Make Activity Readonly!
    //  _relationship.Activity2 = _selectedActivity2;
    //  _relationship.RelationshipType = _selectedRelationshipType;
    //  _relationship.Lag = _lag;
    //}

    #endregion
  }
}
