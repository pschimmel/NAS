using System.Windows.Media;
using NAS.Models.Enums;
using NAS.Resources;
using NAS.ViewModels.Base;
using NAS.ViewModels.Helpers;

namespace NAS.ViewModels
{
  public class SelectGroupingDefinitionViewModel : DialogContentViewModel
  {
    #region Fields

    private Color selectedColor;
    private ActivityProperty selectedActivityProperty;

    #endregion

    #region Constructor

    public SelectGroupingDefinitionViewModel(ActivityProperty property, Color color)
      : base()
    {
      selectedActivityProperty = property;
      SelectedColor = color;
    }

    public SelectGroupingDefinitionViewModel()
      : base()
    {
      if (ActivityProperties.Count != 0)
      {
        selectedActivityProperty = ActivityProperties.First();
      }
    }

    #endregion

    #region Overwritten Members

    public override string Title => NASResources.Grouping;

    public override string Icon => "GroupAndSort";

    public override DialogSize DialogSize => DialogSize.Fixed(350, 150);

    public override HelpTopic HelpTopicKey => HelpTopic.GroupAndSort;

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
