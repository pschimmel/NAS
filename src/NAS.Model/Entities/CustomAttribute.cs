using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NAS.Model.Entities
{
  public class CustomAttribute : NASObject
  {
    private string name;

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

    public virtual Schedule Schedule { get; set; }

    public override string ToString()
    {
      return name;
    }
  }
}
