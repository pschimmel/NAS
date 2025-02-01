using NAS.Models.Base;

namespace NAS.Models.Entities
{
  public class FooterItem : NASObject, IPrintLayoutItem, IClonable<FooterItem>
  {
    public FooterItem()
    {
      Column = 0;
    }

    public FooterItem(FooterItem other)
    {
      Column = other.Column;
      Definition = other.Definition;
    }

    public string Definition { get; set; }

    public int Column { get; set; }

    public FooterItem Clone()
    {
      return new FooterItem(this);
    }
  }
}
