using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using NAS.Model.Entities;
using NAS.ViewModel.Base;

namespace NAS.ViewModel
{
  public class PrintLayoutViewModel : ViewModelBase, IApplyable
  {
    #region Fields

    private readonly Layout _layout;
    private int _headerColumnCount;
    private int _footerColumnCount;

    #endregion

    #region Constructor

    public PrintLayoutViewModel(Layout layout)
      : base()
    {
      _layout = layout;
      _headerColumnCount = layout.HeaderItems.Count;
      _footerColumnCount = layout.FooterItems.Count;
    }

    #endregion

    #region Public Properties

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

    #region IApplyable

    public void Apply()
    {
      _layout.RefreshPrintLayout(HeaderItems, FooterItems);
    }

    #endregion
  }
}
