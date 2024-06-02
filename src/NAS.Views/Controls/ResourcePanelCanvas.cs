using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using NAS.Models.Entities;
using NAS.Models.Enums;
using NAS.ViewModels;

namespace NAS.Views.Controls
{
  public class ResourcePanelCanvas : Canvas
  {
    #region Fields

    private DateTime oldEnd;

    private enum DisplayType { ResourceAllocation, Budget, ActualCost, PlannedCost };

    #endregion

    #region Constructors

    public ResourcePanelCanvas()
    {
      DataContextChanged += Canvas_DataContextChanged;
      Height = 120;
    }

    private void Canvas_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if (e.OldValue != null)
      {
        (e.OldValue as ResourceViewModel).Refresh -= ResourcePanelCanvas_Refresh;
        (e.OldValue as ResourceViewModel).Rebuild -= ResourcePanelCanvas_Rebuild;
      }
      if (e.NewValue != null)
      {
        (e.NewValue as ResourceViewModel).Refresh += ResourcePanelCanvas_Refresh;
        (e.NewValue as ResourceViewModel).Rebuild += ResourcePanelCanvas_Rebuild;
      }
      Build();
    }

    private void ResourcePanelCanvas_Refresh(object sender, EventArgs e)
    {
      Refresh();
    }

    private void ResourcePanelCanvas_Rebuild(object sender, EventArgs e)
    {
      Build();
    }

    #endregion

    #region Properties

    private ResourceViewModel VM => DataContext as ResourceViewModel;

    public VisibleResource Resource => VM.Resource;

    public double DiagramMaxHeight { get; private set; } = 0;

    #endregion

    #region Public Methods

    private void Build()
    {
      Children.Clear();
      oldEnd = VM.End;
      AddResourceRectangles();
      AddLimitLine();
      Refresh();
    }

    private void AddResourceRectangles()
    {
      var values = Enum.GetValues(typeof(DisplayType)).Cast<DisplayType>().ToList();
      foreach (var day in VM.ResourceAllocation.Keys)
      {
        values.ForEach(x => AddRect(day, x));
      }
    }

    private void AddLimitLine()
    {
      var line = new Line();
      line.Stroke = Brushes.Black;
      line.Visibility = Visibility.Hidden;
      line.Tag = "Limit";
      Children.Add(line);
    }

    public void Refresh()
    {
      if (VM == null || Resource == null)
      {
        return;
      }

      if (oldEnd != VM.End)
      {
        Build();
        return;
      }
      DiagramMaxHeight = VM.ResourceMax;

      Width = DateToX(VM.End) + 10 * VM.Zoom + 10;

      foreach (var rect in Children.OfType<Rectangle>().Where(x => x.Tag is Tuple<DateTime, DisplayType>))
      {
        if (rect.Tag is not Tuple<DateTime, DisplayType> tag)
        {
          continue;
        }

        if (tag.Item2 == DisplayType.ResourceAllocation)
        {
          if (Resource.ShowResourceAllocation)
          {
            UpdateRect(rect, VM.ResourceAllocation[tag.Item1], tag.Item1);
          }
          else
          {
            rect.Visibility = Visibility.Hidden;
          }
        }
        else if (tag.Item2 == DisplayType.Budget)
        {
          if (Resource.ShowBudget)
          {
            UpdateRect(rect, Convert.ToDouble(VM.ResourceBudget[tag.Item1]), tag.Item1);
          }
          else
          {
            rect.Visibility = Visibility.Hidden;
          }
        }
        else if (tag.Item2 == DisplayType.ActualCost)
        {
          if (Resource.ShowActualCosts)
          {
            UpdateRect(rect, Convert.ToDouble(VM.ResourceCostsActual[tag.Item1]), tag.Item1);
          }
          else
          {
            rect.Visibility = Visibility.Hidden;
          }
        }
        else if (tag.Item2 == DisplayType.PlannedCost)
        {
          if (Resource.ShowPlannedCosts)
          {
            UpdateRect(rect, Convert.ToDouble(VM.ResourceCostsPlanned[tag.Item1]), tag.Item1);
          }
          else
          {
            rect.Visibility = Visibility.Hidden;
          }
        }
      }
      foreach (var line in Children.OfType<Line>().Where(x => x.Tag != null && x.Tag.ToString() == "Limit"))
      {
        if (Resource.ShowResourceAllocation && VM.Resource.Resource.Limit.HasValue)
        {
          line.X1 = 0;
          line.X2 = Width;
          line.Y1 = Height - VerticalZoom * VM.Resource.Resource.Limit.Value;
          line.Y2 = line.Y1;
          line.Visibility = Visibility.Visible;
        }
        else
        {
          line.Visibility = Visibility.Hidden;
        }
      }
    }

    #endregion

    #region Private Members

    private Dictionary<DisplayType, Brush> brushCache;

    private Brush GetBrush(DisplayType display)
    {
      brushCache ??= new Dictionary<DisplayType, Brush>();

      if (brushCache.ContainsKey(display))
      {
        return brushCache[display];
      }
      else
      {
        Brush brush = Brushes.Black;
        switch (display)
        {
          case DisplayType.ResourceAllocation:
            brush = (Brush)Application.Current.Resources["ResourceAllocationColor"];
            break;
          case DisplayType.Budget:
            brush = (Brush)Application.Current.Resources["BudgetColor"];
            break;
          case DisplayType.ActualCost:
            brush = (Brush)Application.Current.Resources["ActualCostsColor"];
            break;
          case DisplayType.PlannedCost:
            brush = (Brush)Application.Current.Resources["PlannedCostsColor"];
            break;
        }
        brushCache.Add(display, brush);
        return brush;
      }
    }

    private void AddRect(DateTime day, DisplayType display)
    {
      var rect = new Rectangle();
      rect.Tag = new Tuple<DateTime, DisplayType>(day, display);
      rect.Fill = GetBrush(display);
      rect.Visibility = Visibility.Hidden;
      Children.Add(rect);
    }

    private void UpdateRect(Rectangle rect, double resourceAmount, DateTime date)
    {
      if (resourceAmount > 0)
      {
        rect.Height = VerticalZoom * resourceAmount;
        rect.Visibility = Visibility.Visible;

        var end = GetEndDate(date);

        rect.Width = Math.Max(10 * VM.Zoom, DateToX(end) - DateToX(date));

        double x = DateToX(date);
        SetLeft(rect, x);
        double y = Height - rect.Height;
        SetTop(rect, y);
      }
      else
      {
        rect.Visibility = Visibility.Hidden;
      }
    }

    private DateTime GetEndDate(DateTime date)
    {
      if (VM.AggregationType == TimeAggregateType.Day)
      {
        return date;
      }

      var end = VM.End;
      var remainingDays = VM.ResourceAllocation.SkipWhile(day => day.Key <= date).Select(day => day.Key);
      if (remainingDays.Any())
      {
        end = remainingDays.First();
      }

      return end;
    }

    private double VerticalZoom => DiagramMaxHeight == 0 ? 1 : Height / DiagramMaxHeight;

    private double DateToX(DateTime date)
    {
      return (date - VM.Start).TotalDays * VM.Zoom * 10 + 10;
    }

    #endregion
  }
}
