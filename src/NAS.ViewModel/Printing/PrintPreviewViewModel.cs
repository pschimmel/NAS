using System.IO;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Xps.Packaging;
using ES.Tools.Core.MVVM;
using NAS.Model.Base;
using NAS.Model.Entities;
using NAS.Resources;
using NAS.ViewModel.Base;
using NAS.ViewModel.Helpers;

namespace NAS.ViewModel.Printing
{
  public class PrintPreviewViewModel : ViewModelBase
  {
    #region Private Members

    private bool isBusy;
    private string progressText;
    private readonly ScheduleViewModel viewModel;
    private NASDocumentPaginator paginator;
    private IDocumentPaginatorSource document;
    private readonly List<string> tempFileNames = [];

    #endregion

    #region Constructors

    public PrintPreviewViewModel(ScheduleViewModel viewModel, IPrintableCanvas canvas)
    {
      this.viewModel = viewModel;
      Canvas = canvas;
      try
      {
        var localPrintServer = new LocalPrintServer();
        Printer = LocalPrintServer.GetDefaultPrintQueue();
        var ticket = Printer.DefaultPrintTicket;
        PageSize = ticket.PageMediaSize;
        PrintableArea = Printer.GetPrintCapabilities(ticket).PageImageableArea;
        Orientation = ticket.PageOrientation ?? PageOrientation.Landscape;
      }
      catch
      {
        UserNotificationService.Instance.Error(NASResources.MessageInitializingPrinterQueueError);
      }
      Zoom = 1;
      UpdatePreview();
      PageSettingsCommand = new ActionCommand(PageSettingsCommandExecute, () => PageSettingsCommandCanExecute);
      PrintCommand = new ActionCommand(PrintCommandExecute, () => PrintCommandCanExecute);
      LayoutSettingsCommand = new ActionCommand(LayoutSettingsCommandExecute, () => LayoutSettingsCommandCanExecute);
    }

    #endregion

    #region Public Members

    public Schedule Project => viewModel.Schedule;
    public IPrintableCanvas Canvas { get; private set; }
    public PrintQueue Printer { get; private set; }
    public PageMediaSize PageSize { get; private set; }
    public PageImageableArea PrintableArea { get; private set; }
    public PageOrientation Orientation { get; private set; }
    public double Zoom { get; private set; }

    public override HelpTopic HelpTopicKey => HelpTopic.Printing;

    public bool IsBusy
    {
      get => isBusy;
      private set
      {
        if (value != isBusy)
        {
          isBusy = value;
          OnPropertyChanged(nameof(IsBusy));
        }
      }
    }

    public string ProgressText
    {
      get => progressText;
      private set
      {
        if (value != progressText)
        {
          progressText = value;
          OnPropertyChanged(nameof(ProgressText));
        }
      }
    }

    public IDocumentPaginatorSource Document
    {
      get => document;
      private set
      {
        document = value;
        OnPropertyChanged(nameof(Document));
      }
    }

    #endregion

