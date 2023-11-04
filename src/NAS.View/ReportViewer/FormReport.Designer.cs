namespace NAS.ReportViewer
{
  partial class FormReport
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param number="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        fastReport.Dispose();

        if (components != null)
        {
          components.Dispose();
        }
        base.Dispose(disposing);
      }
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      FastReport.Design.DesignerSettings designerSettings1 = new FastReport.Design.DesignerSettings();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormReport));
      FastReport.Design.DesignerRestrictions designerRestrictions1 = new FastReport.Design.DesignerRestrictions();
      FastReport.Export.Email.EmailSettings emailSettings1 = new FastReport.Export.Email.EmailSettings();
      FastReport.PreviewSettings previewSettings1 = new FastReport.PreviewSettings();
      FastReport.ReportSettings reportSettings1 = new FastReport.ReportSettings();
      this.previewControl = new FastReport.Preview.PreviewControl();
      this.environmentSettings = new FastReport.EnvironmentSettings();
      this.SuspendLayout();
      //
      // previewControl
      //
      this.previewControl.BackColor = System.Drawing.SystemColors.AppWorkspace;
      this.previewControl.Buttons = ((FastReport.PreviewButtons)((((((((((FastReport.PreviewButtons.Print | FastReport.PreviewButtons.Open)
                  | FastReport.PreviewButtons.Save)
                  | FastReport.PreviewButtons.Find)
                  | FastReport.PreviewButtons.Zoom)
                  | FastReport.PreviewButtons.Outline)
                  | FastReport.PreviewButtons.PageSetup)
                  | FastReport.PreviewButtons.Edit)
                  | FastReport.PreviewButtons.Watermark)
                  | FastReport.PreviewButtons.Navigator)));
      this.previewControl.Dock = System.Windows.Forms.DockStyle.Fill;
      this.previewControl.Font = new System.Drawing.Font("Tahoma", 8F);
      this.previewControl.Location = new System.Drawing.Point(0, 0);
      this.previewControl.Name = "previewControl";
      this.previewControl.Size = new System.Drawing.Size(742, 377);
      this.previewControl.TabIndex = 0;
      this.previewControl.UIStyle = FastReport.Utils.UIStyle.Office2007Silver;
      //
      // environmentSettings
      //
      designerSettings1.ApplicationConnection = null;
      designerSettings1.DefaultFont = new System.Drawing.Font("Arial", 10F);
      designerSettings1.Icon = ((System.Drawing.Icon)(resources.GetObject("designerSettings1.Icon")));
      designerSettings1.Restrictions = designerRestrictions1;
      designerSettings1.ShowInTaskbar = true;
      designerSettings1.Text = "";
      this.environmentSettings.DesignerSettings = designerSettings1;
      emailSettings1.Address = "";
      emailSettings1.Host = "";
      emailSettings1.MessageTemplate = "";
      emailSettings1.Name = "";
      emailSettings1.Password = "";
      emailSettings1.UserName = "";
      this.environmentSettings.EmailSettings = emailSettings1;
      previewSettings1.Icon = ((System.Drawing.Icon)(resources.GetObject("previewSettings1.Icon")));
      previewSettings1.Text = "";
      this.environmentSettings.PreviewSettings = previewSettings1;
      this.environmentSettings.ReportSettings = reportSettings1;
      this.environmentSettings.UIStyle = FastReport.Utils.UIStyle.VistaGlass;
      //
      // FormReport
      //
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(742, 377);
      this.Controls.Add(this.previewControl);
      this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.Name = "FormReport";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.Text = "FormReport";
      this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
      this.ResumeLayout(false);

    }

    #endregion

    private FastReport.Preview.PreviewControl previewControl;
    private FastReport.EnvironmentSettings environmentSettings;
  }
}
