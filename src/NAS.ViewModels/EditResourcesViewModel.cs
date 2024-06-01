using System.Collections.ObjectModel;
using System.Windows.Input;
using ES.Tools.Core.MVVM;
using NAS.Models.Controllers;
using NAS.Models.Entities;
using NAS.Resources;
using NAS.ViewModels.Base;
using NAS.ViewModels.Helpers;

namespace NAS.ViewModels
{
  public class EditResourcesViewModel : DialogContentViewModel
  {
    #region Fields

    public Schedule _schedule;
    private Resource _currentGlobalResource;
    private Resource _currentResource;
    private ActionCommand _addResourceCommand;
    private ActionCommand _removeResourceCommand;
    private ActionCommand _editResourceCommand;
    private ActionCommand _addGlobalResourceCommand;
    private ActionCommand _removeGlobalResourceCommand;
    private ActionCommand _editGlobalResourceCommand;

    #endregion

    #region Constructor

    public EditResourcesViewModel(Schedule schedule)
      : base()
    {
      _schedule = schedule;
      Resources = new ObservableCollection<Resource>(schedule.Resources);
      GlobalResources = new ObservableCollection<Resource>(GlobalDataController.Resources);
    }

    #endregion

    #region Overwritten Members

    public override string Title => NASResources.EditResources;

    public override string Icon => "Resources";

    public override DialogSize DialogSize => DialogSize.Fixed(300, 300);

    public override HelpTopic HelpTopicKey => HelpTopic.Resources;

    #endregion

    #region Public Members

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

    #region Add Global Resource

    public ICommand AddGlobalResourceCommand => _addGlobalResourceCommand ??= new ActionCommand(AddGlobalResource);

    private void AddGlobalResource()
    {
      using var vm = new ChooseResourceTypeViewModel();
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

        using var vm2 = new ResourceDetailsViewModel(newResource);
        if (ViewFactory.Instance.ShowDialog(vm2) == true)
        {
          GlobalResources.Add(newResource);
          CurrentGlobalResource = newResource;
        }
      }
    }

    #endregion

    #region Remove Global Resource

    public ICommand RemoveGlobalResourceCommand => _removeGlobalResourceCommand ??= new ActionCommand(RemoveGlobalResource, CanRemoveGlobalResource);

    private void RemoveGlobalResource()
    {
      if (!_schedule.CanRemoveResource(CurrentGlobalResource))
      {
        UserNotificationService.Instance.Error(NASResources.MessageCannotRemoveResource);
        return;
      }

      UserNotificationService.Instance.Question(NASResources.MessageDeleteResource, () =>
      {
        GlobalResources.Remove(CurrentGlobalResource);
        CurrentGlobalResource = null;
      });
    }

    private bool CanRemoveGlobalResource()
    {
      return CurrentGlobalResource != null;
    }

    #endregion

    #region Edit Global Resource

    public ICommand EditGlobalResourceCommand => _editGlobalResourceCommand ??= new ActionCommand(EditGlobalResource, CanEditGlobalResource);

    private void EditGlobalResource()
    {
      using var vm = new ResourceDetailsViewModel(CurrentGlobalResource);
      ViewFactory.Instance.ShowDialog(vm);
    }

    private bool CanEditGlobalResource()
    {
      return CurrentGlobalResource != null;
    }

    #endregion

    #region Add Resource

    public ICommand AddResourceCommand => _addResourceCommand ??= new ActionCommand(AddResource);

    private void AddResource()
    {
      using var vm = new ChooseResourceTypeViewModel();
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

        using var vm2 = new ResourceDetailsViewModel(newResource);
        if (ViewFactory.Instance.ShowDialog(vm2) == true)
        {
          Resources.Add(newResource);
          CurrentResource = newResource;
        }
      };
    }

    #endregion

    #region Remove Resource

    public ICommand RemoveResourceCommand => _removeResourceCommand ??= new ActionCommand(RemoveResource, CanRemoveResource);

    private void RemoveResource()
    {
      if (!_schedule.CanRemoveResource(CurrentResource))
      {
        UserNotificationService.Instance.Error(NASResources.MessageCannotRemoveResource);
        return;
      }

      UserNotificationService.Instance.Question(NASResources.MessageDeleteResource, () =>
      {
        Resources.Remove(CurrentResource);
        CurrentResource = null;
      });
    }

    private bool CanRemoveResource()
    {
      return CurrentResource != null;
    }

    #endregion

    #region Edit Resource

    public ICommand EditResourceCommand => _editResourceCommand ??= new ActionCommand(EditResource, CanEditResource);

    private void EditResource()
    {
      using var vm = new ResourceDetailsViewModel(CurrentResource);
      ViewFactory.Instance.ShowDialog(vm);
    }

    private bool CanEditResource()
    {
      return CurrentResource != null;
    }

    #endregion

    #region Apply

    protected override void OnApply()
    {
      base.OnApply();

      GlobalDataController.Resources.Clear();
      GlobalDataController.Resources.AddRange(GlobalResources);
      GlobalDataController.SaveResources();

      foreach (var removedResource in _schedule.Resources.Except(Resources))
      {
        _schedule.Resources.Remove(removedResource);
      }

      foreach (var addedResource in Resources.Except(_schedule.Resources))
      {
        _schedule.Resources.Add(addedResource);
      }
    }

    #endregion
  }
}
