using System;
using NAS.Model.Entities;
using NAS.Model.Enums;

namespace NAS.ViewModel.Base
{
  public interface IVisibleLayoutViewModel : IDisposable
  {
    bool IsVisible { get; set; }
    Layout Layout { get; }
    string Name { get; }
    LayoutType LayoutType { get; }
  }
}
