using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using ES.Tools.Core.MVVM;
using ES.Tools.UI;
using NAS.Models.Base;
using NAS.Models.Entities;
using NAS.Models.Enums;
using NAS.Resources;
using NAS.ViewModels;
using NAS.Views.Converters;
using NAS.Views.Helpers;

namespace NAS.Views.Controls
{
  /// <summary>
  /// Interaction logic for UserControlGantt.xaml
  /// </summary>
  public partial class UserControlGantt
  {
    #region Fields

    private bool suspendRefreshing = false;

    #endregion

    #region Constructors

    public UserControlGantt()
    {
      InitializeComponent();
      dataGrid.RowHeight = GanttHelpers.RowHeight;
      dataGrid.ColumnHeaderHeight = GanttHelpers.ColumnHeaderHeight;
      canvas.RequestCheckPosition += Canvas_RequestCheckPosition;
      canvas.PreviewMouseMove += Canvas_PreviewMouseMove;
      canvas.MouseLeave += Canvas_MouseLeave;
      DataContextChanged += UserControl_DataContextChanged;
      Loaded += UserControlGantt_Loaded;
    }

    private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      suspendRefreshing = true;
      if (e.OldValue is ScheduleViewModel oldVM)
      {
        oldVM.PropertyChanged -= ViewModel_PropertyChanged;
        oldVM.ActivityAdded -= ViewModel_ActivityAdded;
        oldVM.ActivityDeleted -= ViewModel_ActivityDeleted;
        oldVM.RefreshLayout -= ViewModel_RefreshLayout;
        oldVM.ColumnsChanged -= ViewModel_ColumnsChanged;
      }
      if (e.NewValue is ScheduleViewModel vm)
      {
        vm.PropertyChanged += ViewModel_PropertyChanged;
        vm.ActivityAdded += ViewModel_ActivityAdded;
        vm.ActivityDeleted += ViewModel_ActivityDeleted;
        vm.RefreshLayout += ViewModel_RefreshLayout;
        vm.ColumnsChanged += ViewModel_ColumnsChanged;
        suspendRefreshing = false;
        RefreshProject();
      }
    }

    private void UserControlGantt_Loaded(object sender, RoutedEventArgs e)
    {
      Loaded -= UserControlGantt_Loaded;

      Keyboard.Focus(this);
      var sv = GetScrollViewer();
      if (sv != null)
      {
        sv.ScrollChanged -= ScrollViewer_ScrollChanged;
        sv.ScrollChanged += ScrollViewer_ScrollChanged;
        scrollViewer.ScrollChanged -= ScrollViewer_ScrollChanged;
        scrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;
      }
    }

    #endregion

    #region Dependency Properties

    #region Row Height

    public static readonly DependencyProperty RowHeightProperty = DependencyProperty.Register("RowHeight", typeof(double), typeof(UserControlGantt), new FrameworkPropertyMetadata(GanttHelpers.RowHeight, OnRowHeightPropertyChanged));

    public double RowHeight
    {
      get => (double)GetValue(RowHeightProperty);
      set => SetValue(RowHeightProperty, value);
    }

    private static void OnRowHeightPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
    {
      var control = source as UserControlGantt;
      control.canvas.RowHeight = (double)e.NewValue;
    }

    #endregion

    #region Column Header Height

    public static readonly DependencyProperty ColumnHeaderHeightProperty = DependencyProperty.Register("ColumnHeaderHeight", typeof(double), typeof(UserControlGantt), new FrameworkPropertyMetadata(GanttHelpers.ColumnHeaderHeight, OnColumnHeaderHeightPropertyChanged));

    public double ColumnHeaderHeight
    {
      get => (double)GetValue(ColumnHeaderHeightProperty);
      set => SetValue(ColumnHeaderHeightProperty, value);
    }

    private static void OnColumnHeaderHeightPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
    {
      var control = source as UserControlGantt;
      control.canvas.ColumnHeaderHeight = (double)e.NewValue;
    }

    #endregion

    #region Group Header Height

    public static readonly DependencyProperty GroupHeaderHeightProperty = DependencyProperty.Register("GroupHeaderHeight", typeof(double), typeof(UserControlGantt), new FrameworkPropertyMetadata(GanttHelpers.GroupHeaderHeight, OnGroupHeaderHeightPropertyChanged));

