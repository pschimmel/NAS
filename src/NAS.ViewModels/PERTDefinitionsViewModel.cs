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
  public class PERTDefinitionsViewModel : ViewModelBase
  {
    #region Fields

    private readonly Schedule schedule;
    private int tabIndex;
    private PERTDefinition currentGlobalPertDefinition;
    private PERTDefinition currentProjectPertDefinition;

    #endregion

    #region Constructor

    public PERTDefinitionsViewModel(Schedule schedule)
      : base()
    {
      this.schedule = schedule;
      GlobalPERTDefinitions = new ObservableCollection<PERTDefinition>(GlobalDataController.PERTDefinitions);
      ProjectPERTDefinitions = new ObservableCollection<PERTDefinition>(schedule.PERTDefinitions);
      AddGlobalPERTDefinitionCommand = new ActionCommand(AddGlobalPERTDefinitionCommandExecute);
      RemoveGlobalPERTDefinitionCommand = new ActionCommand(RemoveGlobalPERTDefinitionCommandExecute, () => RemoveGlobalPERTDefinitionCommandCanExecute);
      CopyGlobalPERTDefinitionCommand = new ActionCommand(CopyGlobalPERTDefinitionCommandExecute, () => CopyGlobalPERTDefinitionCommandCanExecute);
      EditGlobalPERTDefinitionCommand = new ActionCommand(EditGlobalPERTDefinitionCommandExecute, () => EditGlobalPERTDefinitionCommandCanExecute);
      AddProjectPERTDefinitionCommand = new ActionCommand(AddProjectPERTDefinitionCommandExecute);
      RemoveProjectPERTDefinitionCommand = new ActionCommand(RemoveProjectPERTDefinitionCommandExecute, () => RemoveProjectPERTDefinitionCommandCanExecute);
      CopyProjectPERTDefinitionCommand = new ActionCommand(CopyProjectPERTDefinitionCommandExecute, () => CopyProjectPERTDefinitionCommandCanExecute);
      EditProjectPERTDefinitionCommand = new ActionCommand(EditProjectPERTDefinitionCommandExecute, () => EditProjectPERTDefinitionCommandCanExecute);
    }

    #endregion

    #region Public Members

    public ObservableCollection<PERTDefinition> GlobalPERTDefinitions { get; }

    public PERTDefinition CurrentGlobalPERTDefinition
    {
      get => currentGlobalPertDefinition;
      set
      {
        if (currentGlobalPertDefinition != value)
        {
          currentGlobalPertDefinition = value;
          if (currentGlobalPertDefinition != null)
          {
            currentProjectPertDefinition = null;
          }

          OnPropertyChanged(nameof(CurrentGlobalPERTDefinition));
          OnPropertyChanged(nameof(CurrentProjectPERTDefinition));
          OnPropertyChanged(nameof(SelectedPERTDefinition));
        }
      }
    }

    public ObservableCollection<PERTDefinition> ProjectPERTDefinitions { get; }

    public PERTDefinition CurrentProjectPERTDefinition
    {
      get => currentProjectPertDefinition;
      set
      {
        if (currentProjectPertDefinition != value)
        {
          currentProjectPertDefinition = value;
          if (currentProjectPertDefinition != null)
          {
            currentGlobalPertDefinition = null;
          }

          OnPropertyChanged(nameof(CurrentGlobalPERTDefinition));
          OnPropertyChanged(nameof(CurrentProjectPERTDefinition));
          OnPropertyChanged(nameof(SelectedPERTDefinition));
        }
      }
    }

    public int TabIndex
    {
      get => tabIndex;
      set
      {
        if (tabIndex != value)
        {
          tabIndex = value;
          OnPropertyChanged(nameof(tabIndex));
        }
      }
    }

    public PERTDefinition SelectedPERTDefinition
    {
      get => CurrentProjectPERTDefinition ?? CurrentGlobalPERTDefinition ?? null;
      set
      {
        if (value == null)
        {
          CurrentProjectPERTDefinition = null;
          CurrentGlobalPERTDefinition = null;
        }
        else
        {
          if (value.Schedule == null)
          {
            TabIndex = 0;
            CurrentGlobalPERTDefinition = value;
          }
          else
          {
            TabIndex = 1;
            CurrentProjectPERTDefinition = value;
          }
        }
      }
    }

    #endregion

    #region Add Global Pert Template

    public ICommand AddGlobalPERTDefinitionCommand { get; }

    private void AddGlobalPERTDefinitionCommandExecute()
    {
      var definition = new PERTDefinition();
      var vm = new PERTDefinitionViewModel(definition);
      if (ViewFactory.Instance.ShowDialog(vm) == true)
      {
        GlobalPERTDefinitions.Add(definition);
        CurrentGlobalPERTDefinition = definition;
      }
    }

    #endregion

    #region Remove Global Pert Template

    public ICommand RemoveGlobalPERTDefinitionCommand { get; }

    private void RemoveGlobalPERTDefinitionCommandExecute()
    {
      InstantHelpManager.Instance.SetHelpTopic(HelpTopic.Pert);
      UserNotificationService.Instance.Question(NASResources.MessageDeleteTemplate, () =>
      {
        GlobalPERTDefinitions.Remove(CurrentGlobalPERTDefinition);
      });
    }

    private bool RemoveGlobalPERTDefinitionCommandCanExecute => CurrentGlobalPERTDefinition != null;

    #endregion

    #region Edit Global Pert Template

    public ICommand EditGlobalPERTDefinitionCommand { get; }

    private void EditGlobalPERTDefinitionCommandExecute()
    {
      using var vm = new PERTDefinitionViewModel(CurrentGlobalPERTDefinition);
      ViewFactory.Instance.ShowDialog(vm);
    }

    private bool EditGlobalPERTDefinitionCommandCanExecute => CurrentGlobalPERTDefinition != null;

    #endregion

    #region Copy Global Pert Template

    public ICommand CopyGlobalPERTDefinitionCommand { get; }

    private void CopyGlobalPERTDefinitionCommandExecute()
    {
      var definition = new PERTDefinition(CurrentGlobalPERTDefinition);
      GlobalPERTDefinitions.Add(definition);
      CurrentGlobalPERTDefinition = definition;
    }

    private bool CopyGlobalPERTDefinitionCommandCanExecute => CurrentGlobalPERTDefinition != null;

    #endregion

    #region Add Project Pert Template

    public ICommand AddProjectPERTDefinitionCommand { get; }

    private void AddProjectPERTDefinitionCommandExecute()
    {
      InstantHelpManager.Instance.SetHelpTopic(HelpTopic.Pert);
      var definition = new PERTDefinition();
      var vm = new PERTDefinitionViewModel(definition);
      if (ViewFactory.Instance.ShowDialog(vm) == true)
      {
        ProjectPERTDefinitions.Add(definition);
        CurrentProjectPERTDefinition = definition;
      }
    }

    #endregion

    #region Remove Project Pert Template

    public ICommand RemoveProjectPERTDefinitionCommand { get; }

    private void RemoveProjectPERTDefinitionCommandExecute()
    {
      InstantHelpManager.Instance.SetHelpTopic(HelpTopic.Pert);
      UserNotificationService.Instance.Question(NASResources.MessageDeleteTemplate, () =>
      {
        ProjectPERTDefinitions.Remove(CurrentProjectPERTDefinition);
      });
    }

    private bool RemoveProjectPERTDefinitionCommandCanExecute => CurrentProjectPERTDefinition != null;

    #endregion

    #region Edit Project Pert Template

    public ICommand EditProjectPERTDefinitionCommand { get; }

    private void EditProjectPERTDefinitionCommandExecute()
    {
      var vm = new PERTDefinitionViewModel(CurrentProjectPERTDefinition);
      ViewFactory.Instance.ShowDialog(vm);
    }

    private bool EditProjectPERTDefinitionCommandCanExecute => CurrentProjectPERTDefinition != null;

    #endregion

    #region Copy Project Pert Template

    public ICommand CopyProjectPERTDefinitionCommand { get; }

    private void CopyProjectPERTDefinitionCommandExecute()
    {
      var definition = new PERTDefinition(CurrentProjectPERTDefinition);
      ProjectPERTDefinitions.Add(definition);
      CurrentProjectPERTDefinition = definition;
    }

    private bool CopyProjectPERTDefinitionCommandCanExecute => CurrentProjectPERTDefinition != null;

    #endregion

    #region Apply

    public void Apply()
    {
      GlobalDataController.PERTDefinitions.Clear();
      GlobalDataController.PERTDefinitions.AddRange(GlobalPERTDefinitions);
      GlobalDataController.SavePERTDefinitions();
      schedule.PERTDefinitions.Replace(ProjectPERTDefinitions);
    }


    #endregion
  }
}
