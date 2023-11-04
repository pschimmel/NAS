using NAS.Resources;

namespace NAS.Model.Entities
{
  public class RowDefinition : NASObject
  {
    private double? height = null;
    private int sort;

    public RowDefinition()
    { }

    public RowDefinition(RowDefinition other)
    {
      Definition = other.Definition;
      Height = other.Height;
      Sort = other.Sort;
    }

    public double? Height
    {
      get => height;
      set
      {
        if (value < 0)
        {
          value = 0;
        }

        if (height != value)
        {
          height = value;
          OnPropertyChanged(nameof(Height));
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

    public virtual PERTDefinition Definition { get; set; }

    public string Name
    {
      get
      {
        string s = NASResources.Row;
        if (height.HasValue)
        {
          s += " (" + height.Value + ")";
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