    public double GroupHeaderHeight
    {
      get => (double)GetValue(GroupHeaderHeightProperty);
      set => SetValue(GroupHeaderHeightProperty, value);
    }

    private static void OnGroupHeaderHeightPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
    {
      var control = source as UserControlGantt;
      control.canvas.GroupHeaderHeight = (double)e.NewValue;
    }

    #endregion

    #endregion

    #region Public Properties

    public Layout Layout => VM.CurrentLayout.Layout;

    #endregion

    #region ViewModels

    private ScheduleViewModel VM => DataContext as ScheduleViewModel;

    private void ViewModel_ColumnsChanged(object sender, EventArgs e)
    {
      RefreshColumns();
    }

    private void ViewModel_RefreshLayout(object sender, EventArgs e)
    {
      SortFilterAndGroup();
      RefreshProject();
      RefreshColumns();
    }

    private void ViewModel_ActivityAdded(object sender, ItemEventArgs<ActivityViewModel> e)
    {
      foreach (var column in dataGrid.Columns)
      {
        if ((ActivityProperty)(column as ITaggable).Tag == ActivityProperty.Name)
        {
          dataGrid.CurrentColumn = column;
          break;
        }
      }
      RefreshProject();
    }

    private void ViewModel_ActivityDeleted(object sender, ItemEventArgs<ActivityViewModel> e)
    {
      RefreshProject();
    }

