﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows;
using ES.Tools.Core.Infrastructure;
using NAS.Model.Entities;
using NAS.Resources;
using NAS.ViewModel.Helpers;

namespace NAS.Helpers
{
  public static class FastReportHelper
  {
    public static bool ShowFastReport(Schedule schedule, string fileName, CultureInfo language)
    {
      if (schedule == null)
      {
        throw new ArgumentNullException(nameof(schedule));
      }

      if (fileName == null)
      {
        throw new ArgumentNullException(nameof(fileName));
      }

      if (!File.Exists(fileName))
      {
        UserNotificationService.Instance.Error(string.Format(NASResources.MessageFileNotFound, fileName));
        return false;
      }

      SetUILanguage(language);

      using var fastReport = new FastReport.Report();
      fastReport.Load(fileName);

      if (RegisterData(schedule, fastReport) && fastReport.Prepare())
      {
        using (var reportSettings = new FastReport.EnvironmentSettings())
        {
          reportSettings.PreviewSettings.Buttons = FastReport.PreviewButtons.Print |
            FastReport.PreviewButtons.Save |
            FastReport.PreviewButtons.Find |
            FastReport.PreviewButtons.Zoom |
            FastReport.PreviewButtons.Outline |
            FastReport.PreviewButtons.PageSetup |
            FastReport.PreviewButtons.Watermark |
            FastReport.PreviewButtons.Navigator;
          reportSettings.UIStyle = FastReport.Utils.UIStyle.Office2007Silver;
          using var iconStream = Application.GetResourceStream(new Uri("pack://application:,,,/NAS;component/Images/Stones.ico")).Stream;
          reportSettings.PreviewSettings.Icon = new System.Drawing.Icon(iconStream);
        }
        fastReport.ShowPrepared(true);
        return true;
      }

      return false;
    }

    public static bool EditFastReport(Schedule schedule, string fileName, CultureInfo language)
    {
      if (schedule == null)
      {
        throw new ArgumentNullException(nameof(schedule));
      }

      if (fileName == null)
      {
        throw new ArgumentNullException(nameof(fileName));
      }

      if (!File.Exists(fileName))
      {
        UserNotificationService.Instance.Error(string.Format(NASResources.MessageFileNotFound, fileName));
        return false;
      }

      SetUILanguage(language);

      var fastReport = FastReport.Report.FromFile(fileName);
      string oldData = fastReport.SaveToString();

      if (!RegisterData(schedule, fastReport))
      {
        UserNotificationService.Instance.Error(NASResources.MessageErrorPreparingReport);
        return false;
      }

      fastReport.Design(true);

      if (!File.Exists(fileName))
      {
        UserNotificationService.Instance.Error(string.Format(NASResources.MessageFileNotFound, fileName));
        return false;
      }

      string newData = File.ReadAllText(fastReport.FileName);

      if (!oldData.Trim().Equals(newData.Trim().StripBOM()))
      {
        UserNotificationService.Instance.Question(NASResources.MessageSaveChangedReport, () =>
        {
          try
          {
            fastReport.Save(fileName);
          }
          catch (Exception ex)
          {
            UserNotificationService.Instance.Error(ex.Message);
          }
        });
      }

      return true;
    }

    private static void SetUILanguage(CultureInfo language)
    {
      string localizationFileName = ApplicationHelper.StartupPath + "\\Localization\\" + language.EnglishName + ".frl";
      if (!File.Exists(localizationFileName) && language.Parent != null)
      {
        localizationFileName = ApplicationHelper.StartupPath + "\\Localization\\" + language.Parent.EnglishName + ".frl";
      }

      if (File.Exists(localizationFileName))
      {
        FastReport.Utils.Res.LoadLocale(localizationFileName);
      }
    }

    private static bool RegisterData(Schedule schedule, FastReport.Report fastReport)
    {
      var list = new List<Schedule>
      {
        schedule
      };
      fastReport.RegisterData(list, "Schedule");
      return true;
    }
  }
}