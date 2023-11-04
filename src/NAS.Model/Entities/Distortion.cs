
namespace NAS.Model.Entities
{
  public abstract class Distortion : NASObject
  {
    public string description;
    private Fragnet fragnet;

    public string Description
    {
      get => description;
      set
      {
        if (description != value)
        {
          description = value;
          OnPropertyChanged(nameof(Description));
        }
      }
    }

    public virtual Activity Activity { get; set; }

    public virtual Fragnet Fragnet
    {
      get => fragnet;
      set
      {
        if (fragnet != value)
        {
          fragnet = value;
          OnPropertyChanged(nameof(Fragnet));
        }
      }
    }
  }
}
