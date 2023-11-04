﻿using NAS.ViewModel.Helpers;

namespace NAS.ViewModel.Base
{
  public abstract class ViewModelBase : ES.Tools.Core.MVVM.ViewModel
  {
    protected ViewModelBase()
    { }

    public virtual HelpTopic HelpTopicKey => HelpTopic.None;

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);
    }
  }
}
