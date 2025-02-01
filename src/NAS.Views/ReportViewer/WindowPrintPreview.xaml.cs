using ES.Tools.Core.MVVM;

namespace NAS.ReportViewer
{
  /// <summary>
  /// Interaction logic for WindowPrintPreview.xaml
  /// </summary>
  public partial class WindowPrintPreview : IView
  {
    public WindowPrintPreview()
    {
      InitializeComponent();
    }

    public IViewModel ViewModel
    {
      get => DataContext as IViewModel;
      set => DataContext = value;
    }

    //protected override void OnActivated(EventArgs e)
    //{
    //  base.OnActivated(e);
    //  if (startup) {
    //    if (Application.Current != null && Application.Current.Dispatcher != null)
    //      Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new System.Threading.ThreadStart(delegate { }));
    //    updatePreview();
    //    startup = false;
    //  }
    //}

    //private void updatePreview()
    //{
    //  GC.Collect();
    //  var c = addFrame(canvas);
    //  c.UpdateLayout();
    //  paginator = new NASDocumentPaginator(c, preview.PageSize, preview.PrintableArea, preview.Orientation);
    //  var updatePbDelegate = new UpdateProgressBarDelegate(bi.SetValue);
    //  try {
    //    Dispatcher.Invoke(updatePbDelegate, DispatcherPriority.Background, new object[] { BusyIndicator.IsBusyProperty, true });
    //  }
    //  catch { } 
    //  try {
    //    // BUGFIX: PS 16.08.13 Not working for some reasons
    //    //Uri uri = new Uri(@"pack://" + Path.GetTempFileName(), UriKind.Absolute);
    //    //var memoryStream = new MemoryStream();
    //    //var package = Package.Open(memoryStream, FileMode.Create, FileAccess.ReadWrite);
    //    //PackageStore.AddPackage(uri, package);
    //    //var xpsDocument = new XpsDocument(package, CompressionOption.NotCompressed, uri.AbsoluteUri);
    //    //var rsm = new XpsSerializationManager(new XpsPackagingPolicy(xpsDocument), false);
    //    //paginator.Progress += paginator_Progress;
    //    //rsm.SaveAsXaml(paginator);
    //    //preview.Document = xpsDocument.GetFixedDocumentSequence();
    //    //xpsDocument.Close();
    //    // WORKAROUND:
    //    string tempFileName = Path.GetTempFileName();
    //    if (File.Exists(tempFileName))
    //      File.Delete(tempFileName);
    //    paginator.Progress -= paginator_Progress;
    //    paginator.Progress += paginator_Progress;
    //    using (var xpsDocument = new XpsDocument(tempFileName, FileAccess.ReadWrite)) {
    //      var writer = XpsDocument.CreateXpsDocumentWriter(xpsDocument);
    //      writer.Write(paginator);
    //      preview.Document = xpsDocument.GetFixedDocumentSequence();
    //    }
    //    // END WORKAROUND
    //  }
    //  catch (Exception ex) {
    //    NAS.ES.WPF.Toolkit.MessageBox.Show(ex.Message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
    //  }
    //  try {
    //    Dispatcher.Invoke(updatePbDelegate, DispatcherPriority.Background, new object[] { BusyIndicator.IsBusyProperty, false });
    //  }
    //  catch { }
    //}

    //private delegate void UpdateProgressBarDelegate(DependencyProperty dp, Object value);

    //void paginator_Progress(object sender, ProgressEventArgs e)
    //{
    //  UpdateProgressBarDelegate updatePbDelegate = new UpdateProgressBarDelegate(bi.SetValue);
    //  try {
    //    Dispatcher.Invoke(updatePbDelegate, DispatcherPriority.Background, new object[] { BusyIndicator.BusyContentProperty, "Erstelle Seiten: " + Math.Round(e.Progress, 0) + " %" });
    //  }
    //  catch { }
    //}

    //private void preview_PageSizeChanged(object sender, EventArgs e)
    //{
    //  Size pageSize;
    //  if (preview.Orientation == PageOrientation.Landscape || preview.Orientation == PageOrientation.ReverseLandscape)
    //    pageSize = new Size(preview.PageSize.Height.Value, preview.PageSize.Width.Value);
    //  else
    //    pageSize = new Size(preview.PageSize.Width.Value, preview.PageSize.Height.Value);
    //  paginator.PageSize = pageSize;
    //  paginator.PrintableArea = preview.PrintableArea;
    //  paginator.Orientation = preview.Orientation;
    //  updatePreview();
    //}

    //private Canvas addFrame(Canvas canvas)
    //{
    //  var layout = schedule.ActiveLayout;
    //  var c = canvas.Clone(layout.LeftMargin * 96 / 2.54, layout.TopMargin * 96 / 2.54, layout.HeaderHeight * 96 / 2.54);
    //  var gridHeader = new Grid();
    //  var gridFooter = new Grid();
    //  c.Children.Add(gridHeader);
    //  c.Children.Add(gridFooter);
    //  gridHeader.Height = layout.HeaderHeight * 96 / 2.54;
    //  gridHeader.Width = c.Width - (layout.LeftMargin + layout.RightMargin) * 96 / 2.54;
    //  Canvas.SetTop(gridHeader, layout.TopMargin * 96 / 2.54);
    //  Canvas.SetLeft(gridHeader, layout.LeftMargin * 96 / 2.54);
    //  gridFooter.Height = layout.FooterHeight * 96 / 2.54;
    //  gridFooter.Width = c.Width - (layout.LeftMargin + layout.RightMargin) * 96 / 2.54;
    //  Canvas.SetTop(gridFooter, c.Height - gridFooter.Height - layout.BottomMargin * 96 / 2.54);
    //  Canvas.SetLeft(gridFooter, layout.LeftMargin * 96 / 2.54);
    //  foreach (var definition in layout.HeaderDefinitions.OrderBy(x => x.Column)) {
    //    if (definition.IsHeader)
    //      gridHeader.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition());
    //    else
    //      gridFooter.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition());
    //    var richTextBox = new Xceed.Wpf.Toolkit.RichTextBox() { BorderThickness = new Thickness(0) };
    //    richTextBox.TextFormatter = new RtfFormatter();
    //    if (definition.IsHeader)
    //      gridHeader.Children.Add(richTextBox);
    //    else
    //      gridFooter.Children.Add(richTextBox);
    //    var text = definition.Definition;
    //    if (text != null) {
    //      while (text.contains("##ProjectName##"))
    //        text = text.Replace("##ProjectName##", schedule.Name);
    //      while (text.contains("##DataDate##"))
    //        text = text.Replace("##DataDate##", schedule.DataDate.ToString("yyyy-MM-dd"));
    //      while (text.contains("##Date##"))
    //        text = text.Replace("##Date##", DateTime.Today.ToString("yyyy-MM-dd"));
    //    }
    //    richTextBox.Text = text;
    //    richTextBox.IsReadOnly = true;
    //    Grid.SetColumn(richTextBox, definition.Column - 1);
    //  }
    //  return c;
    //}
  }
}
