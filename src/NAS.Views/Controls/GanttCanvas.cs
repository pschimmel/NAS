using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ES.Tools.Core.MVVM;
using NAS.Models.Base;
using NAS.Models.Entities;
using NAS.Models.Enums;
using NAS.Views.Helpers;
using NAS.Views.Shapes;
using NAS.ViewModels;
using NAS.ViewModels.Base;
using Activity = NAS.Models.Entities.Activity;

namespace NAS.Views.Controls
{
  public class GanttCanvas : DiagramCanvasBase
  {
    #region Events

    public event EventHandler<CheckPositionEventArgs> RequestCheckPosition;

    #endregion

    #region Fields

    private const string activityTag = "ACTIVITY_";
    private const string baselineTag = "BASELINE_";
    private const string calendarTag = "CALENDAR_";
    private const string imageTag = "IMAGE_";
    private const string floatTag = "FLOAT_";
    private const string dataDateTag = "DATADATE";
    private const string blockTag = "BLOCK";
    private const string cursorUriPath = "pack://application:,,,/NAS.Views;component/Cursors/";
    private Color activityStandardColor = Colors.Yellow;
    private Color activityCriticalColor = Colors.Red;
    private Color activityDoneColor = Colors.Blue;
    private Color milestoneStandardColor = Colors.Black;
    private Color milestoneCriticalColor = Colors.Red;
    private Color milestoneDoneColor = Colors.Blue;
    private Color dataDateColor = Colors.Blue;
    private double rowHeight = GanttHelpers.RowHeight;
    protected double columnHeaderHeight = GanttHelpers.ColumnHeaderHeight;
    protected double groupHeaderHeight = GanttHelpers.GroupHeaderHeight;
    protected double tableWidth;
    protected Layout layout;
    private bool suspendRefreshing = false;
    private Activity dragActivity;
    private ActivityMousePosition dragActivityPosition = ActivityMousePosition.Middle;
    private ActivityMousePosition dropActivityPosition = ActivityMousePosition.Middle;
    private Line tempLine = null;
    protected readonly double pixelsPerDip;

    #endregion

    #region Constructor

    public GanttCanvas()
    {
      pixelsPerDip = VisualTreeHelper.GetDpi(this).PixelsPerDip;
      DataContextChanged += Canvas_DataContextChanged;
    }

    #endregion

    #region Properties

    protected bool IsStandalone => this is IPrintableCanvas;

    public double RowHeight
    {
      get => rowHeight;
      set
      {
        if (rowHeight != value)
        {
          rowHeight = value;
          Refresh();
        }
      }
    }

    public double ColumnHeaderHeight
    {
      get => columnHeaderHeight;
      set
      {
        if (columnHeaderHeight != value)
        {
          columnHeaderHeight = value;
          Refresh();
        }
      }
    }

    public double GroupHeaderHeight
    {
      get => groupHeaderHeight;
      set
      {
        if (groupHeaderHeight != value)
        {
          groupHeaderHeight = value;
          Refresh();
        }
      }
    }

    public override Layout Layout
    {
      get => layout;
      set
      {
        layout = value;
        if (layout != null)
        {
          activityStandardColor = DiagramHelperExtensions.TryParseColor(layout.ActivityStandardColor, activityStandardColor);
          activityCriticalColor = DiagramHelperExtensions.TryParseColor(layout.ActivityCriticalColor, activityCriticalColor);
          activityDoneColor = DiagramHelperExtensions.TryParseColor(layout.ActivityDoneColor, activityDoneColor);
          milestoneStandardColor = DiagramHelperExtensions.TryParseColor(layout.MilestoneStandardColor, milestoneStandardColor);
          milestoneCriticalColor = DiagramHelperExtensions.TryParseColor(layout.MilestoneCriticalColor, milestoneCriticalColor);
          milestoneDoneColor = DiagramHelperExtensions.TryParseColor(layout.MilestoneDoneColor, milestoneDoneColor);
          dataDateColor = DiagramHelperExtensions.TryParseColor(layout.DataDateColor, dataDateColor);
          SortFilterAndGroup();
        }
        Refresh();
      }
    }

    #endregion

    #region ViewModel

    protected ScheduleViewModel VM => DataContext as ScheduleViewModel;

    private void Canvas_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      suspendRefreshing = true;
      if (e.OldValue is ScheduleViewModel oldVM)
      {
        oldVM.PropertyChanged -= ViewModel_PropertyChanged;
        oldVM.ActivityAdded -= ViewModel_ActivityAdded;
        oldVM.ActivityDeleted -= ViewModel_ActivityDeleted;
        oldVM.ActivityChanged -= ViewModel_ActivityChanged;
        oldVM.RelationshipAdded -= ViewModel_RelationshipAdded;
        oldVM.RelationshipDeleted -= ViewModel_RelationshipDeleted;
        oldVM.RelationshipChanged -= ViewModel_RelationshipChanged;
        oldVM.RefreshLayout -= ViewModel_RefreshLayout;
        oldVM.Schedule.PropertyChanged -= Schedule_PropertyChanged;
        Layout = null;
      }

