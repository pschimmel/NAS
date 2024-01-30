using ES.Tools.Core.MVVM;

namespace NAS.Model.Entities
{
  public class NASObject : NotifyObject
  {
    public Guid ID { get; set; } = Guid.NewGuid();
  }
}
