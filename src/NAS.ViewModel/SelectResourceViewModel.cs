using System.Collections.Generic;
using System.Linq;
using NAS.Model.Entities;
using NAS.ViewModel.Base;
using NAS.ViewModel.Helpers;

namespace NAS.ViewModel
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
