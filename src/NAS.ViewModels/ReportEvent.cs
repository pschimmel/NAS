using NAS.Models.Base;
using NAS.Models.Controllers;

namespace NAS.ViewModels
{
  public class ReportEvent : GenericAggregatedEvent<ReportData>
  {
  }

  public class ReportData
  {
    public ReportData(Report report)
    {
      Report = report;
    }

    public Report Report { get; }

    public object Image { get; set; }
  }
}
