using System.Collections.Generic;
using System.IO;
using FlowReports;
using NAS.Models;
using NAS.Models.Controllers;
using NUnit.Framework;

namespace NAS.UnitTests
{
  public class FlowReportReaderTests
  {
    private string _integratedReportsPath;
    private ReportCollection _reports;

    [SetUp]
    public void Setup()
    {
      _integratedReportsPath = Globals.IntegratedReportsPath;
      _reports = ReportsController.LoadReports().GetCollection(Models.Enums.ReportLevel.Integrated);
    }

    [Test]
    public void TestIntegratedReportsPathExists()
    {
      Assert.That(Directory.Exists(_integratedReportsPath), Is.True,
        $"Integrated reports path '{_integratedReportsPath}' should exist");
    }

    [Test]
    public void TestReportDefinitionsFileExists()
    {
      string reportDefinitionsPath = Path.Combine(_integratedReportsPath, Globals.ReportDefinitionFileName);
      Assert.That(File.Exists(reportDefinitionsPath), Is.True,
        $"Report definitions file '{reportDefinitionsPath}' should exist");
    }

    [Test]
    public void TestReportDefinitionsCanBeLoaded()
    {
      Assert.That(_reports, Is.Not.Null, "Report definitions should be loaded successfully");
      Assert.That(_reports.Count, Is.GreaterThan(0), "At least one integrated report should be defined");
    }

    [Test]
    [TestCaseSource(nameof(GetIntegratedReports))]
    public void TestFlowReportFileCanBeRead(Report report)
    {
      string filePath = Path.Combine(_integratedReportsPath, report.FileName);

      Assert.That(File.Exists(filePath), Is.True,
        $"FlowReport file '{filePath}' for report '{report.Name}' should exist");

      Assert.DoesNotThrow(() => FlowReport.Load(filePath),
        $"FlowReport file '{filePath}' for report '{report.Name}' should be readable and valid");
    }

    [Test]
    [TestCaseSource(nameof(GetIntegratedReports))]
    public void TestFlowReportHasValidContent(Report report)
    {
      string filePath = Path.Combine(_integratedReportsPath, report.FileName);
      var flowReport = FlowReport.Load(filePath);

      Assert.That(flowReport, Is.Not.Null,
        $"FlowReport '{report.Name}' should load successfully");
    }

    public static IEnumerable<Report> GetIntegratedReports()
    {
      var reports = ReportsController.LoadReports().GetCollection(Models.Enums.ReportLevel.Integrated);
      return reports ?? new List<Report>();
    }
  }
}
