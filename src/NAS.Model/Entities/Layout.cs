using System.Collections.ObjectModel;
using NAS.Model.Enums;
using NAS.Resources;

namespace NAS.Model.Entities
{
  public class Layout : NASObject, IPrintLayout
  {
    private string name;
    private LayoutType layoutType;
    private string milestoneCriticalColor;
    private string milestoneStandardColor;
    private string milestoneDoneColor;
    private string activityCriticalColor;
    private string activityStandardColor;
    private string activityDoneColor;
    private string dataDateColor;
    private bool showRelationships;
    private bool showFloat;
    private FilterCombinationType filterCombination;
    private ActivityProperty leftText;
    private ActivityProperty centerText;
    private ActivityProperty rightText;
    private bool isCurrent;
    private PERTDefinition pertDefinition;
    private double headerHeight;
    private double footerHeight;
    private double marginLeft;
    private double marginRight;
    private double marginTop;
    private double marginBottom;

    public Layout()
    {
      Name = NASResources.NewLayout;
      activityStandardColor = "Yellow";
      activityCriticalColor = "Red";
      activityDoneColor = "Blue";
      milestoneStandardColor = "Black";
      milestoneCriticalColor = "Red";
      milestoneDoneColor = "Blue";
      dataDateColor = "Blue";
      showRelationships = true;
      showFloat = false;
      leftText = ActivityProperty.None;
      centerText = ActivityProperty.None;
      rightText = ActivityProperty.None;
      headerHeight = 1;
      footerHeight = 1;
      ActivityColumns = new ObservableCollection<ActivityColumn>();
      FilterDefinitions = new ObservableCollection<FilterDefinition>();
      GroupingDefinitions = new ObservableCollection<GroupingDefinition>();
      SortingDefinitions = new ObservableCollection<SortingDefinition>();
      HeaderItems = new ObservableCollection<HeaderItem>();
      FooterItems = new ObservableCollection<FooterItem>();
      VisibleBaselines = new ObservableCollection<VisibleBaseline>();
      VisibleResources = new ObservableCollection<VisibleResource>();
    }

    /// <summary>
    /// Copy constructor
    /// </summary>
    public Layout(Layout other)
    {
      CopyData(other);
    }

    public string Name
    {
      get => name;
      set
      {
        if (name != value)
        {
          name = value;
          OnPropertyChanged(nameof(Name));
        }
      }
    }

    public LayoutType LayoutType
    {
      get => layoutType;
      set
      {
        if (layoutType != value)
        {
          layoutType = value;
          OnPropertyChanged(nameof(LayoutType));
        }
      }
    }

    public bool IsCurrent
    {
      get => isCurrent;
      set
      {
        if (isCurrent != value)
        {
          isCurrent = value;
          OnPropertyChanged(nameof(IsCurrent));
          if (Schedule != null)
          {
            Schedule.UpdateCurrentLayout();
          }
        }
      }
    }

    public string MilestoneCriticalColor
    {
      get => milestoneCriticalColor;
      set
      {
        if (milestoneCriticalColor != value)
        {
          milestoneCriticalColor = value;
          OnPropertyChanged(nameof(MilestoneCriticalColor));
        }
      }
    }

    public string MilestoneStandardColor
    {
      get => milestoneStandardColor;
      set
      {
        if (milestoneStandardColor != value)
        {
          milestoneStandardColor = value;
          OnPropertyChanged(nameof(MilestoneStandardColor));
        }
      }
    }

    public string MilestoneDoneColor
    {
      get => milestoneDoneColor;
      set
      {
        if (milestoneDoneColor != value)
        {
          milestoneDoneColor = value;
          OnPropertyChanged(nameof(MilestoneDoneColor));
        }
      }
    }

    public string ActivityCriticalColor
    {
      get => activityCriticalColor;
      set
      {
        if (activityCriticalColor != value)
        {
          activityCriticalColor = value;
          OnPropertyChanged(nameof(ActivityCriticalColor));
        }
      }
    }

    public string ActivityStandardColor
    {
      get => activityStandardColor;
      set
      {
        if (activityStandardColor != value)
        {
          activityStandardColor = value;
          OnPropertyChanged(nameof(ActivityStandardColor));
        }
      }
    }

    public string ActivityDoneColor
    {
      get => activityDoneColor;
      set
      {
        if (activityDoneColor != value)
        {
          activityDoneColor = value;
          OnPropertyChanged(nameof(ActivityDoneColor));
        }
      }
    }

    public string DataDateColor
    {
      get => dataDateColor;
      set
      {
        if (dataDateColor != value)
        {
          dataDateColor = value;
          OnPropertyChanged(nameof(DataDateColor));
        }
      }
    }

    public bool ShowRelationships
    {
      get => showRelationships;
      set
      {
        if (showRelationships != value)
        {
          showRelationships = value;
          OnPropertyChanged(nameof(ShowRelationships));
        }
      }
    }

    public bool ShowFloat
    {
      get => showFloat;
      set
      {
        if (showFloat != value)
        {
          showFloat = value;
          OnPropertyChanged(nameof(ShowFloat));
        }
      }
    }

    public FilterCombinationType FilterCombination
    {
      get => filterCombination;
      set
      {
        if (filterCombination != value)
        {
          filterCombination = value;
          OnPropertyChanged(nameof(FilterCombination));
        }
      }
    }

