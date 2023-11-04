using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NAS.Model.Entities
{
  public abstract class Resource : NASObject
  {
    private string name;
    private decimal costsPerUnit;
    private double? limit;

    protected Resource()
    {
      ResourceAssociations = new ObservableCollection<ResourceAssociation>();
      VisibleResources = new ObservableCollection<VisibleResource>();
    }

    public string Name
    {
      get => name;
      set
      {
        if (name != value)
        {
          name = value;
          OnPropertyChanged(nameof(Name));
        }
      }
    }

    public decimal CostsPerUnit
    {
      get => costsPerUnit;
      set
      {
        if (costsPerUnit != value)
        {
          costsPerUnit = value;
          OnPropertyChanged(nameof(CostsPerUnit));
        }
      }
    }

    public double? Limit
    {
      get => limit;
      set
      {
        if (limit != value)
        {
          limit = value;
          OnPropertyChanged(nameof(Limit));
        }
      }
    }

    public virtual ICollection<ResourceAssociation> ResourceAssociations { get; set; }

    public virtual ICollection<VisibleResource> VisibleResources { get; set; }

    public virtual Schedule Schedule { get; set; }

    public override string ToString()
    {
      return base.ToString();
    }
  }
}
