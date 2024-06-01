using System.Printing;
using NAS.Models.Base;

namespace NAS.ViewModels
{
  public class GetPrintCanvasEvent : GenericAggregatedEvent<PrintCanvasData>
  {
  }

  public class PrintCanvasData
  {
    public PrintCanvasData(PageMediaSize pageSize, PageImageableArea printableArea, PageOrientation orientation)
    {
      PageSize = pageSize;
      PrintableArea = printableArea;
      Orientation = orientation;
    }

    public PageMediaSize PageSize { get; }

    public PageImageableArea PrintableArea { get; }

    public PageOrientation Orientation { get; }

    public object Image { get; set; }
  }
}
