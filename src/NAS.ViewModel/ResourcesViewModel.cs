using System.Collections.ObjectModel;
using System.Windows.Input;
using ES.Tools.Core.MVVM;
using NAS.Model.Controllers;
using NAS.Model.Entities;
using NAS.Resources;
using NAS.ViewModel.Base;
using NAS.ViewModel.Helpers;

namespace NAS.ViewModel
{
  public class ResourcesViewModel : ViewModelBase
  {
    #region Fields

    public Schedule _schedule;
    private Resource _currentGlobalResource;
    private Resource _currentResource;

    #endregion

    #region Constructor

    public ResourcesViewModel(Schedule schedule)
      : base()
    {
      _schedule = schedule;
      Resources = new ObservableCollection<Resource>(schedule.Resources);
      GlobalResources = new ObservableCollection<Resource>(GlobalDataController.Resources);
      AddResourceCommand = new ActionCommand(AddResourceCommandExecute);
      RemoveResourceCommand = new ActionCommand(RemoveResourceCommandExecute, () => RemoveResourceCommandCanExecute);
      EditResourceCommand = new ActionCommand(EditResourceCommandExecute, () => EditResourceCommandCanExecute);
      AddGlobalResourceCommand = new ActionCommand(AddGlobalResourceCommandExecute);
      RemoveGlobalResourceCommand = new ActionCommand(RemoveGlobalResourceCommandExecute, () => RemoveGlobalResourceCommandCanExecute);
      EditGlobalResourceCommand = new ActionCommand(EditGlobalResourceCommandExecute, () => EditGlobalResourceCommandCanExecute);
    }

    #endregion

    #region Public Members

    public override HelpTopic HelpTopicKey => HelpTopic.Resources;

    public ObservableCollection<Resource> Resources { get; }

    public Resource CurrentResource
    {
      get => _currentResource;
      set
      {
        if (_currentResource != value)
        {
          _currentResource = value;
          OnPropertyChanged(nameof(CurrentResource));
        }
      }
    }

    public ObservableCollection<Resource> GlobalResources { get; }

    public Resource CurrentGlobalResource
    {
      get => _currentGlobalResource;
      set
      {
        if (_currentGlobalResource != value)
        {
          _currentGlobalResource = value;
          OnPropertyChanged(nameof(CurrentGlobalResource));
        }
      }
    }

    #endregion

    #region Add Resource

    public ICommand AddResourceCommand { get; }

    private void AddResourceCommandExecute()
    {
      using var vm = new AddResourceViewModel();
      if (ViewFactory.Instance.ShowDialog(vm) == true)
      {
        Resource newResource;
        if (vm.IsWorkResourceSelected)
        {
          newResource = new WorkResource();
        }
        else if (vm.IsMaterialResourceSelected)
        {
          newResource = new MaterialResource();
        }
        else if (vm.IsCalendarResourceSelected)
        {
          newResource = new CalendarResource();
        }
        else
        {
          return;
        }

        newResource.Schedule = _schedule;

        using var vm2 = new ResourceDetailsViewModel(newResource);
        if (ViewFactory.Instance.ShowDialog(vm2) == true)
        {
          vm2.Apply();
          Resources.Add(newResource);
          CurrentResource = newResource;
        }
      };
    }

    #endregion

    #region Remove Resource

    public ICommand RemoveResourceCommand { get; }

    private void RemoveResourceCommandExecute()
    {
      if (!_schedule.CanRemoveResource(CurrentResource))
      {
        UserNotificationService.Instance.Error(NASResources.MessageCannotRemoveResource);
        return;
      }

      UserNotificationService.Instance.Question(NASResources.MessageDeleteResource, () =>
      {
        _ = Resources.Remove(CurrentResource);
        _ = _schedule.Resources.Remove(CurrentResource);
        CurrentResource = null;
      });
    }

    private bool RemoveResourceCommandCanExecute => CurrentResource != null;

    #endregion

    #region Edit Resource

    public ICommand EditResourceCommand { get; }

    private void EditResourceCommandExecute()
    {
      using var vm = new ResourceDetailsViewModel(CurrentResource);
      if (ViewFactory.Instance.ShowDialog(vm) == true)
      {
        vm.Apply();
      }
    }

    private bool EditResourceCommandCanExecute => CurrentResource != null;

    #endregion

    #region Add Global Resource

    public ICommand AddGlobalResourceCommand { get; }

    private void AddGlobalResourceCommandExecute()
    {
      using var vm = new AddResourceViewModel();
      if (ViewFactory.Instance.ShowDialog(vm) == true)
      {
        Resource newResource;
        if (vm.IsWorkResourceSelected)
        {
          newResource = new WorkResource();
        }
        else if (vm.IsMaterialResourceSelected)
        {
          newResource = new MaterialResource();
        }
        else if (vm.IsCalendarResourceSelected)
        {
          newResource = new CalendarResource();
        }
        else
        {
          return;
        }

        newResource.Schedule = _schedule;

        using var vm2 = new ResourceDetailsViewModel(newResource);
        if (ViewFactory.Instance.ShowDialog(vm2) == true)
        {
          vm2.Apply();
          GlobalResources.Add(newResource);
          GlobalDataController.Resources.Add(newResource);
          CurrentGlobalResource = newResource;
        }
      }
    }

    #endregion

    #region Remove Global Resource

    public ICommand RemoveGlobalResourceCommand { get; }

    private void RemoveGlobalResourceCommandExecute()
    {
      if (!_schedule.CanRemoveResource(CurrentGlobalResource))
      {
        UserNotificationService.Instance.Error(NASResources.MessageCannotRemoveResource);
        return;
      }

      UserNotificationService.Instance.Question(NASResources.MessageDeleteResource, () =>
      {
        _ = GlobalResources.Remove(CurrentGlobalResource);
        _ = GlobalDataController.Resources.Remove(CurrentGlobalResource);
        CurrentGlobalResource = null;
      });
    }

    private bool RemoveGlobalResourceCommandCanExecute => CurrentGlobalResource != null;

    #endregion

    #region Edit Global Resource

    public ICommand EditGlobalResourceCommand { get; }

    private void EditGlobalResourceCommandExecute()
    {
      using var vm = new ResourceDetailsViewModel(CurrentGlobalResource);
      if (ViewFactory.Instance.ShowDialog(vm) == true)
      {
        vm.Apply();
      }
    }

    private bool EditGlobalResourceCommandCanExecute => CurrentGlobalResource != null;

    #endregion
  }
}
