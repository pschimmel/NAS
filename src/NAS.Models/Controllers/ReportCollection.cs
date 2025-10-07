using System.Xml.Serialization;
using NAS.Models.Enums;

namespace NAS.Models.Controllers
{
  [XmlRoot("Reports")]
  public class ReportCollection : List<Report>
  {
    public ReportCollection()
    { }

    public ReportCollection(IEnumerable<Report> reports)
      : base(reports)
    { }

    public ReportCollection GetCollection(ReportLevel level)
    {
      return GetCollection(r => r.ReportLevel == level);
    }

    public ReportCollection GetCollection(Func<Report, bool> predicate)
    {
      return new ReportCollection(this.Where(predicate).OrderBy(x => x.Name));
    }
  }
}
