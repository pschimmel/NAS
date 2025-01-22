using NAS.Models.Entities;

namespace NAS.Models.Base
{
  public interface IClonable<T> where T : NASObject
  {
    T Clone();
  }
}
