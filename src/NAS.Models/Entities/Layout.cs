using System.Collections.ObjectModel;
using System.Diagnostics;
using NAS.Models.Enums;
using NAS.Resources;

namespace NAS.Models.Entities
{
  public abstract class Layout : NASObject, IPrintLayout
  {
    private string _name;
    private string _milestoneCriticalColor;
    private string _milestoneStandardColor;
    private string _milestoneDoneColor;
    private string _activityCriticalColor;
    private string _activityStandardColor;
    private string _activityDoneColor;
    private string _dataDateColor;
    private bool _showRelationships;
    private bool _showFloat;
    private FilterCombinationType _filterCombination;
    private bool _isCurrent;
    private double _headerHeight;
    private double _footerHeight;
    private double _leftMargin;
    private double _rightMargin;
    private double _topMargin;
    private double _bottomMargin;

    protected Layout()
    {
      _name = NASResources.NewLayout;
      _activityStandardColor = "Yellow";
      _activityCriticalColor = "Red";
      _activityDoneColor = "Blue";
      _milestoneStandardColor = "Black";
      _milestoneCriticalColor = "Red";
      _milestoneDoneColor = "Blue";
      _dataDateColor = "Blue";
      _showRelationships = true;
      _showFloat = false;
      _headerHeight = 1;
      _footerHeight = 1;
      ActivityColumns = [];
      FilterDefinitions = [];
      GroupingDefinitions = [];
      SortingDefinitions = [];
      HeaderItems = [];
      FooterItems = [];
      VisibleBaselines = [];
      VisibleResources = [];
    }

    /// <summary>
    /// Copy constructor
    /// </summary>
    protected Layout(Layout other)
    {
      Debug.Assert(LayoutType == other.LayoutType);
      foreach (var otherActivityColumn in other.ActivityColumns)
      {
        ActivityColumns.Add(new ActivityColumn(otherActivityColumn));
      }
      ActivityCriticalColor = other.ActivityCriticalColor;
      ActivityDoneColor = other.ActivityDoneColor;
      ActivityStandardColor = other.ActivityDoneColor;
      DataDateColor = other.DataDateColor;
      FilterCombination = other.FilterCombination;
      foreach (var otherFilterDefinition in other.FilterDefinitions)
      {
        FilterDefinitions.Add(new FilterDefinition(otherFilterDefinition));
      }
      FooterHeight = other.FooterHeight;
      foreach (var otherFooterItem in other.FooterItems)
      {
        FooterItems.Add(new FooterItem(otherFooterItem));
      }
      foreach (var otherGroupingDefinition in other.GroupingDefinitions)
      {
        GroupingDefinitions.Add(new GroupingDefinition(otherGroupingDefinition));
      }
      HeaderHeight = other.HeaderHeight;
      foreach (var otherHeaderItem in other.HeaderItems)
      {
        HeaderItems.Add(new HeaderItem(otherHeaderItem));
      }

      BottomMargin = other.BottomMargin;
      LeftMargin = other.LeftMargin;
      RightMargin = other.RightMargin;
      TopMargin = other.TopMargin;
      MilestoneCriticalColor = other.MilestoneCriticalColor;
      MilestoneStandardColor = other.MilestoneStandardColor;
      MilestoneDoneColor = other.MilestoneDoneColor;
      MilestoneStandardColor = other.MilestoneStandardColor;
      Name = other.Name;
      ShowFloat = other.ShowFloat;
      ShowRelationships = other.ShowRelationships;
      foreach (var otherSortingDefinition in other.SortingDefinitions)
      {
        SortingDefinitions.Add(new SortingDefinition(otherSortingDefinition));
      }
      foreach (var otherVisibleBaseline in other.VisibleBaselines)
      {
        VisibleBaselines.Add(new VisibleBaseline(otherVisibleBaseline));
      }
      foreach (var otherVisibleResource in other.VisibleResources)
      {
        VisibleResources.Add(new VisibleResource(otherVisibleResource));
      }
    }

    public string Name
    {
      get => _name;
      set
      {
        if (_name != value)
        {
          _name = value;
          OnPropertyChanged(nameof(Name));
        }
      }
    }

    public abstract LayoutType LayoutType { get; }

    public bool IsCurrent
    {
      get => _isCurrent;
      set
      {
        if (_isCurrent != value)
        {
          _isCurrent = value;
          OnPropertyChanged(nameof(IsCurrent));
        }
      }
    }

    public string MilestoneCriticalColor
    {
      get => _milestoneCriticalColor;
      set
      {
        if (_milestoneCriticalColor != value)
        {
          _milestoneCriticalColor = value;
          OnPropertyChanged(nameof(MilestoneCriticalColor));
        }
      }
    }

