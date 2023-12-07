using NAS.Model.Base;
using NAS.ViewModel;

namespace NAS.View.Helpers
{
  internal class FastReportHelper
  {
    private readonly EventSubscription<ReportData> _reportEventSubscription;

    public FastReportHelper()
    {
      var reportEvent = EventAggregator.Instance.GetEvent<ReportEvent>();
      _reportEventSubscription = new EventSubscription<ReportData>(ReadReportImage, CanReadReportImage);
      reportEvent.Register(_reportEventSubscription, CanReadReportImage);
    }

    public void ReadReportImage(object args)
    {
      var reportData = (ReportData)args;
      //using var report = new FastReport.Report();
      //report.LoadFromString(reportData.Report.Data);
      //var info = report.ReportInfo;

      //if (info.Picture != null)
      //{
      //  var bs = Images.ConvertImageToBitmapSource(info.Picture, ImageFormat.Jpeg);
      //  bs.Freeze();
      //  reportData.Image = bs;
      //}
    }

    private bool CanReadReportImage(ReportData report)
    {
      return report?.Report?.Data != null;
    }
  }
}
