using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NAS.Models.Controllers;
using NUnit.Framework;

namespace NAS.UnitTests
{
  public class ReportSerializationTests
  {
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void TestReportCollectionSerialization()
    {
      var reports = new ReportCollection(
      new List<Report>
      {
        new Report
        {
           Name = "Name",
           ReportLevel = Models.Enums.ReportLevel.Integrated,
           ReportType = Models.Enums.ReportType.Controlling,
           FileName= "filename"
        }
      });

      var emptyNamespaces = new XmlSerializerNamespaces([XmlQualifiedName.Empty]);
      var serializer = new XmlSerializer(typeof(ReportCollection));
      var writerSettings = new XmlWriterSettings
      {
        Indent = true
      };
      //      writerSettings.OmitXmlDeclaration = true;

      string xmlPath = Path.Combine(Path.GetTempPath(), "NAS.Reports.xml");
      using (var writer = XmlWriter.Create(xmlPath, writerSettings))
      {
        serializer.Serialize(writer, reports, emptyNamespaces);
      }

      Assert.That(File.Exists(xmlPath), Is.True, "XML file should be created");
      var xmlContent = File.ReadAllText(xmlPath);
      Assert.That(xmlContent, Does.Contain("Name"), "XML should contain report name");

      File.Delete(xmlPath);
    }
  }
}