    private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "CurrentActivity")
      {
        if (!IsLoaded)
        {
          return;
        }

        VM.CurrentRelationship = null;
        dataGrid.Focus();
        dataGrid.BeginEdit();
      }
    }

    #endregion

    #region Refresh

    public void RefreshProjectAsync()
    {
      if (suspendRefreshing)
      {
        return;
      }

      DispatcherWrapper.Default.BeginInvokeIfRequired(RefreshProject);
    }

    private void RefreshProject()
    {
      if (VM == null || Layout == null)
      {
        return;
      }

      // Show Columns
      RefreshColumns();
      // Filter, sort and group Activities
      SortFilterAndGroup();
    }

    #endregion

    #region Sorting & Grouping

    private void SortFilterAndGroup()
    {
      var view = ViewModelExtensions.GetView(VM.Activities);
      if (view != null)
      {
        if (view is IEditableCollectionView)
        {
          var MyEditableCollectionView = view as IEditableCollectionView;
          if (MyEditableCollectionView.IsAddingNew || MyEditableCollectionView.IsEditingItem)
          {
            MyEditableCollectionView.CommitEdit();
          }
        }
        while (dataGrid.GroupStyle.Any())
        {
          dataGrid.GroupStyle.RemoveAt(0);
        }
        // Filtering
        if (view.CanFilter)
        {
          view.Filter = (object obj) =>
          {
            if (obj is not Models.Entities.Activity)
            {
              return false;
            }

            var activity = obj as Models.Entities.Activity;
            if (activity.Fragnet != null && !activity.Fragnet.IsVisible)
            {
              return false;
            }

            if (Layout.FilterDefinitions.Count == 0)
            {
              return true;
            }

            foreach (var filter in Layout.FilterDefinitions)
            {
              bool isTrue = false;
              isTrue = filter.Compare(activity);
              if (Layout.FilterCombination == FilterCombinationType.Or && isTrue)
              {
                return true;
              }

              if (Layout.FilterCombination == FilterCombinationType.And && !isTrue)
              {
                return false;
              }
            }
            return Layout.FilterCombination == FilterCombinationType.And;
          };
        }
        // Sorting
        if (view.CanSort)
        {
          view.SortDescriptions.Clear();
          foreach (var sortDefinition in Layout.SortingDefinitions)
          {
            if (sortDefinition.Direction == SortDirection.Descending)
            {
              view.SortDescriptions.Add(new SortDescription(sortDefinition.Property.ToString(), ListSortDirection.Descending));
            }
            else
            {
              view.SortDescriptions.Add(new SortDescription(sortDefinition.Property.ToString(), ListSortDirection.Ascending));
            }
          }
        }
        // Grouping
        if (view.CanGroup)
        {
          view.GroupDescriptions.Clear();
          foreach (var groupDefinition in Layout.GroupingDefinitions)
          {
            var factory = new FrameworkElementFactory(typeof(TextBlock));
            switch (groupDefinition.Property)
            {
              case ActivityProperty.Number:
              case ActivityProperty.Name:
              case ActivityProperty.Fragnet:
              case ActivityProperty.WBSItem:
              case ActivityProperty.CustomAttribute1:
              case ActivityProperty.CustomAttribute2:
              case ActivityProperty.CustomAttribute3:
                factory.SetBinding(TextBlock.TextProperty, new Binding("Name"));
                break;
              case ActivityProperty.ActualFinishDate:
              case ActivityProperty.ActualStartDate:
              case ActivityProperty.EarlyFinishDate:
              case ActivityProperty.EarlyStartDate:
              case ActivityProperty.FinishDate:
              case ActivityProperty.LateFinishDate:
              case ActivityProperty.LateStartDate:
              case ActivityProperty.StartDate:
                factory.SetBinding(TextBlock.TextProperty, new Binding("Name") { StringFormat = "yyyy-MM-dd" });
                break;
              case ActivityProperty.TotalActualCosts:
              case ActivityProperty.TotalPlannedCosts:
              case ActivityProperty.TotalBudget:
                factory.SetBinding(TextBlock.TextProperty, new Binding("Name") { StringFormat = "{0:c}" });
                break;
              case ActivityProperty.ActualDuration:
              case ActivityProperty.RetardedDuration:
              case ActivityProperty.FreeFloat:
              case ActivityProperty.OriginalDuration:
              case ActivityProperty.RemainingDuration:
              case ActivityProperty.TotalFloat:
                factory.SetBinding(TextBlock.TextProperty, new Binding("Name") { StringFormat = NASResources.XDays });
                break;
              case ActivityProperty.PercentComplete:
                factory.SetBinding(TextBlock.TextProperty, new Binding("Name") { StringFormat = "{0:F1} %" });
                break;
            }
            factory.SetValue(TextBlock.BackgroundProperty, new SolidColorBrush(DiagramHelperExtensions.TryParseColor(groupDefinition.Color, Colors.LightGray)));
            factory.SetValue(TextBlock.FontWeightProperty, FontWeights.Bold);
            var dt = new DataTemplate();
            dt.VisualTree = factory;
            var groupStyle = new GroupStyle();
            groupStyle.HeaderTemplate = dt;
            groupStyle.Panel = new ItemsPanelTemplate(new FrameworkElementFactory(typeof(DataGridRowsPresenter)));
            dataGrid.GroupStyle.Add(groupStyle);
            view.GroupDescriptions.Add(new PropertyGroupDescription(groupDefinition.Property.ToString()));
          }
        }
      }
    }

    #endregion

    #region Data Grid

    private void RefreshColumns()
    {
      var newColumns = GetVisibleColumns();

      dataGrid.Columns.Clear();
      foreach (var column in newColumns.OrderBy(x => x.DisplayIndex))
      {
        dataGrid.Columns.Add(column);

        // Set column width
        var property = (ActivityProperty)(column as ITaggable).Tag;
        if (property != ActivityProperty.None)
        {
          var i = Layout.ActivityColumns.ToList().Find(x => x.Property == property);
          if (i != null)
          {
            i.ColumnWidth = column.Width.IsAuto ? null : column.Width.Value;
          }
        }
      }
    }

    private void DataGrid_ColumnReordered(object sender, DataGridColumnEventArgs e)
    {
      if (e.Column != null && e.Column.Header != null)
      {
        var property = (ActivityProperty)(e.Column as ITaggable).Tag;
        int idx = e.Column.DisplayIndex;
        var col = Layout.ActivityColumns.FirstOrDefault(x => x.Property == property);

        if (col != null)
        {
          var columnList = Layout.ActivityColumns.OrderBy(x => x.Order).ToList();
          columnList.Remove(col);
          columnList.Insert(idx, col);

          for (int i = 0; i < columnList.Count; i++)
          {
            columnList[i].Order = i;
          }
        }
      }
    }

    private void DataGrid_Sorting(object sender, DataGridSortingEventArgs e)
    {
      if (e.Column != null && e.Column.Header != null)
      {
        var property = (ActivityProperty)(e.Column as ITaggable).Tag;

        if (Layout.SortingDefinitions.Count == 1 && Layout.SortingDefinitions.First().Property == property)
        {
          if (Layout.SortingDefinitions.First().Direction == SortDirection.Ascending)
          {
            Layout.SortingDefinitions.First().Direction = SortDirection.Descending;
            e.Column.SortDirection = ListSortDirection.Descending;
          }
          else
          {
            Layout.SortingDefinitions.First().Direction = SortDirection.Ascending;
            e.Column.SortDirection = ListSortDirection.Ascending;
          }
        }
        else
        {
          e.Column.SortDirection = ListSortDirection.Ascending;
        }
        e.Handled = true;
        RefreshProject();
      }
    }

    #endregion

    #region Mouse Interaction

    private void Canvas_PreviewMouseMove(object sender, MouseEventArgs e)
    {
      if (!IsEnabled)
      {
        return;
      }

      textBlockDate.Visibility = Visibility.Visible;
      textBlockDate.Text = XToDate(Mouse.GetPosition(canvas).X).ToShortDateString();
    }

    private void Canvas_MouseLeave(object sender, MouseEventArgs e)
    {
      textBlockDate.Visibility = Visibility.Hidden;
    }

    private void Canvas_RequestCheckPosition(object sender, CheckPositionEventArgs e)
    {
      var point = Mouse.GetPosition(scrollViewer);
      if (scrollViewer.VerticalOffset > 0 && point.Y <= RowHeight)
      {
        scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - RowHeight);
        e.Result = false;
        return;
      }
      if (scrollViewer.VerticalOffset < scrollViewer.ViewportHeight && point.Y >= scrollViewer.ViewportHeight - RowHeight)
      {
        scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + RowHeight);
        e.Result = false;
        return;
      }
      else if (point.X < 0 || point.Y <= RowHeight || point.X > scrollViewer.ViewportWidth || point.Y > scrollViewer.ViewportHeight)
      {
        canvas.ClearTempItems();
        e.Result = false;
        return;
      }
    }

    private void DataGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      if (!IsEnabled)
      {
        return;
      }

      var dep = (DependencyObject)e.OriginalSource;
      while (dep is not null and not DataGridRow)
      {
        dep = VisualTreeHelper.GetParent(dep);
      }

      if (dep == null)
      {
        return;
      }

      var row = dep as DataGridRow;
      dataGrid.SelectedItem = row.Item;
      dataGrid.CurrentItem = row.Item;
    }

    #endregion

    #region ScrollViewer

    private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
    {
      if (sender is ScrollViewer scrollSender)
      {
        var gridScrollViewer = GetScrollViewer();
        if (scrollSender == gridScrollViewer && scrollSender.ScrollableHeight > 0)
        {
          if (e.VerticalChange != 0)
          {
            scrollViewer.ScrollToVerticalOffset(scrollSender.VerticalOffset / scrollSender.ScrollableHeight * scrollViewer.ScrollableHeight);
          }
        }
        else if (scrollSender == scrollViewer && scrollSender.ScrollableHeight > 0)
        {
          if (e.VerticalChange != 0)
          {
            gridScrollViewer.ScrollToVerticalOffset(scrollSender.VerticalOffset / scrollSender.ScrollableHeight * gridScrollViewer.ScrollableHeight);
            RefreshCalendar();
          }
          if (e.HorizontalChange != 0)
          {
            foreach (var resourcePanel in mainGrid.Children.OfType<UserControlResourcePanel>())
            {
              resourcePanel.scrollViewer.ScrollToHorizontalOffset(scrollSender.HorizontalOffset);
            }
          }
        }
      }
    }


    #endregion

    #region Private Members

    private void RefreshCalendar()
    {
      foreach (var textBlock in canvas.Children.OfType<TextBlock>().Where(i => i.Tag != null && (i.Tag.ToString() == "Calendar1" || i.Tag.ToString() == "Calendar2")))
      {
        if (textBlock.Tag.ToString() == "Calendar1")
        {
          Canvas.SetTop(textBlock, scrollViewer.VerticalOffset);
        }
        else
        {
          Canvas.SetTop(textBlock, scrollViewer.VerticalOffset + ColumnHeaderHeight / 2);
        }
      }
    }

    private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (sender is DataGrid)
      {
        var grid = sender as DataGrid;
        if (grid.SelectedItem != null)
        {
          grid.Dispatcher.BeginInvoke((Action)delegate
          {
            try
            {
              grid.ScrollIntoView(grid.SelectedItem);
            }
            catch { }
          });
        }
      }
    }

    private DateTime XToDate(double x)
    {
      return VM.Schedule.StartDate.AddDays((x - 10) / 10 / VM.Zoom).Date;
    }

    private ScrollViewer GetScrollViewer()
    {
      if (VisualTreeHelper.GetChildrenCount(dataGrid) > 0)
      {
        if (VisualTreeHelper.GetChild(dataGrid, 0) is Decorator border)
        {
          return border.Child as ScrollViewer;
        }
      }
      return null;
    }

    private List<DataGridColumn> GetVisibleColumns()
    {
      var result = new List<DataGridColumn>();
      if (VM?.Schedule == null || Layout == null)
      {
        return result;
      }

      int idx = 0;

      foreach (var activityColumn in Layout.ActivityColumns.OrderBy(x => x.Order))
      {
        DataGridColumn column= CreateColumn(activityColumn.Property);
        Debug.Assert(column is ITaggable);
        column.Width = activityColumn.ColumnWidth.HasValue ? new DataGridLength(activityColumn.ColumnWidth.Value) : DataGridLength.Auto;
        column.Header = ActivityPropertyHelper.GetNameOfActivityProperty(activityColumn.Property);
        column.IsReadOnly = ActivityPropertyHelper.IsPropertyReadonly(activityColumn.Property);
        column.DisplayIndex = idx++;
        (column as ITaggable).Tag = activityColumn.Property;
        result.Add(column);
      }

      return result;
    }

    private DataGridColumn CreateColumn(ActivityProperty property)
    {
      if (ActivityPropertyHelper.GetPropertyType(property) == typeof(Fragnet))
      {
        var comboBoxColumn = new TaggableDataGridComboBoxColumn();
        comboBoxColumn.ItemsSource = new List<Fragnet>(VM.Schedule.Fragnets);
        comboBoxColumn.SelectedItemBinding = new Binding(property.ToString()) { NotifyOnSourceUpdated = true };
        return comboBoxColumn;
      }
      else if (ActivityPropertyHelper.GetPropertyType(property) == typeof(WBSItem))
      {
        var comboBoxColumn = new TaggableDataGridComboBoxColumn();
        comboBoxColumn.ItemsSource = VM.WBSItems;
        comboBoxColumn.SelectedItemBinding = new Binding(property.ToString()) { NotifyOnSourceUpdated = true };
        return comboBoxColumn;
      }
      else if (ActivityPropertyHelper.GetPropertyType(property) == typeof(CustomAttribute))
      {
        var comboBoxColumn = new TaggableDataGridComboBoxColumn();
        comboBoxColumn.ItemsSource = VM.Schedule.CustomAttributes1;
        comboBoxColumn.SelectedItemBinding = new Binding(property.ToString()) { NotifyOnSourceUpdated = true };
        return comboBoxColumn;
      }
      else
      {
        var textColumn = new TaggableDataGridTextColumn();
        var binding = new Binding(property.ToString()) { NotifyOnSourceUpdated = true };
        if (ActivityPropertyHelper.GetPropertyType(property) == typeof(DateTime))
        {
          binding.Converter = new DateConverter();
          var s = new Style(typeof(TextBlock));
          s.Setters.Add(new Setter(HorizontalAlignmentProperty, System.Windows.HorizontalAlignment.Center));
          textColumn.ElementStyle = s;
        }
        else if (ActivityPropertyHelper.GetPropertyType(property) == typeof(int) || ActivityPropertyHelper.GetPropertyType(property) == typeof(decimal) || ActivityPropertyHelper.GetPropertyType(property) == typeof(double))
        {
          if (ActivityPropertyHelper.GetPropertyType(property) == typeof(decimal))
          {
            binding.StringFormat = "{0:N} €";
            binding.Mode = BindingMode.OneWay;
          }
          var s = new Style(typeof(TextBlock));
          s.Setters.Add(new Setter(HorizontalAlignmentProperty, System.Windows.HorizontalAlignment.Right));
          textColumn.ElementStyle = s;
        }
        textColumn.Binding = binding;
        return textColumn;
      }
    }

    #endregion
  }
}
