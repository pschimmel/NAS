using System.Windows;
using System.Windows.Media;
using NAS.ViewModel;

namespace NAS.View.Shapes
{
  public class ActivityShape : ActivityShapeBase
  {
    private Rect _rect = Rect.Empty;
    private readonly double radius = 2;

    public ActivityShape(ActivityViewModel activity) : base(activity)
    { }

    public override Geometry RenderedGeometry => new RectangleGeometry(_rect, radius, radius);

    public override Transform GeometryTransform => Transform.Identity;

    protected override Geometry DefiningGeometry => new RectangleGeometry(_rect, radius, radius);

    protected override Size MeasureOverride(Size constraint)
    {
      if (Stretch != Stretch.UniformToFill)
      {
        return getNaturalSize();
      }
      double num = constraint.Width;
      double height = constraint.Height;
      if (double.IsInfinity(num) && double.IsInfinity(height))
      {
        return getNaturalSize();
      }
      num = double.IsInfinity(num) || double.IsInfinity(height) ? Math.Min(num, height) : Math.Max(num, height);
      return new Size(num, num);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
      double strokeThickness = StrokeThickness;
      double num = strokeThickness / 2.0;
      _rect = new Rect(num, num, Math.Max(0.0, finalSize.Width - strokeThickness), Math.Max(0.0, finalSize.Height - strokeThickness));
      switch (Stretch)
      {
        case Stretch.None:
          _rect.Width = _rect.Height = 0.0;
          break;
        case Stretch.Uniform:
          if (_rect.Width > _rect.Height)
          {
            _rect.Width = _rect.Height;
          }
          else
          {
            _rect.Height = _rect.Width;
          }
          break;
        case Stretch.UniformToFill:
          if (_rect.Width < _rect.Height)
          {
            _rect.Width = _rect.Height;
          }
          else
          {
            _rect.Height = _rect.Width;
          }
          break;
      }
      return finalSize;
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
      var pen = new Pen(Stroke, StrokeThickness);
      drawingContext.DrawRoundedRectangle(Fill, pen, _rect, radius, radius);
    }

    private Size getNaturalSize()
    {
      double strokeThickness = StrokeThickness;
      return new Size(strokeThickness, strokeThickness);
    }

    static ActivityShape()
    {
      StretchProperty.OverrideMetadata(typeof(ActivityShape), new FrameworkPropertyMetadata(Stretch.Fill));
    }
  }
}
