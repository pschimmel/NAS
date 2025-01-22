using System.Collections.ObjectModel;
using System.Windows.Input;
using ES.Tools.Core.MVVM;
using NAS.Models.Entities;
using NAS.Models.Enums;
using NAS.Resources;
using NAS.ViewModels.Base;
using NAS.ViewModels.Helpers;

namespace NAS.ViewModels
{
  public class FilterDefinitionsViewModel : ViewModelBase
  {
    #region Fields

    private readonly Schedule _schedule;
    private FilterDefinition _currentFilterDefinition;
    public Layout _layout;

    #endregion

    #region Constructor

    public FilterDefinitionsViewModel(Schedule schedule, Layout layout)
      : base()
    {
      _schedule = schedule;
      _layout = layout;
      FilterDefinitions = new ObservableCollection<FilterDefinition>(_layout.FilterDefinitions);
      AddFilterDefinitionCommand = new ActionCommand(AddFilterDefinitionCommandExecute);
      RemoveFilterDefinitionCommand = new ActionCommand(RemoveFilterDefinitionCommandExecute, () => RemoveFilterDefinitionCommandCanExecute);
      RemoveAllFilterDefinitionsCommand = new ActionCommand(RemoveAllFilterDefinitionsCommandExecute, () => RemoveAllFilterDefinitionsCommandCanExecute);
      EditFilterDefinitionCommand = new ActionCommand(EditFilterDefinitionCommandExecute, () => EditFilterDefinitionCommandCanExecute);
    }

    #endregion

    #region Public Members

    public override HelpTopic HelpTopicKey => HelpTopic.Filters;

    public ObservableCollection<FilterDefinition> FilterDefinitions { get; }

    public FilterDefinition CurrentFilterDefinition
    {
      get => _currentFilterDefinition;
      set
      {
        if (_currentFilterDefinition != value)
        {
          _currentFilterDefinition = value;
          OnPropertyChanged(nameof(CurrentFilterDefinition));
        }
      }
    }

    public static List<FilterCombinationType> FilterCombinationTypes { get; } = Enum.GetValues(typeof(FilterCombinationType)).Cast<FilterCombinationType>().ToList();

    #endregion

    #region Add Filter Definition

    public ICommand AddFilterDefinitionCommand { get; }

    private void AddFilterDefinitionCommandExecute()
    {
      var newFilterDefinition = new FilterDefinition(_schedule, ActivityProperty.None);

      using var vm = new FilterDefinitionViewModel(_schedule, newFilterDefinition);
      if (ViewFactory.Instance.ShowDialog(vm) == true && newFilterDefinition.Property != ActivityProperty.None)
      {
        vm.Apply();
        FilterDefinitions.Add(newFilterDefinition);
        CurrentFilterDefinition = newFilterDefinition;
      }
    }

    #endregion

    #region Remove Filter Definition

    public ICommand RemoveFilterDefinitionCommand { get; }

    private void RemoveFilterDefinitionCommandExecute()
    {
      UserNotificationService.Instance.Question(NASResources.MessageDeleteFilter, () =>
      {
        FilterDefinitions.Remove(CurrentFilterDefinition);
        CurrentFilterDefinition = null;
      });
    }

    private bool RemoveFilterDefinitionCommandCanExecute => CurrentFilterDefinition != null;

    #endregion

    #region Remove All Filter Definitions

    public ICommand RemoveAllFilterDefinitionsCommand { get; }

    private void RemoveAllFilterDefinitionsCommandExecute()
    {
      UserNotificationService.Instance.Question(NASResources.MessageDeleteAllFilters, () =>
      {
        FilterDefinitions.Clear();
      });
    }

    private bool RemoveAllFilterDefinitionsCommandCanExecute => FilterDefinitions.Any();

    #endregion

    #region Edit Filter Definition

    public ICommand EditFilterDefinitionCommand { get; }

    private void EditFilterDefinitionCommandExecute()
    {
      using var vm = new FilterDefinitionViewModel(_schedule, CurrentFilterDefinition);
      if (ViewFactory.Instance.ShowDialog(vm) == true)
      {
        vm.Apply();
      }
    }

    private bool EditFilterDefinitionCommandCanExecute => CurrentFilterDefinition != null;

    #endregion

    #region Apply

    public void Apply()
    {
      _layout.RefreshFilterDefinitions(FilterDefinitions);
    }

    #endregion
  }
}
