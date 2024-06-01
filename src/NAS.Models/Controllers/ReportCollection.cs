using System.Collections.Generic;
using System.Xml.Serialization;
using NAS.Models.Entities;

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
  }
}