      if (e.NewValue is ScheduleViewModel vm)
      {
        vm.PropertyChanged += ViewModel_PropertyChanged;
        vm.ActivityAdded += ViewModel_ActivityAdded;
        vm.ActivityDeleted += ViewModel_ActivityDeleted;
        vm.ActivityChanged += ViewModel_ActivityChanged;
        vm.RelationshipAdded += ViewModel_RelationshipAdded;
        vm.RelationshipDeleted += ViewModel_RelationshipDeleted;
        vm.RelationshipChanged += ViewModel_RelationshipChanged;
        vm.RefreshLayout += ViewModel_RefreshLayout;
        vm.Schedule.PropertyChanged += Schedule_PropertyChanged;
        Layout = vm.Schedule.CurrentLayout;
      }
      suspendRefreshing = false;
      Refresh();
    }

    private void ViewModel_ActivityAdded(object sender, ItemEventArgs<ActivityViewModel> e)
    {
      AddActivity(e.Item);
    }

    private void Schedule_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "DataDate")
      {
        RefreshDataDate();
      }
      else if (e.PropertyName is "StartDate" or "EndDate")
      {
        RefreshCalendar();
      }
    }

    private void ViewModel_ActivityDeleted(object sender, ItemEventArgs<ActivityViewModel> e)
    {
      Refresh();
    }

    private void ViewModel_ActivityChanged(object sender, ItemEventArgs<ActivityViewModel> e)
    {
      var activity = e.Item;
      RefreshActivity(activity);
      foreach (var predecessor in activity.Activity.GetPreceedingRelationships())
      {
        RefreshRelationship(VM.Relationships.FirstOrDefault(x => x.Relationship == predecessor));
      }

      foreach (var successor in activity.Activity.GetSucceedingRelationships())
      {
        RefreshRelationship(VM.Relationships.FirstOrDefault(x => x.Relationship == successor));
      }
    }

    private void ViewModel_RelationshipAdded(object sender, ItemEventArgs<RelationshipViewModel> e)
    {
      AddRelationship(e.Item);
    }

    private void ViewModel_RelationshipDeleted(object sender, ItemEventArgs<RelationshipViewModel> e)
    {
      RemoveRelationship(e.Item);
    }

    private void ViewModel_RelationshipChanged(object sender, ItemEventArgs<RelationshipViewModel> e)
    {
      RemoveRelationship(e.Item);
      AddRelationship(e.Item);
    }

    private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!IsLoaded)
      {
        return;
      }

      if (e.PropertyName == "CurrentActivity")
      {
        Select<ActivityShapeBase, ActivityViewModel>(VM.CurrentActivity);
      }
      else if (e.PropertyName == "CurrentRelationship")
      {
        Select<RelationshipPath, RelationshipViewModel>(VM.CurrentRelationship);
      }
      else if (e.PropertyName == "Zoom")
      {
        Refresh();
      }
    }

    private void ViewModel_RefreshLayout(object sender, EventArgs e)
    {
      Layout = VM.Schedule.CurrentLayout;
      Refresh();
    }

    #endregion

    #region Refresh

    public override void Refresh()
    {
      if (suspendRefreshing || layout == null)
      {
        return;
      }

      RefreshInternal();
    }

    protected virtual void RefreshInternal()
    {
      Width = 50;
      Height = 50;
      Children.Clear();
      if (VM.Schedule != null)
      {
        // Filter, sort and group Activities
        SortFilterAndGroup();

        // Draw Activities
        var view = (CollectionView)CollectionViewSource.GetDefaultView(VM.Activities);
        if (view != null)
        {
          foreach (var a in view.OfType<ActivityViewModel>().ToList())
          {
            AddActivity(a);
          }

          // Draw Relationships
          foreach (var r in VM.Relationships)
          {
            if (view.PassesFilter(r.Relationship.Activity1) && view.PassesFilter(r.Relationship.Activity2))
            {
              AddRelationship(r);
            }
            else
            {
              RemoveRelationship(r);
            }
          }
        }

        // Draw Calendar
        AddCalendar();

        // Draw Data Date
        RefreshDataDate();

        // Refresh baseline
        foreach (var b in layout.VisibleBaselines)
        {
          RemoveBaseline(b);
          AddBaseline(b);
        }
      }
    }

    #endregion

    #region Activities

    protected void AddActivity(ActivityViewModel activity)
    {
      var shape = CreateActivityShape(activity);
      shape.Tag = activityTag + activity.Activity.ID;
      Children.Add(shape);
      SetZIndex(shape, 3);
      AddFloat(activity);
      RefreshActivity(activity, shape);
    }

    private void AddFloat(ActivityViewModel activity)
    {
      var rect = new Rectangle();
      rect.RadiusX = 2;
      rect.RadiusY = 2;
      rect.Tag = floatTag + activity.Activity.ID;
      Children.Add(rect);
      rect.Stroke = new SolidColorBrush(Color.FromArgb(150, 0, 0, 0));
      SetZIndex(rect, 2);
    }

    private ActivityShapeBase CreateActivityShape(ActivityViewModel activity)
    {
      return activity.IsMilestone
        ? new MilestoneShape(activity) { Width = RowHeight, Height = RowHeight }
        : new ActivityShape(activity) { Height = RowHeight - 4 };
    }

    private void RefreshActivity(ActivityViewModel activity, Shape shape = null)
    {
      if (activity == null)
      {
        return;
      }

      shape ??= GetShapeFromActivity(activity);

      Point location;
      if (shape == null)
      {
        return;
      }

      location = activity.IsMilestone ? RefeshMilestoneShape(activity, shape) : RefreshActivityShape(activity, shape);

      if (activity.IsActivity && activity.Activity.Distortions != null && activity.Activity.Distortions.Count > 0)
      {
        var image = Children.OfType<Image>().FirstOrDefault(i => Equals(i.Tag, imageTag + activity.Activity.ID));
        if (image == null)
        {
          image = new Image() { Source = new BitmapImage(new Uri("pack://application:,,,/NAS.Views;component/Images/Distortion.png")), Height = 16, Width = 16 };
          image.Tag = imageTag + activity.Activity.ID;
          Children.Add(image);
          image.IsHitTestVisible = false;
          string s = string.Empty;
          foreach (var d in activity.Activity.Distortions)
          {
            if (!string.IsNullOrEmpty(s))
            {
              s += Environment.NewLine;
            }

            s += d.ToString();
          }
        }
        SetZIndex(image, 2);
        SetTop(image, GetTop(shape) + shape.Height / 2 + 8);
        SetLeft(image, GetLeft(shape) + shape.Width + 2);
      }
      else
      {
        var image = Children.OfType<Image>().FirstOrDefault(i => Equals(i.Tag, imageTag + activity.Activity.ID));
        if (image != null)
        {
          Children.Remove(image);
        }
      }
      if (!IsStandalone)
      {
        shape.ToolTip = activity.Activity.Name + " (" + activity.Activity.StartDate.ToShortDateString() + " - " + activity.Activity.FinishDate.ToShortDateString() + ")";
      }

      if (Width < location.X + shape.Width + 10)
      {
        Width = location.X + shape.Width + 10;
      }

      if (Height < location.Y + RowHeight + 10)
      {
        Height = location.Y + RowHeight + 10;
      }

      RefreshFloat(activity);
      RefreshActivityText(activity, shape);
    }

    private Point RefreshActivityShape(ActivityViewModel activity, Shape shape)
    {
      Debug.Assert(activity.IsActivity);
      var location = new Point();
      shape.Height = RowHeight - 4;
      double width = DateToX(activity.Activity.FinishDate, true) - DateToX(activity.Activity.StartDate, false);
      if (width < 0)
      {
        width = 0;
      }

      shape.Width = width;
      location.Y = GetYOfActivity(activity) + 2;
      SetTop(shape, location.Y);
      location.X = DateToX(activity.Activity.StartDate, false);
      SetLeft(shape, location.X);
      var remainingColor = activityStandardColor;
      if (activity.Activity.IsCritical)
      {
        remainingColor = activityCriticalColor;
      }

      Brush paintBrush = new SolidColorBrush(remainingColor);
      if (activity.Activity.IsFinished)
      {
        paintBrush = new SolidColorBrush(activityDoneColor);
      }
      else if (activity.Activity.IsStarted && activity.Activity.StartDate < VM.Schedule.DataDate)
      {
        double center = DateToX(VM.Schedule.DataDate, false);
        center = (center - location.X) / shape.Width;
        var gsc = new GradientStopCollection
        {
          new GradientStop(activityDoneColor, 0),
          new GradientStop(activityDoneColor, center),
          new GradientStop(remainingColor, center),
          new GradientStop(remainingColor, 1.0)
        };
        paintBrush = new LinearGradientBrush(gsc, 0.0);
      }
      shape.Fill = paintBrush;
      shape.Stroke = new SolidColorBrush(Colors.Black);
      return location;
    }

    private Point RefeshMilestoneShape(ActivityViewModel milestone, Shape shape)
    {
      Debug.Assert(milestone.IsMilestone);
      var location = new Point();
      location.Y = GetYOfActivity(milestone);
      SetTop(shape, location.Y);
      location.X = DateToX(milestone.Activity.StartDate, false) - RowHeight / 2;
      SetLeft(shape, location.X);
      var color = milestoneStandardColor;
      if (milestone.Activity.IsCritical)
      {
        color = milestoneCriticalColor;
      }

      if (milestone.Activity.IsFinished)
      {
        color = milestoneDoneColor;
      }

      shape.Fill = new SolidColorBrush(color);
      shape.Stroke = Brushes.Black;
      return location;
    }

    private void RefreshFloat(ActivityViewModel activity)
    {
      foreach (var rect in Children.OfType<Rectangle>().Where(x => Equals(x.Tag, floatTag + activity.Activity.ID)))
      {
        if (layout.ShowFloat && activity.Activity.TotalFloat > 0)
        {
          rect.Height = RowHeight - 14;
          double width = DateToX(activity.Activity.LateFinishDate, true) - DateToX(activity.Activity.EarlyFinishDate, false);
          if (width < 0)
          {
            width = 0;
          }

          rect.Width = width;
          double y = GetYOfActivity(activity) + 7;
          SetTop(rect, y);
          double x = DateToX(activity.Activity.EarlyFinishDate, false);
          SetLeft(rect, x);
          if (Width < x + rect.Width + 10)
          {
            Width = x + rect.Width + 10;
          }

          rect.Visibility = Visibility.Visible;
        }
        else
        {
          rect.Visibility = Visibility.Hidden;
        }
      }
    }

    private void RefreshActivityText(ActivityViewModel activity, Shape shape)
    {
      int requiredBlocks = 0;
      if (layout.LeftText != ActivityProperty.None)
      {
        requiredBlocks++;
      }

      if (layout.CenterText != ActivityProperty.None)
      {
        requiredBlocks++;
      }

      if (layout.RightText != ActivityProperty.None)
      {
        requiredBlocks++;
      }

      var blocks = Children.OfType<TextBlock>().Where(x => Equals(x.Tag, blockTag + activity.Activity.ID)).ToList();
      while (blocks.Count > requiredBlocks)
      {
        var item = Children.OfType<TextBlock>().Last(x => Equals(x.Tag, blockTag + activity.Activity.ID));
        Children.Remove(item);
        blocks.Remove(item);
      }
      while (blocks.Count < requiredBlocks)
      {
        var tb = new TextBlock();
        tb.IsHitTestVisible = false;
        tb.Height = RowHeight;
        tb.Tag = blockTag + activity.Activity.ID;
        Children.Add(tb);
        blocks.Add(tb);
        SetZIndex(tb, 4);
        double top = GetTop(shape);
        if (activity.IsMilestone)
        {
          top += 2;
        }

        SetTop(tb, top);
      }
      if (layout.CenterText != ActivityProperty.None && activity.IsActivity)
      {
        var b = blocks[0];
        b.Text = activity.Activity.GetTextFromActivity(layout.CenterText);
        var f = new FormattedText(b.Text, CultureInfo.CurrentCulture, b.FlowDirection, new Typeface(b.FontFamily, b.FontStyle, b.FontWeight, b.FontStretch), b.FontSize, b.Foreground, pixelsPerDip);
        b.Width = f.Width;
        SetLeft(b, GetLeft(shape) + (shape.Width - b.Width) / 2);
        blocks.Remove(b);
      }
      if (layout.LeftText != ActivityProperty.None)
      {
        var b = blocks[0];
        b.Text = activity.Activity.GetTextFromActivity(layout.LeftText);
        var f = new FormattedText(b.Text, CultureInfo.CurrentCulture, b.FlowDirection, new Typeface(b.FontFamily, b.FontStyle, b.FontWeight, b.FontStretch), b.FontSize, b.Foreground, pixelsPerDip);
        b.Width = f.Width;
        double d = GetLeft(shape) - b.Width - 2;
        if (shape is not ActivityShape)
        {
          d -= RowHeight / 2;
        }

        SetLeft(b, d);
        blocks.Remove(b);
      }
      if (layout.RightText != ActivityProperty.None)
      {
        var b = blocks[0];
        b.Text = activity.Activity.GetTextFromActivity(layout.RightText);
        var f = new FormattedText(b.Text, CultureInfo.CurrentCulture, b.FlowDirection, new Typeface(b.FontFamily, b.FontStyle, b.FontWeight, b.FontStretch), b.FontSize, b.Foreground, pixelsPerDip);
        b.Width = f.Width;
        double d = GetLeft(shape) + 2;
        if (shape is ActivityShape)
        {
          d += (shape as ActivityShape).Width;
        }
        else
        {
          d += RowHeight;
        }

        SetLeft(b, d);
        blocks.Remove(b);
        if (Width < d + b.Width + 10)
        {
          Width = d + b.Width + 10;
        }
      }
    }

    #endregion

    #region Relationships

    protected void AddRelationship(RelationshipViewModel relationship)
    {
      if (!layout.ShowRelationships)
      {
        return;
      }

      var path = new RelationshipPath(relationship);
      if (!IsStandalone)
      {
        relationship.PropertyChanged += (sender, args) => RefreshRelationship(sender as RelationshipViewModel);
        path.ToolTip = relationship.ToString();
      }
      Children.Insert(0, path);
      SetZIndex(path, 1);
      RefreshRelationship(relationship);
    }

    private void RemoveRelationship(RelationshipViewModel relationship)
    {
      foreach (var e in Children.OfType<RelationshipPath>().Where(x => x.Item == relationship).ToList())
      {
        Children.Remove(e);
      }
    }

    private void RefreshRelationship(RelationshipViewModel relationship)
    {
      if (!layout.ShowRelationships)
      {
        return;
      }

      var path = GetPathFromRelationship(relationship);
      if (path != null)
      {
        path.Stroke = relationship.Relationship.IsCritical
          ? new SolidColorBrush((Color)ColorConverter.ConvertFromString(layout.ActivityCriticalColor))
          : relationship.Relationship.IsDriving ? Brushes.Black : Brushes.DarkGray;
      }
      var shape1 = GetShapeFromActivity(relationship.Relationship.Activity1);
      var shape2 = GetShapeFromActivity(relationship.Relationship.Activity2);
      if (path == null || shape1 == null || shape2 == null)
      {
        return;
      }

      var startPoint = new Point(GetLeft(shape1), GetTop(shape1) + (RowHeight - 2) / 2);
      var endPoint = new Point(GetLeft(shape2), GetTop(shape2));
      switch (relationship.Relationship.RelationshipType)
      {
        case RelationshipType.FinishStart:
          startPoint.X += shape1.Width;
          break;
        case RelationshipType.FinishFinish:
          startPoint.X += shape1.Width;
          endPoint.X += shape2.Width;
          break;
      }
      int delta = 1;
      if (startPoint.Y > endPoint.Y)
      {
        delta = -1;
        endPoint.Y += RowHeight - 2;
      }
      if (shape2 is MilestoneShape)
      {
        endPoint.Y += delta * RowHeight / 2;
      }

      var sb = new StringBuilder();
      sb.AppendPoint(startPoint.X, startPoint.Y);
      if (startPoint.X > endPoint.X + 5)
      {
        sb.AppendPoint(startPoint.X + 3, startPoint.Y);
        sb.AppendPoint(startPoint.X + 3, startPoint.Y + (shape1.Height / 2 + 2) * delta);
        sb.AppendPoint(endPoint.X, startPoint.Y + (shape1.Height / 2 + 2) * delta);
      }
      else
      {
        sb.AppendPoint(endPoint.X, startPoint.Y);
      }
      sb.AppendPoint(endPoint.X, endPoint.Y - 4 * delta);
      sb.AppendPoint(endPoint.X - 2, endPoint.Y - 4 * delta);
      sb.AppendPoint(endPoint.X, endPoint.Y);
      sb.AppendPoint(endPoint.X + 2, endPoint.Y - 4 * delta);
      sb.AppendPoint(endPoint.X, endPoint.Y - 4 * delta);
      path.Data = Geometry.Parse(sb.ToString());
    }

    #endregion

    #region Baselines

    private void AddBaseline(VisibleBaseline baseline)
    {
      foreach (var baselineActivity in baseline.Schedule.Activities)
      {
        var activity = VM.Activities.FirstOrDefault(x => x.Activity.Number == baselineActivity.Number);
        if (activity == null)
        {
          return;
        }

        var shape = CreateActivityShape(new ActivityViewModel(baselineActivity));
        shape.Tag = baselineTag + baseline.Schedule.ID.ToString();

        Children.Add(shape);

        if (baselineActivity.ActivityType == ActivityType.Milestone)
        {
          double y = GetYOfActivity(activity);
          SetTop(shape, y);
        }
        else
        {
          double width = DateToX(baselineActivity.FinishDate, true) - DateToX(baselineActivity.StartDate, false);
          if (width < 0)
          {
            width = 0;
          }

          shape.Width = width;
          double y = GetYOfActivity(activity) + 2;
          SetTop(shape, y);
        }

        SetZIndex(shape, 0);
        shape.IsHitTestVisible = false;
        shape.Tag = baselineTag + baseline.Schedule.ID + "|" + activity.Activity.ID;
        SetLeft(shape, DateToX(baselineActivity.StartDate, false));
        var color = (Color)ColorConverter.ConvertFromString(baseline.Color);
        shape.Fill = new SolidColorBrush(color);
        shape.Stroke = new SolidColorBrush(color);
        shape.Visibility = Visibility.Visible;
      }
    }

    private void RemoveBaseline(VisibleBaseline baseline)
    {
      string id = baselineTag + baseline.Schedule.ID.ToString();
      foreach (var x in Children.OfType<ActivityShapeBase>().Where(x => x.Tag.ToString().StartsWith(id)).ToList())
      {
        Children.Remove(x);
      }
    }

    #endregion

    #region Background

    private void RefreshDataDate()
    {
      if (VM.Schedule == null)
      {
        return;
      }

      var lines = Children.OfType<Line>();
      if (lines != null)
      {
        var line = lines.FirstOrDefault(i => i.Tag != null && i.Tag.ToString() == dataDateTag);
        if (line == null)
        {
          line = new Line();
          line.Tag = dataDateTag;
          line.Stroke = Brushes.Blue;
          line.StrokeThickness = 3;
          Children.Add(line);
          line.IsHitTestVisible = false;
        }
        double x = DateToX(VM.Schedule.DataDate, false);
        line.X1 = x;
        line.X2 = x;
        line.Y1 = 0;
        line.Y2 = Height;
        SetZIndex(line, 0);
      }
    }

    protected void AddCalendar()
    {
      int x = 1;
      ClearCalendar();
      // Year resource
      var startDate = VM.Schedule.FirstDay;
      var projectEnd = VM.Schedule.LastDay;
      var endDate = new DateTime(startDate.Year, 1, 1).AddYears(1).AddDays(-1); // Gets the end of the year
      if (projectEnd < endDate)
      {
        endDate = projectEnd;
      }

      while (startDate <= projectEnd)
      {
        double x1 = DateToX(startDate, false);
        double x2 = DateToX(endDate, true);
        var textBlock = new TextBlock() { Tag = "Calendar1", FontSize = 9, FontWeight = FontWeights.Bold };
        textBlock.Height = columnHeaderHeight / 2;
        textBlock.Width = x2 - x1;
        textBlock.Text = startDate.Year.ToString();
        textBlock.TextAlignment = TextAlignment.Center;
        textBlock.Background = x % 2 == 0 ? Brushes.LightGreen : Brushes.LightGray;

        x++;
        Children.Add(textBlock);
        SetLeft(textBlock, x1);
        if (projectEnd != endDate)
        {
          var line = new Line() { Tag = "Calendar1", Stroke = Brushes.DarkGreen, StrokeThickness = 0.5, X1 = x2, X2 = x2, Y1 = 0, Y2 = Height };
          Children.Add(line);
        }
        startDate = endDate.AddDays(1);
        endDate = startDate.AddYears(1).AddDays(-1);
        if (projectEnd < endDate)
        {
          endDate = projectEnd;
        }
      }
      // Month Calendar
      startDate = VM.Schedule.FirstDay;
      projectEnd = VM.Schedule.LastDay;
      endDate = new DateTime(startDate.Year, startDate.Month, 1).AddMonths(1).AddDays(-1); // Gets the end of the month
      if (projectEnd < endDate)
      {
        endDate = projectEnd;
      }

      while (startDate <= projectEnd)
      {
        double x1 = DateToX(startDate, false);
        double x2 = DateToX(endDate, true);
        var textBlock = new TextBlock() { Tag = "Calendar2", FontSize = 8, FontWeight = FontWeights.Bold };
        textBlock.Height = columnHeaderHeight / 2;
        textBlock.Width = x2 - x1;
        var f = new FormattedText(startDate.ToString("MMMM"), CultureInfo.CurrentCulture, textBlock.FlowDirection, new Typeface(textBlock.FontFamily, textBlock.FontStyle, textBlock.FontWeight, textBlock.FontStretch), textBlock.FontSize, textBlock.Foreground, pixelsPerDip);
        if (f.Width <= textBlock.Width)
        {
          textBlock.Text = startDate.ToString("MMMM");
        }
        else
        {
          f = new FormattedText(startDate.ToString("MMM"), CultureInfo.CurrentCulture, textBlock.FlowDirection, new Typeface(textBlock.FontFamily, textBlock.FontStyle, textBlock.FontWeight, textBlock.FontStretch), textBlock.FontSize, textBlock.Foreground, pixelsPerDip);
          textBlock.Text = f.Width <= textBlock.Width ? startDate.ToString("MMM") : startDate.ToString("MM");
        }
        textBlock.TextAlignment = TextAlignment.Center;
        textBlock.Background = x % 2 == 0 ? Brushes.BlanchedAlmond : Brushes.WhiteSmoke;

        x++;
        Children.Add(textBlock);
        SetLeft(textBlock, x1);
        if (projectEnd != endDate)
        {
          var line = new Line() { Tag = "Calendar2", Stroke = Brushes.Green, StrokeThickness = 0.1, X1 = x2, X2 = x2, Y1 = 0, Y2 = Height };
          Children.Add(line);
        }
        startDate = endDate.AddDays(1);
        endDate = startDate.AddMonths(1).AddDays(-1);
        if (projectEnd < endDate)
        {
          endDate = projectEnd;
        }
      }
      RefreshCalendar();
    }

    private void ClearCalendar()
    {
      var xX = Children.OfType<TextBlock>();
      foreach (var c in Children.OfType<TextBlock>().Where(i => i.Tag != null && i.Tag.ToString().StartsWith(calendarTag)).ToList())
      {
        Children.Remove(c);
      }

      foreach (var c in Children.OfType<Line>().Where(i => i.Tag != null && i.Tag.ToString().StartsWith(calendarTag)).ToList())
      {
        Children.Remove(c);
      }
    }

    private void RefreshCalendar()
    {
      foreach (var textBlock in Children.OfType<TextBlock>().Where(i => i.Tag != null && (i.Tag.ToString() == calendarTag + "1" || i.Tag.ToString() == calendarTag + "2")))
      {
        SetZIndex(textBlock, 5);
        if (textBlock.Tag.ToString() == calendarTag + "1")
        {
          SetTop(textBlock, 0);
        }
        else
        {
          SetTop(textBlock, ColumnHeaderHeight / 2);
        }
      }
    }

    #endregion

    #region Current Layout

    protected CollectionView SortFilterAndGroup()
    {
      if (VM.Schedule != null)
      {
        var view = ViewModelExtensions.GetView(VM.Activities);
        if (view != null)
        {
          // Filtering
          if (view.CanFilter)
          {
            view.Filter = Contains;
          }

          // Sorting
          if (view.CanSort)
          {
            AddSortDescriptions(view);
          }

          // Grouping
          if (view.CanGroup)
          {
            AddGroupDescriptions(view);
          }
        }
        return view;
      }
      return null;
    }

    private void AddGroupDescriptions(CollectionView view)
    {
      view.GroupDescriptions.Clear();
      foreach (var groupDefinition in layout.GroupingDefinitions)
      {
        view.GroupDescriptions.Add(new PropertyGroupDescription(groupDefinition.Property.ToString()));
      }
    }

    private void AddSortDescriptions(CollectionView view)
    {
      view.SortDescriptions.Clear();
      foreach (var sortDefinition in layout.SortingDefinitions)
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

    private bool Contains(object obj)
    {
      if (obj is not Activity)
      {
        return false;
      }

      var activity = obj as Activity;
      if (activity.Fragnet != null && !activity.Fragnet.IsVisible)
      {
        return false;
      }

      if (layout.FilterDefinitions.Count == 0)
      {
        return true;
      }

      foreach (var filter in layout.FilterDefinitions)
      {
        bool isTrue = filter.Compare(activity);
        if (layout.FilterCombination == FilterCombinationType.Or && isTrue)
        {
          return true;
        }

        if (layout.FilterCombination == FilterCombinationType.And && !isTrue)
        {
          return false;
        }
      }
      return layout.FilterCombination == FilterCombinationType.And;
    }

    #endregion

    #region Mouse Interaction

    protected override void OnPreviewMouseMove(MouseEventArgs e)
    {
      if (!IsEnabled)
      {
        return;
      }

      if (dragActivity != null)
      {
        DrawTempRelationship();
      }
      if (e.OriginalSource is MilestoneShape or ActivityShape)
      {
        if (dragActivity != null)
        {
          DrawCursorForActivityDrop(e.OriginalSource as Shape);
        }
        else
        {
          DrawCursorForActivityDrag(e.OriginalSource as Shape);
        }
      }
    }

    protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
    {
      if (e.Handled)
      {
        return;
      }

      if (!IsEnabled)
      {
        return;
      }

      if (e.OriginalSource is Shape)
      {
        // Clicked item is an element
        if (e.ClickCount == 2)
        {
          MouseEditElement(e.OriginalSource as Shape);
        }
        else
        {
          MouseSelectElement(e.OriginalSource as Shape);
        }
      }
      else
      {
        dragActivity = null;
      }
    }

    private void MouseEditElement(Shape shape)
    {
      if (shape is ActivityShapeBase)
      {
        MouseEditActivity(shape as ActivityShapeBase);
      }
      else if (shape is RelationshipPath)
      {
        MouseEditRelationship(shape as RelationshipPath);
      }
    }

    private void MouseSelectElement(Shape shape)
    {
      if (shape is ActivityShapeBase)
      {
        MouseSelectActivity(shape as ActivityShapeBase);
      }
      else if (shape is RelationshipPath)
      {
        MouseSelectRelationship(shape as RelationshipPath);
      }
      else
      {
        dragActivity = null;
        tempLine = null;
      }
    }

    private void MouseSelectRelationship(RelationshipPath shape)
    {
      var r = shape.Item;
      if (r != null)
      {
        VM.CurrentRelationship = r;
        dragActivity = null;
        Cursor = null;
      }
    }

    private void MouseEditRelationship(RelationshipPath shape)
    {
      var r = shape.Item;
      if (r != null)
      {
        VM.CurrentRelationship = r;
        dragActivity = null;
        Cursor = null;
        if (VM.EditRelationshipCommand.CanExecute(null))
        {
          VM.EditRelationshipCommand.Execute(null);
        }
      }
    }

    private void MouseSelectActivity(ActivityShapeBase shape)
    {
      var a = shape.Item;
      if (a != null)
      {
        VM.CurrentActivity = a;
        if (GetMousePosition(shape) != ActivityMousePosition.Middle)
        {
          dragActivity = a.Activity;
        }
      }
    }

    private void MouseEditActivity(ActivityShapeBase shape)
    {
      var a = shape.Item;
      if (a != null)
      {
        VM.CurrentActivity = a;
        if (VM.EditActivityCommand.CanExecute(null))
        {
          VM.EditActivityCommand.Execute(null);
          dragActivity = null;
        }
      }
    }

    private void DrawCursorForActivityDrag(Shape shape)
    {
      var position = GetMousePosition(shape);
      dragActivityPosition = position;
      Uri cursorUri = null;
      if (position == ActivityMousePosition.Start)
      {
        cursorUri = new Uri(cursorUriPath + "CursorRelationshipSS.cur", UriKind.Absolute);
      }
      else if (position == ActivityMousePosition.End)
      {
        cursorUri = new Uri(cursorUriPath + "CursorRelationshipFS.cur", UriKind.Absolute);
      }
      if (cursorUri == null)
      {
        shape.Cursor = null;
      }
      else
      {
        var cursorStream = Application.GetResourceStream(cursorUri);
        shape.Cursor = new Cursor(cursorStream.Stream);
      }
    }

    private void DrawCursorForActivityDrop(Shape shape)
    {
      var position = GetMousePosition(shape);
      dropActivityPosition = position;
      Uri cursorUri = null;
      if (dragActivityPosition == ActivityMousePosition.Start)
      {
        if (position == ActivityMousePosition.Start)
        {
          cursorUri = new Uri(cursorUriPath + "CursorRelationshipSS.cur", UriKind.Absolute);
        }
        else if (position == ActivityMousePosition.End)
        {
          cursorUri = new Uri(cursorUriPath + "CursorNo.cur", UriKind.Absolute);
        }
      }
      else if (dragActivityPosition == ActivityMousePosition.End)
      {
        if (position == ActivityMousePosition.Start)
        {
          cursorUri = new Uri(cursorUriPath + "CursorRelationshipFS.cur", UriKind.Absolute);
        }
        else if (position == ActivityMousePosition.End)
        {
          cursorUri = new Uri(cursorUriPath + "CursorRelationshipFF.cur", UriKind.Absolute);
        }
      }
      if (cursorUri == null)
      {
        shape.Cursor = null;
      }
      else
      {
        var cursorStream = Application.GetResourceStream(cursorUri);
        shape.Cursor = new Cursor(cursorStream.Stream);
      }
    }

    private void DrawTempRelationship()
    {
      if (CheckPosition() == true)
      {
        if (tempLine == null)
        {
          tempLine = new Line();
          Children.Add(tempLine);
          tempLine.Stroke = Brushes.Black;
        }
        var shape = GetShapeFromActivity(dragActivity);
        tempLine.Y1 = GetTop(shape) + RowHeight / 2;
        tempLine.X1 = GetLeft(shape);
        if (dragActivityPosition == ActivityMousePosition.End)
        {
          tempLine.X1 += shape.Width;
        }

        tempLine.Y2 = Mouse.GetPosition(this).Y;
        tempLine.X2 = Mouse.GetPosition(this).X;
        tempLine.UpdateLayout();
      }
    }

    private bool CheckPosition()
    {
      var args = new CheckPositionEventArgs();
      RequestCheckPosition?.Invoke(this, args);
      return args.Result;
    }

    protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
    {
      base.OnPreviewMouseLeftButtonUp(e);
      if (e.Handled)
      {
        return;
      }

      if (!IsEnabled)
      {
        return;
      }

      if (dragActivity != null && e.OriginalSource is ActivityShapeBase)
      {
        var selectedActivity = (e.OriginalSource as ActivityShapeBase).Item.Activity;
        if (selectedActivity == null || dragActivity == selectedActivity)
        {
          if (tempLine != null)
          {
            Children.Remove(tempLine);
          }

          tempLine = null;
          dragActivity = null;
          Cursor = null;
          return;
        }
        CreateRelationship(dragActivity, selectedActivity);
      }
      Children.Remove(tempLine);
      tempLine = null;
      dragActivity = null;
      Cursor = null;
    }

    private void CreateRelationship(Activity activity1, Activity activity2)
    {
      var type = RelationshipType.FinishStart;
      if (dragActivityPosition == ActivityMousePosition.Start && dropActivityPosition == ActivityMousePosition.Start)
      {
        type = RelationshipType.StartStart;
      }
      else if (dragActivityPosition == ActivityMousePosition.End && dropActivityPosition == ActivityMousePosition.End)
      {
        type = RelationshipType.FinishFinish;
      }

      var relationshipInfo = new AddRelationshipInfo() { Activity1 = activity1, Activity2 = activity2, RelationshipType = type };
      if (VM.AddRelationshipCommand.CanExecute(relationshipInfo))
      {
        VM.AddRelationshipCommand.Execute(relationshipInfo);
      }
    }

    internal void ClearTempItems()
    {
      Children.Remove(tempLine);
      tempLine = null;
      dragActivity = null;
      Cursor = null;
    }

    #endregion

    #region Private Members

    protected double DateToX(DateTime date, bool isEndDate)
    {
      if (isEndDate)
      {
        date = date.AddDays(1).AddMinutes(-1);
      }

      return (date - VM.Schedule.FirstDay).TotalDays * VM.Zoom * 10 + tableWidth;
    }

    private double GetYOfActivity(ActivityViewModel a)
    {
      var view = ViewModelExtensions.GetView(VM.Activities);
      return view.GetRowY(a, RowHeight, GroupHeaderHeight) + ColumnHeaderHeight;
    }

    private Shape GetShapeFromActivity(ActivityViewModel activity)
    {
      return GetShapeFromActivity(activity.Activity);
    }

    private Shape GetShapeFromActivity(Activity activity)
    {
      return Children.OfType<ActivityShapeBase>().FirstOrDefault(x => x.Item.Activity == activity);
    }

    private RelationshipPath GetPathFromRelationship(RelationshipViewModel relationship)
    {
      return Children.OfType<RelationshipPath>().FirstOrDefault(x => x.Item.Relationship == relationship.Relationship);
    }

    private static ActivityMousePosition GetMousePosition(Shape shape)
    {
      double hoverArea = Math.Min(10d, shape.Width / 2);
      var pos = Mouse.GetPosition(shape);
      if (pos.X >= 0 && pos.X <= hoverArea)
      {
        return ActivityMousePosition.Start;
      }
      else if (pos.X >= shape.Width - hoverArea && pos.X <= shape.Width)
      {
        return ActivityMousePosition.End;
      }

      return ActivityMousePosition.Middle;
    }

    #endregion
  }
}
