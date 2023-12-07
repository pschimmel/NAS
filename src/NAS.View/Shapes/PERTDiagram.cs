using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using NAS.ViewModel;

namespace NAS.View.Shapes
{
  public class PERTDiagram : Border, IActivityDiagram<ActivityViewModel>
  {
    private readonly Grid grid;

    public PERTDiagram(ActivityViewModel activity)
    {
      BorderBrush = Brushes.Black;
      Background = Brushes.LightYellow;
      CornerRadius = new CornerRadius(5);
      BorderThickness = new Thickness(1);
      grid = new Grid();
      grid.IsHitTestVisible = false;
      grid.Focusable = false;
      base.Child = grid;
      Item = activity;
    }

    public ActivityViewModel Item { get; private set; }

    public override UIElement Child
    {
      get => base.Child;
      set
      {
        if (value is not Grid)
        {
          throw new InvalidOperationException();
        }

        base.Child = value;
      }
    }

    public RowDefinitionCollection RowDefinitions => grid.RowDefinitions;

    public ColumnDefinitionCollection ColumnDefinitions => grid.ColumnDefinitions;

    public UIElementCollection Children => grid.Children;
  }
}
