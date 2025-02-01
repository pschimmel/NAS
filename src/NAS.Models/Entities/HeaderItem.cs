using NAS.Models.Base;

namespace NAS.Models.Entities
{
  public class HeaderItem : NASObject, IPrintLayoutItem, IClonable<HeaderItem>
  {
    public HeaderItem()
    {
      Column = 0;
    }

    public HeaderItem(HeaderItem other)
    {
      Column = other.Column;
      Definition = other.Definition;
    }

    public string Definition { get; set; }

    public int Column { get; set; }

    public HeaderItem Clone()
    {
      return new HeaderItem(this);
    }
  }
}
