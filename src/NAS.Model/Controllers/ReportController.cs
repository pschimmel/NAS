using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using ES.Tools.Core.Infrastructure;
using NAS.Model.Entities;
using NAS.Model.Enums;

namespace NAS.Model.Controllers
{
  public class ReportController
  {
    public event EventHandler ReportsChanged;

    private readonly string _userReportsPath;
    private List<Report> _reports;

    public ReportController(string userReportsPath)
    {
      _userReportsPath = userReportsPath;
      LoadReports();
    }

    /// <summary>
    /// Gets a list of all reports
    /// </summary>
    public IReadOnlyCollection<Report> Reports
    {
      get
      {
        _reports ??= LoadReports();

        return _reports.AsReadOnly();
      }
    }

    public List<Report> LoadReports()
    {
      var reports = new List<Report>();
      reports.AddRange(LoadReports(ReportLevel.Integrated));
      reports.AddRange(LoadReports(ReportLevel.User));
      return reports;
    }

    public void DeleteReport(Report report)
    {
      _reports.Remove(report);
      ReportsChanged?.Invoke(this, EventArgs.Empty);
    }

    public void CopyReport(Report report, string name)
    {
      _reports.Add(new Report(report) { Name = name });
      ReportsChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Saves all user defined reports
    /// </summary>
    public bool SaveReports()
    {
      var reports = Reports.Where(x => x.ReportLevel == ReportLevel.User)
                           .OrderBy(x => x.Name);
      var collection = new ReportCollection(reports);

      try
      {
        var emptyNamespaces = new XmlSerializerNamespaces([XmlQualifiedName.Empty]);
        var serializer = new XmlSerializer(typeof(ReportCollection));
        var writerSettings = new XmlWriterSettings();
        writerSettings.Indent = true;
        //writerSettings.OmitXmlDeclaration = true;

        using var writer = XmlWriter.Create(_userReportsPath, writerSettings);
        serializer.Serialize(writer, reports, emptyNamespaces);
      }
      catch (Exception ex)
      {
        Debug.Fail(ex.Message);
        return false;
      }

      return true;
    }

    private ReportCollection LoadReports(ReportLevel level)
    {
      string fileName = "";

      switch (level)
      {
        case ReportLevel.Integrated:
          fileName = Path.Combine(ApplicationHelper.StartupPath, Globals.ReportsFileName);
          break;
        case ReportLevel.User:
          fileName = _userReportsPath;
          break;
      }

      if (File.Exists(fileName))
      {
        try
        {
          var serializer = new XmlSerializer(typeof(ReportCollection));
          using var reader = XmlReader.Create(Globals.SettingsFileName);
          var settings = (ReportCollection)serializer.Deserialize(reader);

          if (settings != null)
          {
            return settings;
          }
        }
        catch (Exception ex)
        {
          Debug.Fail(ex.Message);
        }
      }

      return [];
    }
  }
}
