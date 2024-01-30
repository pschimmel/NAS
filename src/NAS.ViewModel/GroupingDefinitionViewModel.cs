using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using NAS.Model.Enums;
using NAS.ViewModel.Base;

namespace NAS.ViewModel
{
  public class GroupingDefinitionViewModel : ViewModelBase
  {
    #region Fields

    private Color selectedColor;
    private ActivityProperty selectedActivityProperty;

    #endregion

    #region Constructor

    public GroupingDefinitionViewModel(ActivityProperty property, Color color)
      : base()
    {
      selectedActivityProperty = property;
      SelectedColor = color;
    }

    public GroupingDefinitionViewModel()
      : base()
    {
      if (ActivityProperties.Count != 0)
      {
        selectedActivityProperty = ActivityProperties.First();
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

    public Color SelectedColor
    {
      get => selectedColor;
      set
      {
        if (selectedColor != value)
        {
          selectedColor = value;
          OnPropertyChanged(nameof(SelectedColor));
        }
      }
    }

    #endregion
  }
}
