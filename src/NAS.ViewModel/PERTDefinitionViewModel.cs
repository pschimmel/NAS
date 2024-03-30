using System.Collections.ObjectModel;
using System.Windows.Input;
using ES.Tools.Core.MVVM;
using NAS.Model.Entities;
using NAS.Resources;
using NAS.ViewModel.Base;
using NAS.ViewModel.Helpers;

namespace NAS.ViewModel
{
  public class PERTDefinitionViewModel : ValidatingViewModel, IApplyable
  {
    #region Fields

    private readonly PERTDefinition _definition;
    private PERTDataItem _currentItem;
    private ColumnDefinition _currentColumnDefinition;
    private RowDefinition _currentRowDefinition;

    #endregion

    #region Constructor

    public PERTDefinitionViewModel(PERTDefinition definition)
      : base()
    {
      _definition = definition;
      Items = new ObservableCollection<PERTDataItem>(definition.Items);
      Rows = new ObservableCollection<RowDefinition>(definition.RowDefinitions);
      Columns = new ObservableCollection<ColumnDefinition>(definition.ColumnDefinitions);
      AddPertItemCommand = new ActionCommand(AddPertItemCommandExecute);
      RemovePertItemCommand = new ActionCommand(RemovePertItemCommandExecute, () => RemovePertItemCommandCanExecute);
      AddColumnDefinitionCommand = new ActionCommand(AddColumnDefinitionCommandExecute);
      RemoveColumnDefinitionCommand = new ActionCommand(RemoveColumnDefinitionCommandExecute, () => RemoveColumnDefinitionCommandCanExecute);
      EditColumnDefinitionCommand = new ActionCommand(EditColumnDefinitionCommandExecute, () => EditColumnDefinitionCommandCanExecute);
      MoveColumnDefinitionUpCommand = new ActionCommand(MoveColumnDefinitionUpCommandExecute, () => MoveColumnDefinitionUpCommandCanExecute);
      MoveColumnDefinitionDownCommand = new ActionCommand(MoveColumnDefinitionDownCommandExecute, () => MoveColumnDefinitionDownCommandCanExecute);
      AddRowDefinitionCommand = new ActionCommand(AddRowDefinitionCommandExecute);
      RemoveRowDefinitionCommand = new ActionCommand(RemoveRowDefinitionCommandExecute, () => RemoveRowDefinitionCommandCanExecute);
      EditRowDefinitionCommand = new ActionCommand(EditRowDefinitionCommandExecute, () => EditRowDefinitionCommandCanExecute);
      MoveRowDefinitionUpCommand = new ActionCommand(MoveRowDefinitionUpCommandExecute, () => MoveRowDefinitionUpCommandCanExecute);
      MoveRowDefinitionDownCommand = new ActionCommand(MoveRowDefinitionDownCommandExecute, () => MoveRowDefinitionDownCommandCanExecute);
    }

    #endregion

    #region Public Properties

    public PERTDefinition Definition => _definition;

    public override HelpTopic HelpTopicKey => HelpTopic.Pert;

    public ObservableCollection<RowDefinition> Rows { get; }

    public ObservableCollection<ColumnDefinition> Columns { get; }

    public ObservableCollection<PERTDataItem> Items { get; }

    public PERTDataItem CurrentItem
    {
      get => _currentItem;
      set
      {
        if (_currentItem != value)
        {
          _currentItem = value;
          OnPropertyChanged(nameof(CurrentItem));
        }
      }
    }

    /// <summary>
    /// Gets or sets the current row dbDefinition.
    /// </summary>
    public RowDefinition CurrentRowDefinition
    {
      get => _currentRowDefinition;
      set
      {
        if (_currentRowDefinition != value)
        {
          _currentRowDefinition = value;
          OnPropertyChanged(nameof(CurrentRowDefinition));
        }
      }
    }

    /// <summary>
    /// Gets or sets the current column dbDefinition.
    /// </summary>
    public ColumnDefinition CurrentColumnDefinition
    {
      get => _currentColumnDefinition;
      set
      {
        if (_currentColumnDefinition != value)
        {
          _currentColumnDefinition = value;
          OnPropertyChanged(nameof(CurrentColumnDefinition));
        }
      }
    }

    #endregion

    #region Add Pert Item

    public ICommand AddPertItemCommand { get; }

    private void AddPertItemCommandExecute()
    {
      using var vm = new SelectActivityPropertyViewModel();
      if (ViewFactory.Instance.ShowDialog(vm) == true)
      {
        var newItem = new PERTDataItem(vm.SelectedActivityProperty);
        Items.Add(newItem);
        CurrentItem = newItem;
      }
    }

    #endregion

    #region Remove Pert Item

    public ICommand RemovePertItemCommand { get; }

    private void RemovePertItemCommandExecute()
    {
      _ = Items.Remove(CurrentItem);
      CurrentItem = null;
    }

    private bool RemovePertItemCommandCanExecute => CurrentItem != null;

    #endregion

    #region Add Column Definition

    public ICommand AddColumnDefinitionCommand { get; }

    private void AddColumnDefinitionCommandExecute()
    {
      var newItem = new ColumnDefinition();
      newItem.Sort = Columns.Count == 0 ? 0 : Columns.Max(x => x.Sort) + 1;
      Columns.Add(newItem);
      CurrentColumnDefinition = newItem;
    }

