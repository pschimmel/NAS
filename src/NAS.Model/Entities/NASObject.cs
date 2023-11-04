using System;
using System.ComponentModel.DataAnnotations.Schema;
using ES.Tools.Core.MVVM;

namespace NAS.Model.Entities
{
  public class NASObject : NotifyObject
  {
    public string ID
    {
      get => Guid.ToString();
      set => Guid = Guid.Parse(value);
    }

    [NotMapped]
    public Guid Guid { get; set; } = Guid.NewGuid();
  }
}
