﻿using System;
using System.Collections.Generic;
using System.Linq;
using NAS.Model.Enums;
using NAS.ViewModel.Base;

namespace NAS.ViewModel
{
  public class SortingDefinitionViewModel : ViewModelBase
  {
    #region Fields

    private ActivityProperty selectedActivityProperty;
    private SortDirection selectedSortDirection;

    #endregion

    #region Constructor

    public SortingDefinitionViewModel(ActivityProperty property, SortDirection direction)
      : base()
    {
      selectedActivityProperty = property;
      selectedSortDirection = direction;
    }

    public SortingDefinitionViewModel()
      : base()
    {
      if (ActivityProperties.Any())
      {
        selectedActivityProperty = ActivityProperties.First();
      }

      if (SortDirections.Any())
      {
        selectedSortDirection = SortDirections.First();
      }
    }

    #endregion

    #region Public Properties

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

    public List<SortDirection> SortDirections => Enum.GetValues(typeof(SortDirection)).Cast<SortDirection>().ToList();

    public SortDirection SelectedSortDirection
    {
      get => selectedSortDirection;
      set
      {
        if (selectedSortDirection != value)
        {
          selectedSortDirection = value;
          OnPropertyChanged(nameof(SelectedSortDirection));
        }
      }
    }

    #endregion
  }
}
