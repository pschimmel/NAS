﻿using NAS.Resources;
using NAS.ViewModel.Base;

namespace NAS.ViewModel
{
  public class AddResourceViewModel : DialogContentViewModel
  {
    public override string Title => NASResources.AddResource;

    public override string Icon => "Resources";

    public bool IsMaterialResourceSelected { get; set; } = true;

    public bool IsWorkResourceSelected { get; set; }

    public bool IsCalendarResourceSelected { get; set; }
  }
}
