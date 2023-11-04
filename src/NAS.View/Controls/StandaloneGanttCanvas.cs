using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using NAS.Model.Entities;
using NAS.Model.Enums;
using NAS.ViewModel;

namespace NAS.View.Controls
{
  public class StandaloneGanttCanvas : GanttCanvas, IPrintableCanvas
  {
    private Dictionary<ActivityProperty, double> columnWidths;
    private double row;
    private const double minColumnWidth = 10;

    protected override void RefreshInternal()
    {
      columnWidths = new Dictionary<ActivityProperty, double>();
      if (layout != null)
      {
        foreach (var col in layout.ActivityColumns)
        {
          columnWidths.Add(col.Property, GetColumnWidth(col.Property));
        }
      }
      tableWidth = GetTableWidth();
      row = 0;
      Children.Clear();
      if (VM.Schedule != null)
      {
        AddTableHeaders();
        var view = SortFilterAndGroup();
        if (view != null)
        {
          if (view.Groups == null)
          {
            foreach (Activity activity in view)
            {
              AddActivity(activity);
              AddTableRow(activity);
            }
          }
          else
          {
            foreach (CollectionViewGroup group in view.Groups)
            {
              AddTableGroup(group, 0);
            }
          }
          foreach (var r in VM.Schedule.Relationships)
          {
            if (view.PassesFilter(r.GetActivity1()) && view.PassesFilter(r.GetActivity2()))
            {
              AddRelationship(r);
            }
          }
        }
        Width = DateToX(VM.Schedule.LastDay, true);
        Height = row;
        AddCalendar();
        AddDataDate();
        AddFrame();
        foreach (var resource in VM.Schedule.CurrentLayout.VisibleResources)
        {
          RefreshResources(resource);
        }
      }
    }

    private void AddFrame()
    {
      var rect = new Rectangle();
      rect.Stroke = Brushes.Black;
      var firstDay = GetFirstDay();
      var lastDay = GetLastDay();
      double x1 = DateToX(firstDay, false);
      double x2 = DateToX(lastDay, true);
      rect.Width = x2 - x1;
      rect.Height = row - columnHeaderHeight;
      Children.Add(rect);
      SetLeft(rect, tableWidth);
      SetTop(rect, columnHeaderHeight);
    }

    public void RefreshResources(VisibleResource resource)
    {
      var viewModel = new ResourceViewModel(resource, VM as ScheduleViewModel);
      var c = new ResourcePanelCanvas() { DataContext = viewModel, Height = 100 };
      c.Refresh();
      Height += 100;
      Children.Add(c);
      SetTop(c, Height - 100);
      SetLeft(c, tableWidth);
    }

    #region Paint Table

    private void AddTableHeaders()
    {
      double x = 0;
      foreach (var column in layout.ActivityColumns)
      {
        double width = Math.Max(columnWidths[column.Property], minColumnWidth) + 1;
        AddTableCell(ActivityPropertyHelper.GetNameOfActivityProperty(column.Property),
          TextAlignment.Center,
          x,
          width,
          ColumnHeaderHeight + 1,
          Colors.LightGray);
        x += width - 1;
      }
      row += ColumnHeaderHeight;
    }

    private void AddTableRow(Activity activity)
    {
      double x = 0;
      foreach (var column in layout.ActivityColumns)
      {
        double width = Math.Max(columnWidths[column.Property], minColumnWidth) + 1;
        AddTableCell(
          activity.GetTextFromActivity(column.Property),
          ActivityPropertyHelper.GetAlignment(ActivityPropertyHelper.GetPropertyType(column.Property)),
          x,
          width,
          RowHeight + 1,
          Colors.White);
        x += width - 1;
      }
      row += RowHeight;
    }

    private void AddTableCell(string name, TextAlignment alignment, double x, double width, double height, Color color)
    {
      var block = new TextBlock();
      block.Text = name;
      block.VerticalAlignment = System.Windows.VerticalAlignment.Center;
      block.TextAlignment = alignment;
      AddTableCellBorder(block, x, row, width, height, color);
    }

    private void AddTableGroup(CollectionViewGroup group, int level)
    {
      var block = new TextBlock();
      if (group.Name != null)
      {
        block.Text = group.Name.ToString();
      }

      AddTableCellBorder(block, 0, row, tableWidth, GroupHeaderHeight, (Color)ColorConverter.ConvertFromString(layout.GroupingDefinitions.ToArray()[level].Color));
      row += GroupHeaderHeight;
      if (group.IsBottomLevel)
      {
        foreach (Activity activity in group.Items)
        {
          AddActivity(activity);
          AddTableRow(activity);
        }
      }
      else
      {
        foreach (CollectionViewGroup subGroup in group.Items)
        {
          AddTableGroup(subGroup, level + 1);
        }
      }
    }

    private void AddTableCellBorder(TextBlock block, double x, double y, double width, double height, Color backgroundColor)
    {
      var border = new Border();
      border.Child = block;
      border.Padding = new Thickness(2, 0, 2, 0);
      border.BorderBrush = Brushes.Black;
      border.Background = new SolidColorBrush(backgroundColor);
      border.BorderThickness = new Thickness(1);
      border.UseLayoutRounding = true;
      border.Width = width;
      border.Height = height;
      SetLeft(border, x);
      SetTop(border, y);
      SetZIndex(border, 6);
      Children.Add(border);
    }

    private double GetColumnWidth(ActivityProperty property)
    {
      string s = ActivityPropertyHelper.GetNameOfActivityProperty(property);
      foreach (var a in VM.Schedule.Activities.Where(x => x.Fragnet == null || x.Fragnet.IsVisible))
      {
        string s2 = a.GetTextFromActivity(property);
        if (s2 != null && s2.Length > s.Length)
        {
          s = s2;
        }
      }
      var t = new FormattedText(s, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Verdana"), 12, Brushes.Black, pixelsPerDip);
      return t.Width + 4;
    }

    private double GetTableWidth()
    {
      double result = 0;
      if (layout != null)
      {
        foreach (var column in layout.ActivityColumns)
        {
          result += Math.Max(columnWidths[column.Property], minColumnWidth);
        }
      }
      return result;
    }

    #endregion

    protected void AddDataDate()
    {
      var line = new Line();
      line.Stroke = Brushes.Blue;
      line.StrokeThickness = 3;
      Children.Add(line);
      double x = DateToX(VM.Schedule.DataDate, false);
      line.X1 = x;
      line.X2 = x;
      line.Y1 = 0;
      line.Y2 = Height;
      SetZIndex(line, 0);
    }

    protected DateTime GetFirstDay()
    {
      var startDate = VM.Schedule.FirstDay;
      foreach (var baseline in VM.Schedule.VisibleBaselines)
      {
        if (baseline.Schedule.FirstDay < startDate)
        {
          startDate = baseline.Schedule.FirstDay;
        }
      }
      return startDate;
    }

    protected DateTime GetLastDay()
    {
      var endDate = VM.Schedule.LastDay;
      foreach (var baseline in VM.Schedule.VisibleBaselines)
      {
        if (baseline.Schedule.LastDay > endDate)
        {
          endDate = baseline.Schedule.LastDay;
        }
      }
      return endDate;
    }
  }
}
