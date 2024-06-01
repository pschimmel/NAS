using NAS.Models.Base;
using NAS.Models.Entities;

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
