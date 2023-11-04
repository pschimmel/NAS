using System.Collections.Generic;
using System.Xml.Serialization;
using NAS.Model.Entities;

namespace NAS.Model.Controllers
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
