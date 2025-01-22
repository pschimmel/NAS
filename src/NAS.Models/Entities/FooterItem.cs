using NAS.Models.Base;

namespace NAS.Models.Entities
{
  public class FooterItem : NASObject, IPrintLayoutItem, IClonable<FooterItem>
  {
    private string _definition;
    private int _column;

    public FooterItem()
    {
      _column = 0;
    }

    public FooterItem(FooterItem other)
    {
      _column = other._column;
      _definition = other._definition;
    }

    public string Definition
    {
      get => _definition;
      set
      {
        if (_definition != value)
        {
          _definition = value;
          OnPropertyChanged(nameof(Definition));
        }
      }
    }

    public int Column
    {
      get => _column;
      set
      {
        if (_column != value)
        {
          _column = value;
          OnPropertyChanged(nameof(Column));
        }
      }
    }

    public FooterItem Clone()
    {
      return new FooterItem(this);
    }
  }
}
