using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using NAS.Model.Entities;
using NAS.Model.Enums;

namespace NAS.View.Shapes
{
  public class MilestoneShape : ActivityShapeBase
  {
    private Rect rect = Rect.Empty;

    public MilestoneShape(Activity milestone) : base(milestone)
    {
      Debug.Assert(milestone.ActivityType == ActivityType.Milestone);
    }

    private Geometry GetGeometry()
    {
      var point1 = new Point(rect.Width / 2, 2);
      var point2 = new Point(rect.Width - 2, rect.Height / 2);
      var point3 = new Point(rect.Width / 2, rect.Height - 2);
      var point4 = new Point(2, rect.Height / 2);
      var geometry = new StreamGeometry();
      using (var geometryContext = geometry.Open())
      {
        geometryContext.BeginFigure(point1, true, true);
        var points = new PointCollection { point2, point3, point4 };
        geometryContext.PolyLineTo(points, true, true);
      }
      geometry.Freeze();
      return geometry;
    }

    public override Geometry RenderedGeometry => GetGeometry();

    public override Transform GeometryTransform => Transform.Identity;

    protected override Geometry DefiningGeometry => GetGeometry();

    protected override Size MeasureOverride(Size constraint)
    {
      if (Stretch != Stretch.UniformToFill)
      {
        return GetNaturalSize();
      }
      double num = constraint.Width;
      double height = constraint.Height;
      if (double.IsInfinity(num) && double.IsInfinity(height))
      {
        return GetNaturalSize();
      }
      num = double.IsInfinity(num) || double.IsInfinity(height) ? Math.Min(num, height) : Math.Max(num, height);
      return new Size(num, num);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
      double strokeThickness = StrokeThickness;
      double num = strokeThickness / 2.0;
      rect = new Rect(num, num, Math.Max(0.0, finalSize.Width - strokeThickness), Math.Max(0.0, finalSize.Height - strokeThickness));
      switch (Stretch)
      {
        case Stretch.None:
          rect.Width = rect.Height = 0.0;
          break;
        case Stretch.Uniform:
          if (rect.Width > rect.Height)
          {
            rect.Width = rect.Height;
          }
          else
          {
            rect.Height = rect.Width;
          }
          break;
        case Stretch.UniformToFill:
          if (rect.Width < rect.Height)
          {
            rect.Width = rect.Height;
          }
          else
          {
            rect.Height = rect.Width;
          }
          break;
      }
      return finalSize;
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
      var pen = new Pen(Stroke, StrokeThickness);
      drawingContext.DrawGeometry(Fill, pen, GetGeometry());
    }

    private Size GetNaturalSize()
    {
      double strokeThickness = StrokeThickness;
      return new Size(strokeThickness, strokeThickness);
    }

    static MilestoneShape()
    {
      StretchProperty.OverrideMetadata(typeof(MilestoneShape), new FrameworkPropertyMetadata(Stretch.Fill));
    }
  }
}
