using NAS.Models.Entities;

namespace NAS.ViewModels.Base
{
  public interface ILayoutViewModel
  {
    Layout Layout { get; }

    IPrintableCanvas Canvas { get; set; }
    ScheduleViewModel Schedule { get; }
  }
}