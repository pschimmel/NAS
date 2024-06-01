using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using NAS.ViewModels.Helpers;

namespace NAS.ViewModels.Printing
{
  /// <summary>
  /// Paginator for canvas printing
  /// </summary>
  public class NASDocumentPaginator : DocumentPaginator
  {
    public event EventHandler<ProgressEventArgs> Progress;

    private readonly Canvas canvas;
    private Size pageSize;
    private Dictionary<int, DocumentPage> pageBuffer;

    /// <summary>
    /// Initializes a new instance of the <see cref="NASDocumentPaginator"/> class.
    /// </summary>
    /// <param number="canvas">The canvas.</param>
    public NASDocumentPaginator(Canvas canvas, PageMediaSize size, PageImageableArea printableArea, PageOrientation orientation)
    {
      if (size == null)
      {
        size = new PageMediaSize(PageMediaSizeName.ISOA4);
        try
        {
          var printer = LocalPrintServer.GetDefaultPrintQueue();
          PrintableArea = printer.GetPrintCapabilities().PageImageableArea;
        }
        catch { }
        Orientation = PageOrientation.Portrait;
      }

      pageSize = orientation is PageOrientation.Landscape or PageOrientation.ReverseLandscape
          ? new Size(size.Height.Value, size.Width.Value)
          : new Size(size.Width.Value, size.Height.Value);

      PrintableArea = printableArea;
      Orientation = orientation;

      this.canvas = canvas ?? new Canvas();
    }

    /// <summary>
    /// Gets a count of the number of pages currently formatted
    /// </summary>
    /// <returns>A count of the number of pages that have been formatted.</returns>
    public override int PageCount => PagesX * PagesY;

    /// <summary>
    /// Gets or sets the suggested width and height of each page.
    /// </summary>
    /// <returns>A <see cref="T:System.Windows.Size"/> representing the width and height of each page.</returns>
    public override Size PageSize
    {
      get => pageSize;
      set
      {
        var newSize=Orientation is PageOrientation.Landscape or PageOrientation.ReversePortrait
          ? pageSize.Height > pageSize.Width ? new Size(value.Height, value.Width) : value
          : pageSize.Height > pageSize.Width ? value : new Size(value.Height, value.Width);

        if (pageSize != newSize)
        {
          pageSize = newSize;
          pageBuffer = null;
        }
      }
    }

    /// <summary>
    /// Gets or sets the printable area.
    /// </summary>
    /// <value>
    /// The printable area.
    /// </value>
    public PageImageableArea PrintableArea { get; set; }

    /// <summary>
    /// Gets or sets the orientation.
    /// </summary>
    /// <value>
    /// The orientation.
    /// </value>
    public PageOrientation Orientation { get; set; }

    /// <summary>
    /// Gets a value indicating whether <see cref="P:System.Windows.Documents.DocumentPaginator.PageCount"/> is the total number of pages.
    /// </summary>
    /// <returns>true if pagination is complete and <see cref="P:System.Windows.Documents.DocumentPaginator.PageCount"/> is the total number of pages; otherwise, false, if pagination is in process and <see cref="P:System.Windows.Documents.DocumentPaginator.PageCount"/> is the number of pages currently formatted (not the total).This value may revert to false, after being true, if <see cref="P:System.Windows.Documents.DocumentPaginator.PageSize"/> or content changes; because those events would force a repagination.</returns>
    public override bool IsPageCountValid => true;

    /// <summary>
    /// Returns the element being paginated.
    /// </summary>
    /// <returns>An <see cref="T:System.Windows.Documents.IDocumentPaginatorSource"/> representing the element being paginated.</returns>
    public override IDocumentPaginatorSource Source => null;

    /// <summary>
    /// Gets the <see cref="T:System.Windows.Documents.DocumentPage"/> for the specified page number.
    /// </summary>
    /// <param number="pageNumber">The zero-based page number of the document page that is needed.</param>
    /// <returns>
    /// The <see cref="T:System.Windows.Documents.DocumentPage"/> for the specified <paramref number="pageNumber"/>, or <see cref="F:System.Windows.Documents.DocumentPage.Missing"/> if the page does not exist.
    /// </returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    ///   <paramref number="pageNumber"/> is negative.</exception>
    public override DocumentPage GetPage(int pageNumber)
    {
      pageBuffer ??= [];

      Progress?.Invoke(this, ProgressEventArgs.Progress(PageCount > 0 ? ((double)pageNumber + 1) * 100 / PageCount : 100));
      if (pageBuffer.ContainsKey(pageNumber))
      {
        return pageBuffer[pageNumber];
      }
      else
      {
        int pageY = pageNumber / PagesX;
        int pageX = pageNumber - pageY * PagesX;
        canvas.Clip = new RectangleGeometry(new Rect(new Point(pageX * ActualWidth, pageY * ActualHeight), new Size(ActualWidth, ActualHeight)));
        canvas.Measure(new Size(ActualWidth, ActualHeight));
        canvas.Arrange(new Rect(new Point(-pageX * ActualWidth + OriginActualWidth, -pageY * ActualHeight + OriginActualHeight), new Size(ActualWidth, ActualHeight)));
        var dp = new DocumentPage(canvas, pageSize, new Rect(), new Rect(new Point(OriginActualWidth, OriginActualHeight), new Size(ActualWidth, ActualHeight)));
        pageBuffer.Add(pageNumber, dp);
        return dp;
      }
    }

    #region Private members

    private int PagesX
    {
      get
      {
        double x = ActualWidth;
        return Math.Abs(x) < 0.001 ? 1 : Convert.ToInt32(Math.Ceiling(canvas.Width / x));
      }
    }

    private int PagesY
    {
      get
      {
        double y = ActualHeight;
        return Math.Abs(y) < 0.001 ? 1 : Convert.ToInt32(Math.Ceiling(canvas.Height / y));
      }
    }

    private double ActualWidth
    {
      get
      {
        if (PrintableArea == null)
        {
          return pageSize.Width;
        }

        if (Orientation is PageOrientation.Landscape or PageOrientation.ReversePortrait)
        {
          if (PrintableArea.ExtentHeight > PrintableArea.ExtentWidth)
          {
            return PrintableArea.ExtentHeight;
          }
        }
        return PrintableArea.ExtentWidth;
      }
    }

    private double ActualHeight
    {
      get
      {
        if (PrintableArea == null)
        {
          return pageSize.Height;
        }

        if (Orientation is PageOrientation.Landscape or PageOrientation.ReversePortrait)
        {
          if (PrintableArea.ExtentHeight > PrintableArea.ExtentWidth)
          {
            return PrintableArea.ExtentWidth;
          }
        }
        return PrintableArea.ExtentHeight;
      }
    }

    private double OriginActualWidth
    {
      get
      {
        if (Orientation is PageOrientation.Landscape or PageOrientation.ReversePortrait)
        {
          if (PrintableArea.ExtentHeight > PrintableArea.ExtentWidth)
          {
            return PrintableArea.OriginHeight;
          }
        }
        return PrintableArea.OriginWidth;
      }
    }

    private double OriginActualHeight
    {
      get
      {
        if (Orientation is PageOrientation.Landscape or PageOrientation.ReversePortrait)
        {
          if (PrintableArea.ExtentHeight > PrintableArea.ExtentWidth)
          {
            return PrintableArea.OriginWidth;
          }
        }
        return PrintableArea.OriginHeight;
      }
    }

    #endregion
  }
}
