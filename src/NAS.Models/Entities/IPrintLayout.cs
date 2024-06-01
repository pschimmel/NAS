using System.Collections.ObjectModel;

namespace NAS.Models.Entities
{
  public interface IPrintLayout
  {
    double FooterHeight { get; set; }

    ObservableCollection<FooterItem> FooterItems { get; }

    double HeaderHeight { get; set; }

    ObservableCollection<HeaderItem> HeaderItems { get; }

    double MarginBottom { get; set; }

    double MarginLeft { get; set; }

    double MarginRight { get; set; }

    double MarginTop { get; set; }
  }
}