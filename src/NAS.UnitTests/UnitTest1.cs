using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using NAS.Model.Controllers;
using NAS.Model.Entities;
using NUnit.Framework;

namespace NAS.UnitTests
{
  public class Tests
  {
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
      var reports = new ReportCollection(
      new List<Report>
      {
        new Report
        {
           Name = "Name",
           IsReadOnly = false,
           ReportLevel = Model.Enums.ReportLevel.Integrated,
           ReportType = Model.Enums.ReportType.Controlling,
           FileName= "filename"
        }
      });

      var emptyNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
      var serializer = new XmlSerializer(typeof(ReportCollection));
      var writerSettings = new XmlWriterSettings();
      writerSettings.Indent = true;
//      writerSettings.OmitXmlDeclaration = true;

      using var writer = XmlWriter.Create("C:\\Temp\\NAS.Reports.xml", writerSettings);
      serializer.Serialize(writer, reports, emptyNamespaces);

    }
  }
}