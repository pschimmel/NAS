namespace NAS.ViewModels.Helpers
{
  public class Change
  {
    public ChangeType ChangeType { get; set; }

    public string Description { get; set; }

    public string Version { get; set; }

    public override string ToString()
    {
      return $"[{ChangeType}] {Description}";
    }
  }

  public enum ChangeType
  {
    Bug,
    Feature,
    Enhancement,
    Info
  }
}
