using System.IO;
using FlowReports;
using FlowReports.Model;
using NAS.Models.Controllers;
using NAS.Models.Entities;
using NAS.Models.Enums;

namespace NAS.ViewModels.Helpers
{
  /// <summary>
  /// Manages the loading, displaying, and editing of reports using FlowReports.
  /// </summary>
  internal static class ReportManager
  {
    /// <summary>
    /// Shows a report based on its type using the provided schedule.
    /// </summary>
    internal static void ShowReport(Models.Controllers.Report report, Schedule schedule)
    {
      ArgumentNullException.ThrowIfNull(report);
      ArgumentNullException.ThrowIfNull(schedule);

      if (report.ReportType == ReportType.Activities)
      {
        ShowActivitiesReport(report, schedule.Activities);
      }
      else
      {
        throw new NotSupportedException($"Report type '{report.ReportType}' is not supported.");
      }
    }

    /// <summary>
    /// Edits a report based on its type using the provided schedule. 
    /// </summary>
    internal static void EditReport(Models.Controllers.Report report, Schedule schedule)
    {
      ArgumentNullException.ThrowIfNull(report);
      ArgumentNullException.ThrowIfNull(schedule);

      if (report.ReportLevel == ReportLevel.Integrated)
        throw new InvalidOperationException("The report is integrated and cannot be edited.");

      if (report.ReportType == ReportType.Activities)
      {
        EditActivitiesReport(report, schedule.Activities);
      }
      else
      {
        throw new NotSupportedException($"Report type '{report.ReportType}' is not supported.");
      }
    }

    /// <summary>
    /// Loads and displays a report from the specified file path using the provided activities.
    /// </summary>
    private static void ShowActivitiesReport(Models.Controllers.Report report, IEnumerable<Activity> activities)
    {
      FlowReports.Model.Report flowReport = LoadReport(report);
      FlowReport.Show(flowReport, activities);
    }

    /// <summary>
    /// Loads a report from the specified file path and starts editing.
    /// </summary>
    private static void EditActivitiesReport(Models.Controllers.Report report, IEnumerable<Activity> activities)
    {
      FlowReports.Model.Report flowReport = LoadReport(report);
      FlowReport.Edit(flowReport, activities);
    }

    private static FlowReports.Model.Report LoadReport(Models.Controllers.Report report)
    {
      string path = report.ReportLevel == ReportLevel.Integrated
        ? Models.Globals.IntegratedReportsPath
        : ReportsController.GetUserReportsPath();

      string filePath = Path.Combine(path, report.FileName);

      if (!File.Exists(filePath))
        throw new FileNotFoundException("The specified report file was not found.", filePath);

      return FlowReport.Load(filePath);
    }
  }
}
