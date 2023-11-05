using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ES.Tools.Adorners;
using NAS.Model.Base;
using NAS.Model.Entities;
using NAS.Model.Enums;
using NAS.View.Helpers;
using NAS.View.Shapes;
using NAS.ViewModel;

namespace NAS.View.Controls
{
  public class PERTCanvas : DiagramCanvasBase
  {
    #region Events

    public event EventHandler<CheckPositionEventArgs> RequestCheckPosition;

    #endregion

    #region Fields

    private Color activityStandardColor = Colors.Yellow;
    private Color activityCriticalColor = Colors.Red;
    private Color activityDoneColor = Colors.Blue;
    private Color milestoneStandardColor = Colors.Black;
    private Color milestoneCriticalColor = Colors.Red;
    private Color milestoneDoneColor = Colors.Blue;
    private Color dataDateColor = Colors.Blue;
    private Line tempLine = null;
    private DataTemplateAdorner dragAdorner;
    private PERTActivityData dragActivityData;
    private PERTActivityData[,] matrix;
    private PERTDefinition template;
    private bool suspendRefreshing = false;
    private Layout layout;

    #endregion

    #region Constructors

    public PERTCanvas()
    {
      DataContextChanged += Canvas_DataContextChanged;
    }

    #endregion

    #region Public Properties

