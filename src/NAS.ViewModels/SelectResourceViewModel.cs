using System.Collections.Generic;
using System.Linq;
using NAS.Models.Entities;
using NAS.ViewModels.Base;
using NAS.ViewModels.Helpers;

namespace NAS.ViewModels
{
  public class SelectResourceViewModel : ViewModelBase
  {
    #region Fields

    private Resource selectedResource;

    #endregion

    #region Constructor

    public SelectResourceViewModel(Schedule schedule)
      : base()
    {
      Resources = schedule.Resources.ToList();
    }

    #endregion

    #region Public Properties

    public override HelpTopic HelpTopicKey => HelpTopic.Resources;

    public List<Resource> Resources { get; }

    public Resource SelectedResource
    {
      get => selectedResource;
      set
      {
        if (selectedResource != value)
        {
          selectedResource = value;
          OnPropertyChanged(nameof(SelectedResource));
        }
      }
    }

    #endregion
  }
}
