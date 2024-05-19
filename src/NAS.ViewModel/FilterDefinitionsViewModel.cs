using System.Collections.ObjectModel;
using System.Windows.Input;
using ES.Tools.Core.MVVM;
using NAS.Model.Base;
using NAS.Model.Entities;
using NAS.Model.Enums;
using NAS.Resources;
using NAS.ViewModel.Base;
using NAS.ViewModel.Helpers;

namespace NAS.ViewModel
{
  public class FilterDefinitionsViewModel : ViewModelBase, IApplyable
  {
    #region Fields

    private FilterDefinition _currentFilterDefinition;
    public Layout _layout;

    #endregion

    #region Constructor

    public FilterDefinitionsViewModel(Layout layout)
      : base()
    {
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
      var newFilterDefinition = new FilterDefinition(ActivityProperty.None);

      using var vm = new FilterDefinitionViewModel(newFilterDefinition);
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
      using var vm = new FilterDefinitionViewModel(CurrentFilterDefinition);
      if (ViewFactory.Instance.ShowDialog(vm) == true)
      {
        vm.Apply();
      }
    }

    private bool EditFilterDefinitionCommandCanExecute => CurrentFilterDefinition != null;

    #endregion

    #region IApplyable

    public void Apply()
    {
      _layout.RefreshFilterDefinitions(FilterDefinitions);
    }

    #endregion
  }
}