    protected bool IsStandalone => this is IPrintableCanvas;

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
        }
        Refresh();
      }
    }

    #endregion

    #region ViewModel

    private ScheduleViewModel VM => DataContext as ScheduleViewModel;

    private void Canvas_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      suspendRefreshing = true;
      if (e.OldValue is ScheduleViewModel oldVM)
      {
        oldVM.PropertyChanged -= ViewModel_PropertyChanged;
        oldVM.ActivityAdded -= ViewModel_ActivityAdded;
        oldVM.ActivityDeleted -= ViewModel_ActivityDeleted;
        oldVM.RelationshipAdded -= ViewModel_RelationshipAdded;
        oldVM.RelationshipDeleted -= ViewModel_RelationshipDeleted;
        oldVM.RefreshLayout -= ViewModel_LayoutChanged;
        Layout = null;
      }

      if (e.NewValue is ScheduleViewModel vm)
      {
        vm.PropertyChanged += ViewModel_PropertyChanged;
        vm.ActivityAdded += ViewModel_ActivityAdded;
        vm.ActivityDeleted += ViewModel_ActivityDeleted;
        vm.RelationshipAdded += ViewModel_RelationshipAdded;
        vm.RelationshipDeleted += ViewModel_RelationshipDeleted;
        vm.RefreshLayout += ViewModel_LayoutChanged;
        Layout = vm.Schedule.CurrentLayout;
      }
      suspendRefreshing = false;
      Refresh();
    }

    private void ViewModel_ActivityAdded(object sender, ItemEventArgs<Activity> e)
    {
      var activity = e.Item;
      var data = activity.Schedule.GetOrAddActivityData(activity);
      AddActivity(data);
      VM.CurrentActivity = activity;
    }

    private void ViewModel_ActivityDeleted(object sender, ItemEventArgs<Activity> e)
    {
      RemoveActivity(e.Item);
    }

    private void ViewModel_RelationshipAdded(object sender, ItemEventArgs<Relationship> e)
    {
      AddRelationship(e.Item);
    }

    private void ViewModel_RelationshipDeleted(object sender, ItemEventArgs<Relationship> e)
    {
      RemoveRelationship(e.Item);
    }

    private void ViewModel_LayoutChanged(object sender, ItemEventArgs<Layout> e)
    {
      Layout = VM.Schedule.CurrentLayout;
    }

    private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!IsLoaded)
      {
        return;
      }

      if (e.PropertyName == "CurrentActivity")
      {
        Select<PERTDiagram, Activity>(VM.CurrentActivity);
      }
      else if (e.PropertyName == "CurrentRelationship")
      {
        Select<RelationshipPath, Relationship>(VM.CurrentRelationship);
      }
    }

    #endregion

    #region Refresh

    public override void Refresh()
    {
      if (suspendRefreshing || layout == null)
      {
        return;
      }

      Children.Clear();
      if (VM.Schedule != null)
      {
        // Load Template
        if (Layout.LayoutType == Model.Enums.LayoutType.PERT)
        {
          template = Layout.PERTDefinition;
        }
        // Draw Activities
        var view = ViewModelExtensions.GetView(VM.Activities);
        if (view != null)
        {
          // Filtering
          if (view.CanFilter)
          {
            view.Filter = x => Contains(x);
          }

          var visibleActivities = view.Cast<Activity>().ToList();
          int matrixX = visibleActivities.Any() ? Math.Max(visibleActivities.Max(x => x.GetActivityData().LocationX), 3) : 1;
          int matrixY = visibleActivities.Any() ? Math.Max(visibleActivities.Max(x => x.GetActivityData().LocationY), 3) : 1;
          if (matrixX * matrixY < visibleActivities.Count)
          {
            matrixX = Convert.ToInt32(Math.Sqrt(Math.Ceiling(Convert.ToDouble(visibleActivities.Count))));
            matrixY = matrixX;
          }
          matrix = new PERTActivityData[matrixX + 2, matrixY + 2];
          foreach (var a in visibleActivities)
          {
            AddActivity(a.GetActivityData());
          }

          // Draw Relationships
          foreach (var r in VM.Schedule.Relationships)
          {
            if (visibleActivities.Contains(r.GetActivity1()) && visibleActivities.Contains(r.GetActivity2()))
            {
              AddRelationship(r);
            }
            else
            {
              RemoveRelationship(r);
            }
          }
        }
        Width = (template.Width + template.SpacingX) * matrix.GetLength(0) + template.SpacingX;
        Height = (template.Height + template.SpacingX) * matrix.GetLength(1) + template.SpacingY;
      }
    }

    private bool Contains(object obj)
    {
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

        if (Layout.FilterDefinitions.Count == 0)
        {
          return true;
        }

        foreach (var filter in Layout.FilterDefinitions)
        {
          bool isTrue = filter.Compare(activity);
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

    #endregion

    #region Activities

    private void AddActivity(PERTActivityData a)
    {
      if (a.LocationX >= matrix.GetLength(0))
      {
        a.LocationX = -1;
      }

      if (a.LocationY >= matrix.GetLength(1))
      {
        a.LocationY = -1;
      }

      if (matrix[a.LocationX, a.LocationY] != null)
      {
        a.LocationX = -1;
        a.LocationY = -1;
      }
      if (a.LocationX == -1 || a.LocationY == -1)
      {
        for (int x = 0; (a.LocationX == -1 || a.LocationY == -1) && x < matrix.GetLength(0); x++)
        {
          for (int y = 0; (a.LocationX == -1 || a.LocationY == -1) && y < matrix.GetLength(1); y++)
          {
            if (matrix[x, y] == null)
            {
              a.LocationX = x;
              a.LocationY = y;
            }
          }
        }
      }
      matrix[a.LocationX, a.LocationY] = a;
      var rect = GetActivityDiagram(a.GetActivity());
      a.PropertyChanged += (sender, args) =>
      {
        var activity = sender as Activity;
        if (args.PropertyName == "IsStarted" || args.PropertyName == "IsFinished" || args.PropertyName == "IsCritical")
        {
          RefreshActivity(activity);
          foreach (var predecessor in activity.GetPreceedingRelationships())
          {
            RefreshRelationship(predecessor);
          }

          foreach (var successor in activity.GetSucceedingRelationships())
          {
            RefreshRelationship(successor);
          }
        }
      };
      Children.Add(rect);
      SetZIndex(rect, 3);
      RefreshActivity(a.GetActivity());
    }

    private PERTDiagram GetActivityDiagram(Activity activity)
    {
      var dia = new PERTDiagram(activity);
      dia.Width = template.Width;
      dia.Height = template.Height;
      foreach (var row in template.RowDefinitions)
      {
        var definition = new System.Windows.Controls.RowDefinition();
        definition.Height = row.Height.HasValue ? new GridLength(row.Height.Value) : new GridLength(1, GridUnitType.Star);
        dia.RowDefinitions.Add(definition);
      }
      foreach (var column in template.ColumnDefinitions)
      {
        var definition = new System.Windows.Controls.ColumnDefinition();
        definition.Width = column.Width.HasValue ? new GridLength(column.Width.Value) : new GridLength(1, GridUnitType.Star);
        dia.ColumnDefinitions.Add(definition);
      }
      return dia;
    }

    private void RemoveActivity(Activity activity)
    {
      var dia = GetDiagramFromActivity(activity);
      if (dia != null)
      {
        while (Children.OfType<Line>().Any(l => l.Tag == activity))
        {
          Children.Remove(Children.OfType<Line>().First(l => l.Tag == activity));
        }

        while (Children.OfType<Image>().Any(i => i.Tag == activity))
        {
          Children.Remove(Children.OfType<Image>().First(i => i.Tag == activity));
        }

        Children.Remove(dia);
        for (int x = 0; x < matrix.GetLength(0); x++)
        {
          for (int y = 0; y < matrix.GetLength(1); y++)
          {
            if (matrix[x, y].ActivityGuid == activity.Guid)
            {
              matrix[x, y] = null;
            }
          }
        }
      }
    }

    private void RefreshActivity(Activity a)
    {
      if (a == null)
      {
        return;
      }

      var dia = GetDiagramFromActivity(a);
      dia.Children.Clear();
      double x;
      double y;
      if (dia != null)
      {
        y = GetYOfActivity(a);
        SetTop(dia, y);
        x = GetXOfActivity(a);
        SetLeft(dia, x);
        if (a.ActivityType == ActivityType.Milestone)
        {
          dia.Background = new SolidColorBrush(milestoneStandardColor);
          if (a.IsCritical)
          {
            dia.BorderBrush = new SolidColorBrush(milestoneCriticalColor);
          }
        }
        else
        {
          dia.Background = new SolidColorBrush(activityStandardColor);
          if (a.IsCritical)
          {
            dia.BorderBrush = new SolidColorBrush(activityCriticalColor);
          }
        }

        // Apply Text Values
        foreach (var item in template.Items)
        {
          var tb = new TextBlock();
          BindingOperations.SetBinding(tb, TextBlock.TextProperty, a.GetBindingFromActivity(item.Property));
          var b = new Border();
          b.Child = tb;
          b.BorderBrush = Brushes.Gray;
          double left = 0.5;
          double top = 0.5;
          double right = 0.5;
          double bottom = 0.5;
          if (item.Row == 0)
          {
            top = 0;
          }

          if (item.Column == 0)
          {
            left = 0;
          }

          if (item.Column + item.ColumnSpan == dia.ColumnDefinitions.Count)
          {
            right = 0;
          }

          if (item.Row + item.RowSpan == dia.RowDefinitions.Count)
          {
            bottom = 0;
          }

          b.BorderThickness = new Thickness(left, top, right, bottom);
          dia.Children.Add(b);
          b.SetValue(Grid.RowProperty, item.Row);
          b.SetValue(Grid.ColumnProperty, item.Column);
          b.SetValue(Grid.RowSpanProperty, item.RowSpan);
          b.SetValue(Grid.ColumnSpanProperty, item.ColumnSpan);
          tb.TextWrapping = TextWrapping.Wrap;
          tb.TextTrimming = TextTrimming.CharacterEllipsis;
          tb.FontSize = template.FontSize;
          switch (item.HorizontalAlignment)
          {
            case Model.Enums.HorizontalAlignment.Left:
              tb.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
              break;
            case Model.Enums.HorizontalAlignment.Center:
              tb.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
              break;
            case Model.Enums.HorizontalAlignment.Right:
              tb.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
              break;
          }
          switch (item.VerticalAlignment)
          {
            case Model.Enums.VerticalAlignment.Top:
              tb.VerticalAlignment = System.Windows.VerticalAlignment.Top;
              break;
            case Model.Enums.VerticalAlignment.Center:
              tb.VerticalAlignment = System.Windows.VerticalAlignment.Center;
              break;
            case Model.Enums.VerticalAlignment.Bottom:
              tb.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
              break;
          }
        }

        // Apply line for started and finished activities
        while (Children.OfType<Line>().Any(l => l.Tag == a))
        {
          Children.Remove(Children.OfType<Line>().First(l => l.Tag == a));
        }

        if (a.IsStarted)
        {
          var line = new Line();
          line.Stroke = Brushes.Black;
          line.StrokeThickness = 1;
          Children.Add(line);              //     /
          line.X1 = x;                     //    /
          line.Y1 = y + template.Height;   //   /
          line.X2 = x + template.Width;    //  /
          line.Y2 = y;
          line.Tag = a;
          SetZIndex(line, 4);
        }
        if (a.IsFinished)
        {
          var line = new Line();
          line.Stroke = Brushes.Black;
          line.StrokeThickness = 1;
          Children.Add(line);              //  \
          line.X1 = x;                     //   \
          line.Y1 = y;                     //    \
          line.X2 = x + template.Width;    //     \
          line.Y2 = y + template.Height;
          line.Tag = a;
          SetZIndex(line, 4);
        }

        // Apply Distortion Icon
        if (a.ActivityType == ActivityType.Activity && a.Distortions != null && a.Distortions.Count > 0)
        {
          var image = Children.OfType<Image>().FirstOrDefault(i => i.Tag == a);
          if (image == null)
          {
            image = new Image() { Source = new BitmapImage(new Uri("pack://application:NAS,,,/images/Distortion.png")), Height = 16, Width = 16 };
            image.Tag = a;
            Children.Add(image);
            image.IsHitTestVisible = false;
            string s = "";
            foreach (var d in a.Distortions)
            {
              if (!string.IsNullOrEmpty(s))
              {
                s += Environment.NewLine;
              }

              s += d.ToString();
            }
          }
          SetZIndex(image, 2);
          SetTop(image, GetTop(dia) + dia.Height / 2 - 8);
          SetLeft(image, GetLeft(dia) + dia.Width + 2);
        }
        else
        {
          var image = Children.OfType<Image>().FirstOrDefault(i => i.Tag == a);
          if (image != null)
          {
            Children.Remove(image);
          }
        }
        dia.ToolTip = a.Name + " (" + a.StartDate.ToShortDateString() + " - " + a.FinishDate.ToShortDateString() + ")";
      }
    }

    #endregion

    #region Relationships

    private void AddRelationship(Relationship r)
    {
      if (!Layout.ShowRelationships)
      {
        return;
      }

      var path = new RelationshipPath(r);
      path.Stroke = r.IsCritical ? new SolidColorBrush(activityCriticalColor) : r.IsDriving ? Brushes.Black : Brushes.DarkGray;

      if (!IsStandalone)
      {
        r.PropertyChanged += (sender, args) => RefreshRelationship(sender as Relationship);
        path.ToolTip = r.ToString();
      }

      Children.Insert(0, path);
      SetZIndex(path, 1);
      RefreshRelationship(r);
    }

    private void RemoveRelationship(Relationship relation)
    {
      foreach (var e in Children.OfType<RelationshipPath>().Where(x => x.Item == relation).ToList())
      {
        Children.Remove(e);
      }
    }

    private enum PointType { Left, Right };

    private void RefreshRelationship(Relationship r)
    {
      if (!Layout.ShowRelationships)
      {
        return;
      }

      var path = GetPathFromRelationship(r);
      var diagramm1 = GetDiagramFromActivity(r.GetActivity1());
      var diagramm2 = GetDiagramFromActivity(r.GetActivity2());
      if (path == null || diagramm1 == null || diagramm2 == null)
      {
        return;
      }

      var startPoint = new Point(GetLeft(diagramm1), GetTop(diagramm1) + template.Height / 2);
      var endPoint = new Point(GetLeft(diagramm2), GetTop(diagramm2) + template.Height / 2);
      var startPointType = PointType.Left;
      var endPointType = PointType.Left;
      switch (r.RelationshipType)
      {
        case RelationshipType.FinishStart:
          startPoint.X += diagramm1.Width;
          startPointType = PointType.Right;
          break;
        case RelationshipType.FinishFinish:
          startPoint.X += diagramm1.Width;
          startPointType = PointType.Right;
          endPoint.X += diagramm2.Width;
          endPointType = PointType.Right;
          break;
        case RelationshipType.StartStart:
          // Do nothing
          break;
      }
      double spacing = Math.Max(2, Math.Round(template.SpacingX / 2 - 2));
      int startPointSign = startPointType == PointType.Right ? 1 : -1;
      int endPointSign = endPointType == PointType.Right ? 1 : -1;
      var sb = new StringBuilder();
      // Start Point
      sb.AppendPoint(startPoint.X, startPoint.Y);
      // Spacing
      sb.AppendPoint(startPoint.X + startPointSign * spacing, startPoint.Y);
      // Optional correction
      if (startPointType == PointType.Right && startPoint.X > endPoint.X || startPointType == PointType.Left && startPoint.X < endPoint.X)
      {
        if (startPoint.Y < endPoint.Y)
        {
          sb.AppendPoint(startPoint.X + startPointSign * spacing, startPoint.Y + template.Height / 2 + template.SpacingY / 2);
        }
        else
        {
          sb.AppendPoint(startPoint.X + startPointSign * spacing, startPoint.Y - template.Height / 2 - template.SpacingY / 2);
        }
      }
      if (endPointType == PointType.Left && startPoint.X > endPoint.X || endPointType == PointType.Right && startPoint.X < endPoint.X)
      {
        if (startPoint.Y > endPoint.Y)
        {
          sb.AppendPoint(endPoint.X + endPointSign * spacing, endPoint.Y + template.Height / 2 + template.SpacingY / 2);
        }
        else
        {
          sb.AppendPoint(endPoint.X + endPointSign * spacing, endPoint.Y - template.Height / 2 - template.SpacingY / 2);
        }
      }
      // Connection
      sb.AppendPoint(endPoint.X + endPointSign * spacing, endPoint.Y);
      // Spacing
      sb.AppendPoint(endPoint.X, endPoint.Y);
      // Arrow
      sb.AppendPoint(endPoint.X + endPointSign * 4, endPoint.Y - 3);
      sb.AppendPoint(endPoint.X, endPoint.Y);
      sb.AppendPoint(endPoint.X + endPointSign * 4, endPoint.Y + 3);
      path.Data = Geometry.Parse(sb.ToString());
    }

    #endregion

    #region Mouse Interaction

    protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
    {
      base.OnPreviewMouseLeftButtonDown(e);
      if (e.Handled || !IsEnabled)
      {
        return;
      }

      if (e.OriginalSource is PERTDiagram || e.OriginalSource is TextBlock)
      {
        var diagramm = e.OriginalSource is PERTDiagram ? e.OriginalSource : (e.OriginalSource as TextBlock).GetParent<PERTDiagram>();
        if (diagramm == null)
        {
          return;
        }

        var activity = diagramm.Item;
        dragActivityData = activity.GetActivityData();

        var rect = new Rect(diagramm.RenderSize);
        if (rect.Width > 10 && rect.Height > 10)
        {
          rect.Inflate(-10, -10);
        }

        // Move activity if mouse click is inside of the diagram
        if (rect.Contains(Mouse.GetPosition(diagramm)))
        {
          var template =  (DataTemplate)TryFindResource("PERTDragDrop");
          var tempDiagram = GetActivityDiagram(activity);
          var dummy = new PERTDiagramDummy { Width = tempDiagram.Width, Height = tempDiagram.Height };
          dragAdorner = new DataTemplateAdorner(this, dummy, template);
          var position = e.GetPosition(this);
          dragAdorner.UpdatePosition(position.X - tempDiagram.Width / 2, position.Y - tempDiagram.Height / 2);
          return;
        }
        else
        {
          // Draw Relationship
          if (activity == null)
          {
            return;
          }

          VM.CurrentActivity = activity;
          if (e.ClickCount == 2 && VM.EditActivityCommand.CanExecute(null))
          {
            // Edit Details
            VM.EditActivityCommand.Execute(null);
            dragActivityData = null;
          }
          else
          {
            dragActivityData = activity.GetActivityData();
          }

          return;
        }
      }
      else if (e.OriginalSource is RelationshipPath)
      {
        VM.CurrentRelationship = (e.OriginalSource as RelationshipPath).Item;
        dragActivityData = null;
        Cursor = null;
        if (e.ClickCount == 2 && VM.EditRelationshipCommand.CanExecute(null))
        {
          VM.EditRelationshipCommand.Execute(null);
        }

        return;
      }
      dragActivityData = null;
      Cursor = null;
      VM.CurrentActivity = null;
      VM.CurrentRelationship = null;
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
      base.OnMouseMove(e);
      if (e.Handled || !IsEnabled)
      {
        return;
      }

      if (dragActivityData != null)
      {
        if (dragAdorner != null)
        {
          // Move Activity
          var position = e.GetPosition(this);
          dragAdorner.UpdatePosition(position.X - dragAdorner.ActualWidth / 2, position.Y - dragAdorner.ActualHeight / 2);
        }
        else
        {
          // Draw Relationships with Mouse
          if (CheckPosition() == false)
          {
            return;
          }

          if (tempLine == null)
          {
            tempLine = new Line();
            Children.Add(tempLine);
            tempLine.Stroke = Brushes.Black;
          }

          var activity = dragActivityData.GetActivity();
          var diagram = GetDiagramFromActivity(activity);
          tempLine.Y1 = GetTop(diagram) + template.Height / 2;
          tempLine.X1 = GetLeft(diagram);
          if (!(activity.ActivityType == ActivityType.Milestone))
          {
            tempLine.X1 += diagram.Width;
          }

          tempLine.Y2 = Mouse.GetPosition(this).Y;
          tempLine.X2 = Mouse.GetPosition(this).X;
          Cursor = Cursors.Cross;
          tempLine.UpdateLayout();
        }
      }
    }

    private bool CheckPosition()
    {
      var args = new CheckPositionEventArgs();
      RequestCheckPosition?.Invoke(this, args);
      return args.Result;
    }

    internal void ClearTempItems()
    {
      Children.Remove(tempLine);
      StopDrag();
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

      if (dragAdorner != null && dragActivityData != null)
      {
        // Move Activity
        // find closest position
        var position = e.GetPosition(this);
        int tempX = -1;
        int tempY = -1;
        double tempDistance = double.MaxValue;
        for (int x = 0; x < matrix.GetLength(0); x++)
        {
          for (int y = 0; y < matrix.GetLength(1); y++)
          {
            double distanceX = template.SpacingX + x * (template.Width + template.SpacingX) + template.Width / 2;
            double distanceY = template.SpacingY + y * (template.Height + template.SpacingY) + template.Height / 2;
            double distance = Math.Sqrt((distanceX - position.X) * (distanceX - position.X) + (distanceY - position.Y) * (distanceY - position.Y));
            if (tempDistance > distance)
            {
              tempDistance = distance;
              tempX = x;
              tempY = y;
            }
          }
        }
        if (tempX != -1 && tempY != -1)
        {
          if (matrix[tempX, tempY] == null)
          { // Location is not occupied
            matrix[dragActivityData.LocationX, dragActivityData.LocationY] = null;
            matrix[tempX, tempY] = dragActivityData;
            dragActivityData.LocationX = tempX;
            dragActivityData.LocationY = tempY;

            var activity = dragActivityData.GetActivity();
            RefreshActivity(activity);

            foreach (var rel in activity.GetVisiblePreceedingRelationships())
            {
              RefreshRelationship(rel);
            }

            foreach (var rel in activity.GetVisibleSucceedingRelationships())
            {
              RefreshRelationship(rel);
            }
          }
        }
      }
      else if (dragActivityData != null && (e.OriginalSource is PERTDiagram || e.OriginalSource is TextBlock))
      {
        var activity = dragActivityData.GetActivity();

        // Draw relationship
        var diagramm = e.OriginalSource is PERTDiagram ? e.OriginalSource : (e.OriginalSource as TextBlock).GetParent<PERTDiagram>();
        var selectedActivity = diagramm.Item;
        if (selectedActivity == null)
        {
          return;
        }

        if (activity != selectedActivity && VM.AddRelationshipCommand.CanExecute(null))
        {
          VM.AddRelationshipCommand.Execute(new AddRelationshipInfo() { Activity1 = activity, Activity2 = selectedActivity });
        }
      }
      StopDrag();
    }

    public void StopDrag()
    {
      Children.Remove(tempLine);
      tempLine = null;
      if (dragAdorner != null)
      {
        dragAdorner.Dispose();
        dragAdorner = null;
      }
      dragActivityData = null;
      Cursor = null;
    }

    #endregion

    #region Private Members

    private double GetXOfActivity(Activity activity)
    {
      var a = activity.GetActivityData();
      return template.SpacingX + a.LocationX * (template.Width + template.SpacingX);
    }

    private double GetYOfActivity(Activity activity)
    {
      var a = activity.GetActivityData();
      return template.SpacingY + a.LocationY * (template.Height + template.SpacingY);
    }

    private PERTDiagram GetDiagramFromActivity(Activity activity)
    {
      return Children.OfType<PERTDiagram>().FirstOrDefault(x => x.Item == activity);
    }

    private RelationshipPath GetPathFromRelationship(Relationship relationship)
    {
      return Children.OfType<RelationshipPath>().FirstOrDefault(x => x.Item == relationship);
    }

    #endregion
  }
}
