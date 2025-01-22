using System.Collections.ObjectModel;
using NAS.Models.Entities;
using NAS.Resources;
using NAS.ViewModels.Base;
using NAS.ViewModels.Helpers;

namespace NAS.ViewModels
{
  public class EditPrintLayoutViewModel : DialogContentViewModel
  {
    #region Fields

    private readonly Layout _layout;
    private int _headerColumnCount;
    private int _footerColumnCount;

    #endregion

    #region Constructor

    public EditPrintLayoutViewModel(Layout layout)
      : base()
    {
      _layout = layout;
      LayoutName = layout.Name;
      LeftMargin = layout.LeftMargin;
      RightMargin = layout.RightMargin;
      TopMargin = layout.TopMargin;
      BottomMargin = layout.BottomMargin;
      _headerColumnCount = layout.HeaderItems.Count;
      _footerColumnCount = layout.FooterItems.Count;
      foreach (var headerItem in layout.HeaderItems)
      {
        HeaderItems.Add(headerItem.Clone());
      }

      foreach (var footerItem in layout.FooterItems)
      {
        FooterItems.Add(footerItem.Clone());
      }

      HeaderHeight = layout.HeaderHeight;
      FooterHeight = layout.FooterHeight;
    }

    #endregion

    #region Overwritten Members

    public override string Title => NASResources.PrintLayout;

    public override string Icon => "Layout";

    public override DialogSize DialogSize => DialogSize.Fixed(700, 550);

    public override HelpTopic HelpTopicKey => HelpTopic.Layout;

    #endregion

    #region Properties

    public string LayoutName { get; }

    public double LeftMargin { get; set; }

    public double RightMargin { get; set; }

    public double TopMargin { get; set; }

    public double BottomMargin { get; set; }

    public double HeaderHeight { get; set; }

    public double FooterHeight { get; set; }

    public int HeaderColumnCount
    {
      get => _headerColumnCount;
      set
      {
        if (_headerColumnCount != value)
        {
          while (value > HeaderItems.Count)
          {
            HeaderItems.Add(new HeaderItem() { Column = HeaderItems.Count });
          }

          while (value < HeaderItems.Count)
          {
            HeaderItems.RemoveAt(HeaderItems.Count - 1);
          }

          _headerColumnCount = value;
          OnPropertyChanged(nameof(HeaderColumnCount));
          OnPropertyChanged(nameof(HeaderItems));
          OnPropertyChanged(nameof(HeaderStarColumns));
        }
      }
    }

    public ObservableCollection<HeaderItem> HeaderItems { get; }

    public string HeaderStarColumns
    {
      get
      {
        string result = "";
        for (int i = 0; i < HeaderColumnCount; i++)
        {
          if (!string.IsNullOrWhiteSpace(result))
          {
            result += ",";
          }

          result += i;
        }
        return result;
      }
    }

    public int FooterColumnCount
    {
      get => _footerColumnCount;
      set
      {
        if (_footerColumnCount != value)
        {
          while (value > FooterItems.Count)
          {
            FooterItems.Add(new FooterItem() { Column = HeaderItems.Count });
          }

          while (value < FooterItems.Count)
          {
            FooterItems.RemoveAt(FooterItems.Count - 1);
          }

          _footerColumnCount = value;
          OnPropertyChanged(nameof(FooterColumnCount));
          OnPropertyChanged(nameof(FooterItems));
          OnPropertyChanged(nameof(FooterStarColumns));
        }
      }
    }

    public ObservableCollection<FooterItem> FooterItems { get; }

    public string FooterStarColumns
    {
      get
      {
        string result = "";
        for (int i = 0; i < FooterColumnCount; i++)
        {
          if (!string.IsNullOrWhiteSpace(result))
          {
            result += ",";
          }

          result += i;
        }
        return result;
      }
    }

    #endregion

    #region Apply

    protected override void OnApply()
    {
      if (LeftMargin >= 0)
      {
        _layout.LeftMargin = LeftMargin;
      }

      if (RightMargin >= 0)
      {
        _layout.RightMargin = RightMargin;
      }

      if (TopMargin >= 0)
      {
        _layout.TopMargin = TopMargin;
      }

      if (BottomMargin >= 0)
      {
        _layout.BottomMargin = BottomMargin;
      }

      if (HeaderHeight >= 0)
      {
        _layout.HeaderHeight = HeaderHeight;
      }

      if (FooterHeight >= 0)
      {
        _layout.FooterHeight = FooterHeight;
      }

      _layout.HeaderItems.Clear();
      _layout.FooterItems.Clear();

      foreach (var headerItem in HeaderItems)
      {
        _layout.HeaderItems.Add(headerItem);
      }

      foreach (var footerItem in FooterItems)
      {
        _layout.FooterItems.Add(footerItem);
      }
    }

    #endregion
  }
}
