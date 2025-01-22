using System.Collections.ObjectModel;

namespace NAS.Models.Entities
{
  public interface IPrintLayout
  {
    double FooterHeight { get; set; }

    ObservableCollection<FooterItem> FooterItems { get; }

    double HeaderHeight { get; set; }

    ObservableCollection<HeaderItem> HeaderItems { get; }

    double BottomMargin { get; set; }

    double LeftMargin { get; set; }

    double RightMargin { get; set; }

    double TopMargin { get; set; }
  }
}
