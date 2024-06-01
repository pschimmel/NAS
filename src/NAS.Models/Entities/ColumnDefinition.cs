using NAS.Resources;

namespace NAS.Models.Entities
{
  public class ColumnDefinition : NASObject
  {
    private double? _width;
    private int _sort;

    public ColumnDefinition()
    { }

    public ColumnDefinition(ColumnDefinition other)
    {
      Width = other.Width;
      Sort = other.Sort;
    }

    public double? Width
    {
      get => _width;
      set
      {
        if (value < 0)
        {
          value = 0;
        }

        if (_width != value)
        {
          _width = value;
          OnPropertyChanged(nameof(Width));
          OnPropertyChanged(nameof(Name));
        }
      }
    }

    public int Sort
    {
      get => _sort;
      set
      {
        if (value < 0)
        {
          value = 0;
        }

        if (_sort != value)
        {
          _sort = value;
          OnPropertyChanged(nameof(Sort));
        }
      }
    }

    public string Name
    {
      get
      {
        string s = NASResources.Column;
        if (_width.HasValue)
        {
          s += $" ({_width.Value})";
        }
        else
        {
          s += " (Auto)";
        }

        return s;
      }
    }

    public override string ToString()
    {
      return Name;
    }
  }
}
