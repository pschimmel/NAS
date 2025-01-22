using System.Collections.ObjectModel;
using System.ComponentModel;
using ES.Tools.Core.MVVM;
using NAS.Models.Entities;
using NAS.Models.Enums;

namespace NAS.ViewModels
{
  public class GanttLayoutViewModel : VisibleLayoutViewModel
  {
    #region Fields

    private ObservableCollection<SortingDefinition> sortingDefinitions;
    private ObservableCollection<GroupingDefinition> groupingDefinitions;

    #endregion

    #region Constructor

    public GanttLayoutViewModel(Schedule schedule, GanttLayout layout)
      : base(schedule, layout)
    {
      if (layout.LayoutType != LayoutType.Gantt)
      {
        throw new ArgumentException("Layout is not a Gantt layout!");
      }
    }

    protected override void Initialize()
    {
      RefreshColumns();
    }

    #endregion

    #region Properties

    public override LayoutType LayoutType => LayoutType.Gantt;

    public ObservableCollection<ColumnViewModel> Columns { get; } = [];

    public string DataDateColor => Layout.DataDateColor;

    public bool ShowFloat
    {
      get => Layout.ShowFloat;
      set
      {
        if (Layout.ShowFloat != value)
        {
          Layout.ShowFloat = value;
        }
      }
    }

    /// <summary>
    /// Collection of <see cref="SortingDefinition"/>.
    /// </summary>
    public ObservableCollection<SortingDefinition> SortingDefinitions
    {
      get
      {
        if (sortingDefinitions == null)
        {
          sortingDefinitions = new ObservableCollection<SortingDefinition>(Layout.SortingDefinitions);
          var view = ViewModelExtensions.GetView(sortingDefinitions);
          view.SortDescriptions.Add(new SortDescription("Order", ListSortDirection.Ascending));
        }
        return sortingDefinitions;
      }
    }

    /// <summary>
    /// Collection of <see cref="GroupingDefinition"/>.
    /// </summary>
    public ObservableCollection<GroupingDefinition> GroupingDefinitions
    {
      get
      {
        if (groupingDefinitions == null)
        {
          groupingDefinitions = new ObservableCollection<GroupingDefinition>(Layout.GroupingDefinitions);
          var view = ViewModelExtensions.GetView(groupingDefinitions);
          view.SortDescriptions.Add(new SortDescription("Order", ListSortDirection.Ascending));
        }
        return groupingDefinitions;
      }
    }

    public ActivityProperty LeftText => (Layout as GanttLayout).LeftText;

    public ActivityProperty CenterText => (Layout as GanttLayout).CenterText;

    public ActivityProperty RightText => (Layout as GanttLayout).RightText;

    #endregion

    #region Event Handlers

    protected override void AttachEventHandlers()
    {
      base.AttachEventHandlers();

      if (Columns != null)
      {
        Columns.CollectionChanged += Columns_CollectionChanged;
      }
    }

    private void Columns_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      if (e.OldItems != null)
      {
        foreach (var oldColumn in e.OldItems.OfType<ActivityColumn>())
        {
          Columns.Where(x => x.Property == oldColumn.Property)
            .ToList()
            .ForEach(x => Columns.Remove(x));
        }
      }
      if (e.NewItems != null)
      {
        foreach (var newColumn in e.NewItems.OfType<ActivityColumn>())
        {
          if (!Columns.Any(x => x.Property == newColumn.Property))
          {
            var vm = GetColumnViewModel(newColumn);
            Columns.Add(vm);
          }
        }
      }
    }

    #endregion

    #region Columns

    private void RefreshColumns()
    {
      Columns.Clear();

      foreach (var column in Layout.ActivityColumns)
      {
        var vm = GetColumnViewModel(column);
        Columns.Add(vm);
      }
    }

    private ColumnViewModel GetColumnViewModel(ActivityColumn column)
    {
      var vm = column.Property switch
      {
        ActivityProperty.Fragnet => new ColumnViewModel(column.Property, _schedule.Fragnets),
        ActivityProperty.WBSItem => new ColumnViewModel(column.Property, _schedule.GetWBSItems()),
        ActivityProperty.CustomAttribute1 => new ColumnViewModel(column.Property, _schedule.CustomAttributes1),
        ActivityProperty.CustomAttribute2 => new ColumnViewModel(column.Property, _schedule.CustomAttributes2),
        ActivityProperty.CustomAttribute3 => new ColumnViewModel(column.Property, _schedule.CustomAttributes3),
        _ => new ColumnViewModel(column.Property),
      };
      vm.Width = column.ColumnWidth;
      vm.Order = column.Order;
      return vm;
    }

    #endregion
  }
}