    #endregion

    #region Remove Column Definition

    public ICommand RemoveColumnDefinitionCommand { get; }

    private void RemoveColumnDefinitionCommandExecute()
    {
      _ = Columns.Remove(CurrentColumnDefinition);
      CurrentColumnDefinition = null;
    }

    private bool RemoveColumnDefinitionCommandCanExecute => CurrentColumnDefinition != null;

    #endregion

    #region Edit Column Definition

    public ICommand EditColumnDefinitionCommand { get; }

    private void EditColumnDefinitionCommandExecute()
    {
      var vm = new PERTGridSizeViewModel(CurrentColumnDefinition.Width);
      if (ViewFactory.Instance.ShowDialog(vm) == true)
      {
        CurrentColumnDefinition.Width = vm.AutoSize ? null : vm.Size;
      }
    }

    private bool EditColumnDefinitionCommandCanExecute => CurrentColumnDefinition != null;

    #endregion

    #region Move Column Definition Up

    public ICommand MoveColumnDefinitionUpCommand { get; }

    private void MoveColumnDefinitionUpCommandExecute()
    {
      var item = CurrentColumnDefinition;
      var nextItem = Columns.FirstOrDefault(x => x.Sort == CurrentColumnDefinition.Sort - 1);
      if (nextItem != null)
      {
        nextItem.Sort++;
      }

      item.Sort--;
    }

    private bool MoveColumnDefinitionUpCommandCanExecute => CurrentColumnDefinition != null && CurrentColumnDefinition.Sort > 0;

    #endregion

    #region Move Column Definition Down

    public ICommand MoveColumnDefinitionDownCommand { get; }

    private void MoveColumnDefinitionDownCommandExecute()
    {
      var item = CurrentColumnDefinition;
      var previousItem = Columns.FirstOrDefault(x => x.Sort == CurrentColumnDefinition.Sort + 1);
      if (previousItem != null)
      {
        previousItem.Sort--;
      }

      item.Sort++;
    }

    private bool MoveColumnDefinitionDownCommandCanExecute => CurrentColumnDefinition != null && CurrentColumnDefinition.Sort < Columns.Count - 2;

    #endregion

    #region Add Row Definition

    public ICommand AddRowDefinitionCommand { get; }

    private void AddRowDefinitionCommandExecute()
    {
      var newItem = new RowDefinition();
      newItem.Sort = Rows.Count == 0 ? 0 : Rows.Max(x => x.Sort) + 1;
      Rows.Add(newItem);
      CurrentRowDefinition = newItem;
    }

    #endregion

    #region Remove Row Definition

    public ICommand RemoveRowDefinitionCommand { get; }

    private void RemoveRowDefinitionCommandExecute()
    {
      _ = Rows.Remove(CurrentRowDefinition);
      CurrentColumnDefinition = null;
    }

    private bool RemoveRowDefinitionCommandCanExecute => CurrentRowDefinition != null;

    #endregion

    #region Edit Row Definition

    public ICommand EditRowDefinitionCommand { get; }

    private void EditRowDefinitionCommandExecute()
    {
      var vm = new PERTGridSizeViewModel(CurrentRowDefinition.Height);
      if (ViewFactory.Instance.ShowDialog(vm) == true)
      {
        CurrentRowDefinition.Height = vm.AutoSize ? null : vm.Size;
      }
    }

    private bool EditRowDefinitionCommandCanExecute => CurrentRowDefinition != null;

    #endregion

    #region Move Row Definition Up

    public ICommand MoveRowDefinitionUpCommand { get; }

    private void MoveRowDefinitionUpCommandExecute()
    {
      var item = CurrentRowDefinition;
      var nextItem = Rows.FirstOrDefault(x => x.Sort == CurrentRowDefinition.Sort - 1);
      if (nextItem != null)
      {
        nextItem.Sort++;
      }

      item.Sort--;
    }

    private bool MoveRowDefinitionUpCommandCanExecute => CurrentRowDefinition != null && CurrentRowDefinition.Sort > 0;

    #endregion

    #region Move Row Definition Down

    public ICommand MoveRowDefinitionDownCommand { get; }

    private void MoveRowDefinitionDownCommandExecute()
    {
      var item = CurrentRowDefinition;
      var previousItem = Rows.FirstOrDefault(x => x.Sort == CurrentRowDefinition.Sort + 1);
      if (previousItem != null)
      {
        previousItem.Sort--;
      }

      item.Sort++;
    }

    private bool MoveRowDefinitionDownCommandCanExecute => CurrentRowDefinition != null && CurrentRowDefinition.Sort < Rows.Count - 2;

    #endregion

    #region Validation

    protected override ValidationResult OnValidating()
    {
      return string.IsNullOrWhiteSpace(_definition.Name)
             ? ValidationResult.Error(NASResources.PleaseEnterName)
             : ValidationResult.OK();
    }

    #endregion

    #region Apply

    public void Apply()
    {
      if (Validate().IsOK)
      {
        _definition.RefreshData(Rows, Columns, Items);
      }
    }

    #endregion
  }
}

