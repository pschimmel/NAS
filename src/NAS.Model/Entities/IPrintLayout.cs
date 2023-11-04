using System.Collections.Generic;

namespace NAS.Model.Entities
{
  public interface IPrintLayout
  {
    double FooterHeight { get; set; }
    ICollection<FooterItem> FooterItems { get; set; }
    double HeaderHeight { get; set; }
    ICollection<HeaderItem> HeaderItems { get; set; }
    double MarginBottom { get; set; }
    double MarginLeft { get; set; }
    double MarginRight { get; set; }
    double MarginTop { get; set; }
  }
}