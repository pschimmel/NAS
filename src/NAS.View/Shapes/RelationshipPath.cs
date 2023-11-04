using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using NAS.Model.Entities;

namespace NAS.View.Shapes
{
  /// <summary>
  /// The Path shape element for Relationships
  /// </summary>
  public sealed class RelationshipPath : Shape, IActivityDiagram<Relationship>
  {
    #region Constructors

    /// <summary>
    /// Instantiates a new instance of a Path.
    /// </summary>
    public RelationshipPath(Relationship relationship)
    {
      StrokeLineJoin = PenLineJoin.Round;
      StrokeEndLineCap = PenLineCap.Round;
      StrokeStartLineCap = PenLineCap.Round;
      StrokeThickness = 2;
      Item = relationship;
    }

    #endregion Constructors

    #region Properties

    public Relationship Item { get; private set; }

    #endregion

    #region Dynamic Properties

    /// <summary>
    /// Data property
    /// </summary>
    public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(Geometry), typeof(RelationshipPath), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender), null);

    /// <summary>
    /// Data property
    /// </summary>
    public Geometry Data
    {
      get => (Geometry)GetValue(DataProperty);
      set => SetValue(DataProperty, value);
    }
    #endregion

    #region Protected Methods and Properties

    /// <summary>
    /// Get the path that defines this shape
    /// </summary>
    protected override Geometry DefiningGeometry => Data ?? Geometry.Empty;

    protected override void OnRender(DrawingContext drawingContext)
    {
      if (DefiningGeometry != Geometry.Empty)
      {
        drawingContext.DrawGeometry(Fill, new Pen(Brushes.Transparent, 10), DefiningGeometry);
      }
      base.OnRender(drawingContext);
    }

    #endregion
  }
}