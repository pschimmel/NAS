using System.Diagnostics;
using NAS.Model.Base;
using NAS.Resources;
using NAS.ViewModel.Base;

namespace NAS.ViewModel
{
  public class ReportViewModel : ViewModelBase
  {
    #region Fields

    private Lazy<object> _imageCache;

    #endregion

    #region Constructors

    public ReportViewModel(Model.Entities.Report report)
    {
      Report = report;
      report.NameChanged += (_, __) =>
      {
        OnPropertyChanged(nameof(Name));
      };

      report.DataChanged += (_, __) =>
      {
        _imageCache = new Lazy<object>(GetImage);
        OnPropertyChanged(nameof(Image));
      };

      _imageCache = new Lazy<object>(GetImage);
    }

    #endregion

    #region Public Properties

    public string Name => Report.Name;

    public string Category
    {
      get
      {
        var manager = NASResources.ResourceManager;
        return manager.GetString(Report.ReportType.ToString());
      }
    }

    internal Model.Entities.Report Report { get; }

    public object Image => _imageCache.Value;

    #endregion

    #region Private Members

    private object GetImage()
    {
      if (Report == null || string.IsNullOrWhiteSpace(Report.Data))
      {
        return null;
      }

      try
      {
        var reportEvent = EventAggregator.Instance.GetEvent<ReportEvent>();
        var data = new ReportData(Report);
        reportEvent.Publish(data);
        return data.Image;

        // TODO: Move code to View

        //using var report = new FastReport.Report();
        //report.LoadFromString(Report.Data);
        //var info = report.ReportInfo;

        //if (info.Picture != null)
        //{
        //  //var image = ES.Tools.Drawing.Images.ChangeSize(info.Picture, 64, 64);
        //  var bs = Images.ConvertImageToBitmapSource(info.Picture, ImageFormat.Jpeg);
        //  bs.Freeze();
        //  return bs;
        //}
      }
      catch (Exception ex)
      {
        Debug.Fail("Cannot convert image: " + ex.Message);
      }

      return null;
    }

    #endregion
  }
}