    public ActivityProperty LeftText
    {
      get => leftText;
      set
      {
        if (leftText != value)
        {
          leftText = value;
          OnPropertyChanged(nameof(LeftText));
        }
      }
    }

    public ActivityProperty CenterText
    {
      get => centerText;
      set
      {
        if (centerText != value)
        {
          centerText = value;
          OnPropertyChanged(nameof(CenterText));
        }
      }
    }

    public ActivityProperty RightText
    {
      get => rightText;
      set
      {
        if (rightText != value)
        {
          rightText = value;
          OnPropertyChanged(nameof(RightText));
        }
      }
    }

    public PERTDefinition PERTDefinition
    {
      get => pertDefinition;
      set
      {
        if (pertDefinition != value)
        {
          pertDefinition = value;
          OnPropertyChanged(nameof(PERTDefinition));
        }
      }
    }

    public double HeaderHeight
    {
      get => headerHeight;
      set
      {
        if (headerHeight != value)
        {
          headerHeight = value;
          OnPropertyChanged(nameof(HeaderHeight));
        }
      }
    }

    public double FooterHeight
    {
      get => footerHeight;
      set
      {
        if (footerHeight != value)
        {
          footerHeight = value;
          OnPropertyChanged(nameof(FooterHeight));
        }
      }
    }

    public double MarginLeft
    {
      get => marginLeft;
      set
      {
        if (marginLeft != value)
        {
          marginLeft = value;
          OnPropertyChanged(nameof(MarginLeft));
        }
      }
    }

    public double MarginRight
    {
      get => marginRight;
      set
      {
        if (marginRight != value)
        {
          marginRight = value;
          OnPropertyChanged(nameof(MarginRight));
        }
      }
    }

    public double MarginTop
    {
      get => marginTop;
      set
      {
        if (marginTop != value)
        {
          marginTop = value;
          OnPropertyChanged(nameof(MarginTop));
        }
      }
    }

    public double MarginBottom
    {
      get => marginBottom;
      set
      {
        if (marginBottom != value)
        {
          marginBottom = value;
          OnPropertyChanged(nameof(MarginBottom));
        }
      }
    }

    public virtual ICollection<ActivityColumn> ActivityColumns { get; set; }
    public virtual ICollection<FilterDefinition> FilterDefinitions { get; set; }
    public virtual ICollection<GroupingDefinition> GroupingDefinitions { get; set; }
    public virtual ICollection<HeaderItem> HeaderItems { get; set; }
    public virtual ICollection<FooterItem> FooterItems { get; set; }
    public virtual ICollection<SortingDefinition> SortingDefinitions { get; set; }
    public virtual ICollection<VisibleBaseline> VisibleBaselines { get; set; }
    public virtual ICollection<VisibleResource> VisibleResources { get; set; }
    public virtual Schedule Schedule { get; set; }

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

    #region Sorting/Grouping

    public void RefreshSortingAndGrouping(IEnumerable<SortingDefinition> sortingDefinitions, IEnumerable<GroupingDefinition> groupingDefinitions)
    {
      if (sortingDefinitions == null)
      {
        throw new ArgumentNullException(nameof(sortingDefinitions), "Argument can't be null");
      }

      if (groupingDefinitions == null)
      {
        throw new ArgumentNullException(nameof(sortingDefinitions), "Argument can't be null");
      }

      SortingDefinitions.Clear();
      GroupingDefinitions.Clear();

      foreach (var sortingDefinition in sortingDefinitions)
      {
        SortingDefinitions.Add(sortingDefinition);
      }

      foreach (var groupingDefinition in groupingDefinitions)
      {
        GroupingDefinitions.Add(groupingDefinition);
      }
    }

    #endregion

    #region Print Layout

    public void RefreshPrintLayout(IEnumerable<HeaderItem> headerItems, IEnumerable<FooterItem> footerItems)
    {
      if (headerItems == null)
      {
        throw new ArgumentNullException(nameof(headerItems), "Argument mustn't be null");
      }

      if (footerItems == null)
      {
        throw new ArgumentNullException(nameof(footerItems), "Argument mustn't be null");
      }

      SortingDefinitions.Clear();
      GroupingDefinitions.Clear();

      foreach (var headerItem in headerItems)
      {
        HeaderItems.Add(headerItem);
      }

      foreach (var footerItem in footerItems)
      {
        FooterItems.Add(footerItem);
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

    private void CopyData(Layout other)
    {
      foreach (var otherActivityColumn in other.ActivityColumns)
      {
        ActivityColumns.Add(new ActivityColumn(otherActivityColumn));
      }
      ActivityCriticalColor = other.ActivityCriticalColor;
      ActivityDoneColor = other.ActivityDoneColor;
      ActivityStandardColor = other.ActivityDoneColor;
      CenterText = other.CenterText;
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
      LayoutType = other.LayoutType;
      LeftText = other.LeftText;
      MarginBottom = other.MarginBottom;
      MarginLeft = other.MarginLeft;
      MarginRight = other.MarginRight;
      MarginTop = other.MarginTop;
      MilestoneCriticalColor = other.MilestoneCriticalColor;
      MilestoneStandardColor = other.MilestoneStandardColor;
      MilestoneDoneColor = other.MilestoneDoneColor;
      MilestoneStandardColor = other.MilestoneStandardColor;
      Name = other.Name;
      PERTDefinition = new PERTDefinition(other.PERTDefinition);
      RightText = other.RightText;
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

    #endregion
  }
}
