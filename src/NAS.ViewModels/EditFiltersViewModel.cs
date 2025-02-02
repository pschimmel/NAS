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
  public class EditFiltersViewModel : DialogContentViewModel
  {
    #region Fields

    private readonly Schedule _schedule;
    private FilterDefinitionViewModel _selectedFilterDefinition;
    public Layout _layout;
    private ActionCommand _addFilterDefinitionCommand;
    private ActionCommand _removeFilterDefinitionCommand;
    private ActionCommand _removeAllFilterDefinitionsCommand;
    private ActionCommand _editFilterDefinitionCommand;


    #endregion

    #region Constructor

    public EditFiltersViewModel(Schedule schedule, Layout layout)
      : base()
    {
      _schedule = schedule;
      _layout = layout;
      FilterDefinitions = new ObservableCollection<FilterDefinitionViewModel>(_layout.FilterDefinitions.Select(x => new FilterDefinitionViewModel(schedule, x.Clone())));
      FilterCombination = _layout.FilterCombination;
    }

    #endregion

    #region Overwritten Members

    public override string Title => NASResources.Filter;

    public override string Icon => "Filter";

    public override DialogSize DialogSize => DialogSize.Fixed(350, 300);

    public override HelpTopic HelpTopicKey => HelpTopic.Filters;

    #endregion

    #region Properties

    public ObservableCollection<FilterDefinitionViewModel> FilterDefinitions { get; }

    public FilterDefinitionViewModel SelectedFilterDefinition
    {
      get => _selectedFilterDefinition;
      set
      {
        if (_selectedFilterDefinition != value)
        {
          _selectedFilterDefinition = value;
          OnPropertyChanged(nameof(SelectedFilterDefinition));
        }
      }
    }

    public static List<FilterCombinationType> FilterCombinationTypes { get; } = Enum.GetValues(typeof(FilterCombinationType)).Cast<FilterCombinationType>().ToList();

    public FilterCombinationType FilterCombination { get; set; }

    #endregion

    #region Add Filter Definition

    public ICommand AddFilterDefinitionCommand => _addFilterDefinitionCommand ??= new ActionCommand(AddFilterDefinition);

    private void AddFilterDefinition()
    {
      var newFilterDefinition = new FilterDefinition(ActivityProperty.None);
      using var vm = new SelectFilterDefinitionViewModel(_schedule, newFilterDefinition);
      if (ViewFactory.Instance.ShowDialog(vm) == true && newFilterDefinition.Property != ActivityProperty.None)
      {
        var newVM = new FilterDefinitionViewModel(_schedule, newFilterDefinition);
        FilterDefinitions.Add(newVM);
        SelectedFilterDefinition = newVM;
      }
    }

    #endregion

    #region Remove Filter Definition

    public ICommand RemoveFilterDefinitionCommand => _removeFilterDefinitionCommand ??= new ActionCommand(RemoveFilterDefinition, CanRemoveFilterDefinition);

    private void RemoveFilterDefinition()
    {
      UserNotificationService.Instance.Question(NASResources.MessageDeleteFilter, () =>
      {
        FilterDefinitions.Remove(SelectedFilterDefinition);
        SelectedFilterDefinition.Dispose();
        SelectedFilterDefinition = null;
      });
    }

    private bool CanRemoveFilterDefinition()
    {
      return SelectedFilterDefinition != null;
    }

    #endregion

    #region Remove All Filter Definitions

    public ICommand RemoveAllFilterDefinitionsCommand => _removeAllFilterDefinitionsCommand ??= new ActionCommand(RemoveAllFilterDefinitions, CanRemoveAllFilterDefinitions);

    private void RemoveAllFilterDefinitions()
    {
      UserNotificationService.Instance.Question(NASResources.MessageDeleteAllFilters, FilterDefinitions.Clear);
    }

    private bool CanRemoveAllFilterDefinitions()
    {
      return FilterDefinitions.Any();
    }

    #endregion

    #region Edit Filter Definition

    public ICommand EditFilterDefinitionCommand => _editFilterDefinitionCommand ??= new ActionCommand(EditFilterDefinition, CanEditFilterDefinition);

    private void EditFilterDefinition()
    {
      using var vm = new SelectFilterDefinitionViewModel(_schedule, SelectedFilterDefinition.FilterDefinition);
      if (ViewFactory.Instance.ShowDialog(vm) == true)
      {
        vm.Apply();
      }
    }

    private bool CanEditFilterDefinition()
    {
      return SelectedFilterDefinition != null;
    }

    #endregion

    #region Apply

    protected override void OnApply()
    {
      _layout.FilterDefinitions.Clear();

      foreach (var filterDefinitionVM in FilterDefinitions)
      {
        _layout.FilterDefinitions.Add(filterDefinitionVM.FilterDefinition);
      }
    }

    #endregion

    #region IDisposable

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);
      if (disposing)
      {
        foreach (var filterDefinition in FilterDefinitions)
        {
          filterDefinition.Dispose();
        }
      }
    }

    #endregion
  }
}
