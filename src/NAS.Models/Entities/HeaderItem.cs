namespace NAS.Models.Entities
{
  public class HeaderItem : NASObject, IPrintLayoutItem
  {
    public HeaderItem()
    {
      Column = 0;
    }

    private HeaderItem(HeaderItem other)
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
