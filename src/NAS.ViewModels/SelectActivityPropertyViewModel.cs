﻿using System;
using System.Collections.Generic;
using System.Linq;
using NAS.Models.Enums;
using NAS.ViewModels.Base;

namespace NAS.ViewModels
{
  public class SelectActivityPropertyViewModel : ViewModelBase
  {
    #region Fields

    private ActivityProperty selectedActivityProperty;

    #endregion

    #region Constructor

    public SelectActivityPropertyViewModel()
      : base()
    {
      if (ActivityProperties.Count != 0)
      {
        selectedActivityProperty = ActivityProperties.First();
      }
    }

    #endregion

    #region Properties

    public List<ActivityProperty> ActivityProperties
    {
      get
      {
        var list = new List<ActivityProperty>();
        foreach (ActivityProperty item in Enum.GetValues(typeof(ActivityProperty)))
        {
          if (item != ActivityProperty.None)
          {
            list.Add(item);
          }
        }

        return list;
      }
    }

    public ActivityProperty SelectedActivityProperty
    {
      get => selectedActivityProperty;
      set
      {
        if (selectedActivityProperty != value)
        {
          selectedActivityProperty = value;
          OnPropertyChanged(nameof(SelectedActivityProperty));
        }
      }
    }

    #endregion
  }
}
