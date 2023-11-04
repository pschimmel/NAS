using NAS.Resources;

namespace NAS.Model.Entities
{
  public class ColumnDefinition : NASObject
  {
    private double? width = null;
    private int sort;

    public ColumnDefinition()
    { }

    public ColumnDefinition(ColumnDefinition other)
    {
      Definition = other.Definition;
      Width = other.Width;
      Sort = other.Sort;
    }

    public double? Width
    {
      get => width;
      set
      {
        if (value < 0)
        {
          value = 0;
        }

        if (width != value)
        {
          width = value;
          OnPropertyChanged(nameof(Width));
          OnPropertyChanged(nameof(Name));
        }
      }
    }

    public int Sort
    {
      get => sort;
      set
      {
        if (value < 0)
        {
          value = 0;
        }

        if (sort != value)
        {
          sort = value;
          OnPropertyChanged(nameof(Sort));
        }
      }
    }

    public string Name
    {
      get
      {
        string s = NASResources.Column;
        if (width.HasValue)
        {
          s += " (" + width.Value + ")";
        }
        else
        {
          s += " (Auto)";
        }

        return s;
      }
    }

    public virtual PERTDefinition Definition { get; set; }

    public override string ToString()
    {
      return Name;
    }
  }
}