    public string MilestoneStandardColor
    {
      get => _milestoneStandardColor;
      set
      {
        if (_milestoneStandardColor != value)
        {
          _milestoneStandardColor = value;
          OnPropertyChanged(nameof(MilestoneStandardColor));
        }
      }
    }

    public string MilestoneDoneColor
    {
      get => _milestoneDoneColor;
      set
      {
        if (_milestoneDoneColor != value)
        {
          _milestoneDoneColor = value;
          OnPropertyChanged(nameof(MilestoneDoneColor));
        }
      }
    }

    public string ActivityCriticalColor
    {
      get => _activityCriticalColor;
      set
      {
        if (_activityCriticalColor != value)
        {
          _activityCriticalColor = value;
          OnPropertyChanged(nameof(ActivityCriticalColor));
        }
      }
    }

    public string ActivityStandardColor
    {
      get => _activityStandardColor;
      set
      {
        if (_activityStandardColor != value)
        {
          _activityStandardColor = value;
          OnPropertyChanged(nameof(ActivityStandardColor));
        }
      }
    }

    public string ActivityDoneColor
    {
      get => _activityDoneColor;
      set
      {
        if (_activityDoneColor != value)
        {
          _activityDoneColor = value;
          OnPropertyChanged(nameof(ActivityDoneColor));
        }
      }
    }

    public string DataDateColor
    {
      get => _dataDateColor;
      set
      {
        if (_dataDateColor != value)
        {
          _dataDateColor = value;
          OnPropertyChanged(nameof(DataDateColor));
        }
      }
    }

    public bool ShowRelationships
    {
      get => _showRelationships;
      set
      {
        if (_showRelationships != value)
        {
          _showRelationships = value;
          OnPropertyChanged(nameof(ShowRelationships));
        }
      }
    }

    public bool ShowFloat
    {
      get => _showFloat;
      set
      {
        if (_showFloat != value)
        {
          _showFloat = value;
          OnPropertyChanged(nameof(ShowFloat));
        }
      }
    }

    public FilterCombinationType FilterCombination
    {
      get => _filterCombination;
      set
      {
        if (_filterCombination != value)
        {
          _filterCombination = value;
          OnPropertyChanged(nameof(FilterCombination));
        }
      }
    }

    public double HeaderHeight
    {
      get => _headerHeight;
      set
      {
        if (_headerHeight != value)
        {
          _headerHeight = value;
          OnPropertyChanged(nameof(HeaderHeight));
        }
      }
    }

    public double FooterHeight
    {
      get => _footerHeight;
      set
      {
        if (_footerHeight != value)
        {
          _footerHeight = value;
          OnPropertyChanged(nameof(FooterHeight));
        }
      }
    }

    public double LeftMargin
    {
      get => _leftMargin;
      set
      {
        if (_leftMargin != value)
        {
          _leftMargin = value;
          OnPropertyChanged(nameof(LeftMargin));
        }
      }
    }

    public double RightMargin
    {
      get => _rightMargin;
      set
      {
        if (_rightMargin != value)
        {
          _rightMargin = value;
          OnPropertyChanged(nameof(RightMargin));
        }
      }
    }

    public double TopMargin
    {
      get => _topMargin;
      set
      {
        if (_topMargin != value)
        {
          _topMargin = value;
          OnPropertyChanged(nameof(TopMargin));
        }
      }
    }

    public double BottomMargin
    {
      get => _bottomMargin;
      set
      {
        if (_bottomMargin != value)
        {
          _bottomMargin = value;
          OnPropertyChanged(nameof(BottomMargin));
        }
      }
    }

    public ObservableCollection<ActivityColumn> ActivityColumns { get; }

    public ObservableCollection<FilterDefinition> FilterDefinitions { get; }

    public ObservableCollection<GroupingDefinition> GroupingDefinitions { get; }

    public ObservableCollection<HeaderItem> HeaderItems { get; }

    public ObservableCollection<FooterItem> FooterItems { get; }

    public ObservableCollection<SortingDefinition> SortingDefinitions { get; }

    public ObservableCollection<VisibleBaseline> VisibleBaselines { get; }

    public ObservableCollection<VisibleResource> VisibleResources { get; }

    #region Filters

    public void RefreshFilterDefinitions(IEnumerable<FilterDefinition> filterDefinitions)
    {
      if (filterDefinitions == null)
      {
        throw new ArgumentNullException(nameof(filterDefinitions), "Argument can't be null");
      }

      FilterDefinitions.Clear();

      foreach (var filterDefinition in filterDefinitions)
      {
        FilterDefinitions.Add(filterDefinition);
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Returns activity <see cref="string"/> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="string"/> that represents this instance.
    /// </returns>
    public override string ToString()
    {
      return Name;
    }

    #endregion

    #region Private Methods

    #endregion
  }
}
