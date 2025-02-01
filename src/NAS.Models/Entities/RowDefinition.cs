using NAS.Resources;

namespace NAS.Models.Entities
{
  public class RowDefinition : NASObject
  {
    private double? _height = null;
    private int _sort;

    public RowDefinition()
    { }

    public RowDefinition(RowDefinition other)
    {
      Height = other.Height;
      Sort = other.Sort;
    }

    public double? Height
    {
      get => _height;
      set
      {
        if (value < 0)
        {
          value = 0;
        }

        if (_height != value)
        {
          _height = value;
          OnPropertyChanged(nameof(Height));
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
        string s = NASResources.Row;
        if (_height.HasValue)
        {
          s += $" ({_height.Value})";
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
