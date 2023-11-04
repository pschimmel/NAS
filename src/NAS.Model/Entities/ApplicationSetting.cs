
namespace NAS.Model.Entities
{
  public class ApplicationSetting : NASObject
  {
    private string name;
    private string version;

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

    public string Version
    {
      get => version;
      set
      {
        if (version != value)
        {
          version = value;
          OnPropertyChanged(nameof(Version));
        }
      }
    }
  }
}
