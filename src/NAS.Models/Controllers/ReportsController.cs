using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NAS.Models.Enums;
using NAS.Models.Settings;

namespace NAS.Models.Controllers
{
  public static class ReportsController
  {
    /// <summary>
    /// Loads all reports from integrated and user-defined locations.
    /// </summary>
    public static ReportCollection LoadReports()
    {
      var reports = new ReportCollection();
      reports.AddRange(LoadReports(ReportLevel.Integrated));
      reports.AddRange(LoadReports(ReportLevel.User));
      return reports;
    }

    /// <summary>
    /// Loads reports from a specified level (Integrated or User).
    /// </summary>
    private static ReportCollection LoadReports(ReportLevel level)
    {
      string path = "";

      switch (level)
      {
        case ReportLevel.Integrated:
          path = Globals.IntegratedReportsPath;
          break;
        case ReportLevel.User:
          path = GetUserReportsPath();
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof(level), level, null);
      }

      string filePath = Path.Combine(path, Globals.ReportDefinitionFileName);

      if (File.Exists(filePath))
      {
        return LoadReportDefinitions(filePath);
      }

      return new ReportCollection();
    }

    private static ReportCollection LoadReportDefinitions(string fileName)
    {
      try
      {
        var serializer = new XmlSerializer(typeof(ReportCollection));
        using var reader = XmlReader.Create(fileName);
        var reports = (ReportCollection)serializer.Deserialize(reader);
        return reports;
      }
      catch (Exception ex)
      {
        Debug.Fail(ex.Message);
      }

      return null;
    }

    /// <summary>
    /// Saves all user defined reports
    /// </summary>
    public static bool SaveReports(ReportCollection reports)
    {
      var collection = reports.GetCollection(ReportLevel.User);

      try
      {
        var emptyNamespaces = new XmlSerializerNamespaces([XmlQualifiedName.Empty]);
        var serializer = new XmlSerializer(typeof(ReportCollection));
        var writerSettings = new XmlWriterSettings
        {
          Indent = true
        };
        //writerSettings.OmitXmlDeclaration = true;
        string userReportsPath = Path.Combine(GetUserReportsPath(), Globals.ReportDefinitionFileName);
        using var writer = XmlWriter.Create(userReportsPath, writerSettings);
        serializer.Serialize(writer, collection, emptyNamespaces);
      }
      catch (Exception ex)
      {
        Debug.Fail(ex.Message);
        return false;
      }

      return true;
    }

    /// <summary>
    /// Deletes a user-defined report.
    /// </summary>
    public static void DeleteReport(Report report)
    {
      if (report.ReportLevel == ReportLevel.Integrated)
        throw new InvalidOperationException("The report is integrated and cannot be deleted.");

      string reportFilePath = Path.Combine(GetUserReportsPath(), report.FileName);
      if (File.Exists(reportFilePath))
      {
        try
        {
          File.Delete(reportFilePath);
          ReportCollection reports = LoadReports(ReportLevel.User).GetCollection(x => x.Name != report.Name);
          SaveReports(reports);
        }
        catch (Exception ex)
        {
          Debug.Fail(ex.Message);
        }
      }
    }

    /// <summary>
    /// Copies an existing report to create a new user-defined report with a different name.
    /// </summary>
    public static Report CopyReport(Report report, string name)
    {
      var newReport = new Report(report) { Name = name, ReportLevel = ReportLevel.User };
      var oldPath = report.ReportLevel == ReportLevel.User ? GetUserReportsPath() : Globals.IntegratedReportsPath;
      var oldFilePath = Path.Combine(oldPath, report.FileName);
      
      if (File.Exists(oldFilePath))
      {
        var reports = LoadReports(ReportLevel.User);
        if (reports.Any(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
          throw new InvalidOperationException("A report with the same name already exists.");

        var newFileName = newReport.FileName;
        var newFilePath = Path.Combine(GetUserReportsPath(), newFileName);
        int idx = 1;

        while (File.Exists(newFilePath))
        {
          newFileName = $"{Path.GetFileNameWithoutExtension(newReport.FileName)} {idx++}{Path.GetExtension(newReport.FileName)}";
          newFilePath = Path.Combine(GetUserReportsPath(), newFileName);
          newReport.FileName = newFileName;
        }

        if (!Directory.Exists(Path.GetDirectoryName(newFilePath)))
        {
          Directory.CreateDirectory(Path.GetDirectoryName(newFilePath));
        }

        File.Copy(oldFilePath, newFilePath);

        reports.Add(newReport);
        SaveReports(reports);

        return newReport;
      }

      throw new FileNotFoundException("The source report file was not found.", oldFilePath);
    }

    public static string GetUserReportsPath()
    {
      string path = SettingsController.Settings.UserReportsFolderPath;
      if (!Directory.Exists(path))
      {
        try
        {
          Directory.CreateDirectory(path);
        }
        catch (Exception ex)
        {
          Debug.Fail(ex.Message);
        }
      }
      return path;
    }
  }
}
