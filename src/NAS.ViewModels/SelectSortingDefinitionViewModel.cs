using NAS.Models.Enums;
using NAS.Resources;
using NAS.ViewModels.Base;
using NAS.ViewModels.Helpers;

namespace NAS.ViewModels
{
  public class SelectSortingDefinitionViewModel : DialogContentViewModel
  {
    #region Fields

    private ActivityProperty selectedActivityProperty;
    private SortDirection selectedSortDirection;

    #endregion

    #region Constructor

    public SelectSortingDefinitionViewModel(ActivityProperty property, SortDirection direction)
      : base()
    {
      selectedActivityProperty = property;
      selectedSortDirection = direction;
    }

    public SelectSortingDefinitionViewModel()
      : base()
    {
      if (ActivityProperties.Count != 0)
      {
        selectedActivityProperty = ActivityProperties.First();
      }

      if (SortDirections.Count != 0)
      {
        selectedSortDirection = SortDirections.First();
      }
    }

    #endregion

    #region Overwritten Members

    public override string Title => NASResources.Sorting;

    public override string Icon => "GroupAndSort";

    public override DialogSize DialogSize => DialogSize.Fixed(350, 150);

    public override HelpTopic HelpTopicKey => HelpTopic.GroupAndSort;

    #endregion

    #region Properties

    public List<ActivityProperty> ActivityProperties
    {
      get
      {
        var list = new List<ActivityProperty>();
        foreach (ActivityProperty item in Enum.GetValues(typeof(ActivityProperty)))
        {
          if (item != ActivityProperty.None)
          {
            list.Add(item);
          }
        }

        return list;
      }
    }

    public ActivityProperty SelectedActivityProperty
    {
      get => selectedActivityProperty;
      set
      {
        if (selectedActivityProperty != value)
        {
          selectedActivityProperty = value;
          OnPropertyChanged(nameof(SelectedActivityProperty));
        }
      }
    }

    public List<SortDirection> SortDirections => Enum.GetValues(typeof(SortDirection)).Cast<SortDirection>().ToList();

    public SortDirection SelectedSortDirection
    {
      get => selectedSortDirection;
      set
      {
        if (selectedSortDirection != value)
        {
          selectedSortDirection = value;
          OnPropertyChanged(nameof(SelectedSortDirection));
        }
      }
    }

    #endregion
  }
}