    #region Protected Members

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);
      if (disposing)
      {
        foreach (string fileName in tempFileNames)
        {
          try
          {
            File.Delete(fileName);
          }
          catch (Exception ex)
          {
            UserNotificationService.Instance.Error(ex.Message);
          }
        }
      }
    }

    #endregion

    #region Page Settings

    public ICommand PageSettingsCommand { get; }

    private void PageSettingsCommandExecute()
    {
      var vm = new PageSettingsViewModel(Printer, PageSize, Orientation, Zoom);
      if (ViewFactory.Instance.ShowDialog(vm) == true)
      {
        Printer = vm.SelectedPrinter;
        PageSize = vm.SelectedPageSize;
        Zoom = Convert.ToDouble(vm.Zoom) / 100;
        Orientation = vm.Orientation;
        var ticket = Printer.UserPrintTicket;
        ticket.PageOrientation = Orientation;
        ticket.PageMediaSize = PageSize;
        PrintableArea = Printer.GetPrintCapabilities(ticket).PageImageableArea;

        var pageSize = Orientation is PageOrientation.Landscape or PageOrientation.ReverseLandscape
          ? new Size(PageSize.Height.Value, PageSize.Width.Value)
          : new Size(PageSize.Width.Value, PageSize.Height.Value);

        paginator.PageSize = pageSize;
        paginator.PrintableArea = PrintableArea;
        paginator.Orientation = Orientation;
        UpdatePreview();
      }
    }

    private bool PageSettingsCommandCanExecute => true;

    #endregion

    #region Print

    public ICommand PrintCommand { get; }

    private void PrintCommandExecute()
    {
      var doc = Document;
      var printDialog = new PrintDialog
      {
        PageRangeSelection = PageRangeSelection.AllPages,
        PrintQueue = Printer
      };
      printDialog.PrintTicket.PageMediaSize = PageSize;
      printDialog.PrintTicket.PageOrientation = Orientation;
      printDialog.UserPageRangeEnabled = true;
      bool? result = printDialog.ShowDialog();
      // set the print ticket for the document sequence and write it to the printer.
      (doc as FixedDocumentSequence).PrintTicket = printDialog.PrintTicket;
      if (result == true)
      {
        var writer = PrintQueue.CreateXpsDocumentWriter(printDialog.PrintQueue);
        writer.WriteAsync(doc as FixedDocumentSequence, printDialog.PrintTicket);
      }
    }

    private bool PrintCommandCanExecute => true;

    #endregion

    #region Layout Settings

    public ICommand LayoutSettingsCommand { get; }

    private void LayoutSettingsCommandExecute()
    {
      using var vm = new PrintLayoutViewModel(viewModel.Schedule.CurrentLayout);
      if (ViewFactory.Instance.ShowDialog(vm) == true)
      {
        UpdatePreview();
      }
    }

    private bool LayoutSettingsCommandCanExecute => viewModel.Schedule.CurrentLayout != null;

    #endregion

    #region Private Members

    private void UpdatePreview()
    {
      var addHeaderEvent = EventAggregator.Instance.GetEvent<GetPrintCanvasEvent>();
      addHeaderEvent.Publish(new PrintCanvasData(PageSize, PrintableArea, Orientation));

      var canvas = PrepareCanvas(PageSize, PrintableArea, Orientation);
      paginator = new NASDocumentPaginator(canvas, PageSize, PrintableArea, Orientation);
      paginator.Progress += Paginator_Progress;
      IsBusy = true;
      try
      {
        // BUGFIX: PS 16.08.13 Not working for some reasons
        //Uri uri = new Uri(@"pack://" + Path.GetTempFileName(), UriKind.Absolute);
        //var memoryStream = new MemoryStream();
        //var package = Package.Open(memoryStream, FileMode.Create, FileAccess.ReadWrite);
        //PackageStore.AddPackage(uri, package);
        //var xpsDocument = new XpsDocument(package, CompressionOption.NotCompressed, uri.AbsoluteUri);
        //var rsm = new XpsSerializationManager(new XpsPackagingPolicy(xpsDocument), false);
        //paginator.Progress += paginator_Progress;
        //rsm.SaveAsXaml(paginator);
        //preview.Document = xpsDocument.GetFixedDocumentSequence();
        //xpsDocument.Close();
        // WORKAROUND:

        string tempFileName = Path.GetTempFileName();
        tempFileNames.Add(tempFileName);

        using var xpsDocument = new XpsDocument(tempFileName, FileAccess.ReadWrite);
        var writer = XpsDocument.CreateXpsDocumentWriter(xpsDocument);
        writer.Write(paginator);
        Document = xpsDocument.GetFixedDocumentSequence();
        // END WORKAROUND
      }
      catch (Exception ex)
      {
        UserNotificationService.Instance.Error(ex.Message);
      }
      finally
      {
        paginator.Progress -= Paginator_Progress;
        IsBusy = false;
      }
    }

    private delegate void UpdateProgressBarDelegate(DependencyProperty dp, object value);

    private void Paginator_Progress(object sender, ProgressEventArgs e)
    {
      ProgressText = string.Format(NASResources.CreatingPagesProgress, Math.Round(e.CurrentProgress, 0));
    }

    private void Preview_PageSizeChanged(object sender, EventArgs e)
    {
      var pageSize=Orientation is PageOrientation.Landscape or PageOrientation.ReverseLandscape
        ?new Size(PageSize.Height.Value, PageSize.Width.Value)
        :new Size(PageSize.Width.Value, PageSize.Height.Value);
      paginator.PageSize = pageSize;
      paginator.PrintableArea = PrintableArea;
      paginator.Orientation = Orientation;
      UpdatePreview();
    }

    private Canvas PrepareCanvas(PageMediaSize pageSize, PageImageableArea printableArea, PageOrientation orientation)
    {
      var size = new Size(GetActualWidth(pageSize, printableArea, orientation), GetActualHeight(pageSize, printableArea, orientation));
      var layout = Project.CurrentLayout;
      double marginLeft = layout.MarginLeft * 96 / 2.54;
      double marginRight = layout.MarginRight * 96 / 2.54;
      double marginTop = layout.MarginTop * 96 / 2.54;
      double marginBottom = layout.MarginBottom * 96 / 2.54;
      double headerHeight = layout.HeaderHeight * 96 / 2.54;
      double footerHeight = layout.FooterHeight * 96 / 2.54;
      var newCanvas = AddFrame((Canvas)Canvas, size, marginLeft, marginTop, marginRight, marginBottom, headerHeight, footerHeight);
      AddHeader(newCanvas, layout, newCanvas.Width - marginLeft - marginRight, headerHeight, marginLeft, marginTop);
      AddFooter(newCanvas, layout, newCanvas.Width - marginLeft - marginRight, footerHeight, marginLeft, newCanvas.Height - marginBottom - footerHeight);
      return newCanvas;
    }

    private double GetActualWidth(PageMediaSize pageSize, PageImageableArea printableArea, PageOrientation orientation)
    {
      if (printableArea == null)
      {
        return orientation is PageOrientation.Landscape or PageOrientation.ReverseLandscape
          ? pageSize.Height.Value
          : pageSize.Width.Value;
      }
      if (orientation is PageOrientation.Landscape or PageOrientation.ReversePortrait)
      {
        if (printableArea.ExtentHeight > printableArea.ExtentWidth)
        {
          return printableArea.ExtentHeight;
        }
      }
      return printableArea.ExtentWidth;
    }

    private double GetActualHeight(PageMediaSize pageSize, PageImageableArea printableArea, PageOrientation orientation)
    {
      if (printableArea == null)
      {
        return orientation is PageOrientation.Landscape or PageOrientation.ReverseLandscape
          ? pageSize.Width.Value
          : pageSize.Height.Value;
      }
      if (orientation is PageOrientation.Landscape or PageOrientation.ReversePortrait)
      {
        if (printableArea.ExtentHeight > printableArea.ExtentWidth)
        {
          return printableArea.ExtentWidth;
        }
      }
      return printableArea.ExtentHeight;
    }

    private Canvas AddFrame(Canvas originalCanvas, Size pageSize, double leftMargin, double topMargin, double rightMargin, double bottomMargin, double headerHeight, double footerHeight)
    {
      double width = originalCanvas.Width + leftMargin + rightMargin;
      double height = originalCanvas.Height + topMargin + headerHeight + bottomMargin + footerHeight;
      width = Math.Ceiling(width / pageSize.Width) * pageSize.Width;
      height = Math.Ceiling(height / pageSize.Height) * pageSize.Height;
      var c = new Canvas { Width = width, Height = height };
      var clone = originalCanvas;
      c.Children.Add(clone);
      System.Windows.Controls.Canvas.SetLeft(clone, leftMargin);
      System.Windows.Controls.Canvas.SetTop(clone, topMargin + headerHeight);
      return c;
    }

    private void AddHeader(Canvas c, Layout layout, double width, double height, double left, double top)
    {
      var header = new Grid { Height = height, Width = width };
      c.Children.Add(header);
      System.Windows.Controls.Canvas.SetTop(header, top);
      System.Windows.Controls.Canvas.SetLeft(header, left);
      foreach (var definition in layout.HeaderItems.OrderBy(x => x.Column))
      {
        AddHeaderOrFooterColumn(header, definition);
      }
    }

    private void AddFooter(Canvas c, Layout layout, double width, double height, double left, double top)
    {
      var footer = new Grid { Height = height, Width = width };
      c.Children.Add(footer);
      System.Windows.Controls.Canvas.SetTop(footer, top);
      System.Windows.Controls.Canvas.SetLeft(footer, left);
      foreach (var definition in layout.FooterItems.OrderBy(x => x.Column))
      {
        AddHeaderOrFooterColumn(footer, definition);
      }
    }

    private void AddHeaderOrFooterColumn(Grid grid, IPrintLayoutItem def)
    {
      grid.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition());
      //TODO: MOVE TO VIEW
      //var richTextBox = new Xceed.Wpf.Toolkit.RichTextBox() { BorderThickness = new Thickness(0) };
      //richTextBox.TextFormatter = new RtfFormatter();
      //grid.Children.Add(richTextBox);
      string text = def.Definition;
      if (text != null)
      {
        while (text.Contains("##ProjectName##"))
        {
          text = text.Replace("##ProjectName##", Project.Name);
        }

        while (text.Contains("##DataDate##"))
        {
          text = text.Replace("##DataDate##", Project.DataDate.ToString("yyyy-MM-dd"));
        }

        while (text.Contains("##Date##"))
        {
          text = text.Replace("##Date##", DateTime.Today.ToString("yyyy-MM-dd"));
        }
      }
      //richTextBox.Text = text;
      //richTextBox.IsReadOnly = true;
      //Grid.SetColumn(richTextBox, def.Column);
    }

    #endregion
  }
}
