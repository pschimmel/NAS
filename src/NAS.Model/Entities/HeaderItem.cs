
namespace NAS.Model.Entities
{
  public class HeaderItem : NASObject, IPrintLayoutItem
  {
    private string definition;
    private int column;

    public HeaderItem()
    {
      column = 0;
    }

    public HeaderItem(HeaderItem other)
    {
      column = other.column;
      definition = other.definition;
    }

    public string Definition
    {
      get => definition;
      set
      {
        if (definition != value)
        {
          definition = value;
          OnPropertyChanged(nameof(Definition));
        }
      }
    }

    public int Column
    {
      get => column;
      set
      {
        if (column != value)
        {
          column = value;
          OnPropertyChanged(nameof(Column));
        }
      }
    }

    public virtual Layout Layout { get; set; }
  }
}
