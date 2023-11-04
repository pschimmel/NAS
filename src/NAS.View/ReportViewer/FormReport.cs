using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using ES.WPF;
using NAS.Globalization;
using NAS.Model.Entities;

namespace NAS.ReportViewer
{
  public partial class FormReport : Form
  {
    private readonly FastReport.Report fastReport;
    private readonly Schedule schedule;
    private readonly bool edit;

    public FormReport(Report report, Schedule p, bool edit)
    {
      InitializeComponent();
      Visible = false;
      fastReport = FastReport.Report.FromFile(report.FileName);
      Text = report.Name;
      schedule = p;
      this.edit = edit;
      fastReport.Preview = previewControl;
    }

    protected override void OnShown(EventArgs e)
    {
      var list = new List<Schedule>
      {
        schedule
      };
      fastReport.RegisterData(list, "Project");
      string localizationFileName = ApplicationHelper.StartupPath + "\\Localization\\" + System.Threading.Thread.CurrentThread.CurrentUICulture.EnglishName + ".frl";
      if (!File.Exists(localizationFileName) && System.Threading.Thread.CurrentThread.CurrentUICulture.Parent != null)
      {
        localizationFileName = ApplicationHelper.StartupPath + "\\Localization\\" + System.Threading.Thread.CurrentThread.CurrentUICulture.Parent.EnglishName + ".frl";
      }

      if (File.Exists(localizationFileName))
      {
        FastReport.Utils.Res.LoadLocale(localizationFileName);
      }

      if (edit)
      {
        fastReport.Design();
        Close();
      }
      else
      {
        Visible = true;
        if (fastReport.Prepare())
        {
          fastReport.ShowPrepared();
        }
        else
        {
          ES.WPF.Toolkit.MessageBox.Show(NASResources.MessageErrorPreparingReport, NASResources.Error, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
      }
    }
  }
}
