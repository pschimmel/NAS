using NAS.Model.Base;
using NAS.Model.Entities;

namespace NAS.ViewModel
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
