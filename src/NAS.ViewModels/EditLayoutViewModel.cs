using System.ComponentModel;
using System.Windows.Input;
using ES.Tools.Core.MVVM;
using NAS.Models.Entities;
using NAS.Models.Enums;
using NAS.Resources;
using NAS.ViewModels.Base;
using NAS.ViewModels.Helpers;

namespace NAS.ViewModels
{
  public class EditLayoutViewModel : ValidatingViewModel
  {
    #region Fields

    private VisibleBaseline currentVisibleBaseline;
    private List<ActivityProperty> activityProperties = null;

    #endregion

    #region Constructor

    public EditLayoutViewModel(Layout layout)
      : base()
    {
      CurrentLayout = layout;
      Schedule = layout.Schedule;
      layout.PropertyChanged += Layout_PropertyChanged;
      EditPrintLayoutCommand = new ActionCommand(EditPrintLayoutCommandExecute, () => EditPrintLayoutCommandCanExecute);
      SelectPertTemplateCommand = new ActionCommand(SelectPertTemplateCommandExecute, () => SelectPertTemplateCommandCanExecute);
      AddVisibleBaselineCommand = new ActionCommand(AddVisibleBaselineCommandExecute, () => AddVisibleBaselineCommandCanExecute);
      RemoveVisibleBaselineCommand = new ActionCommand(RemoveVisibleBaselineCommandExecute, () => RemoveVisibleBaselineCommandCanExecute);
    }

    private void Layout_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == nameof(Layout.LayoutType))
      {
        OnPropertyChanged(nameof(IsPERT));
      }
    }

    #endregion

    #region Properties

    public Layout CurrentLayout { get; private set; }

    public Schedule Schedule { get; private set; }

    public bool IsPERT => CurrentLayout.LayoutType == LayoutType.PERT;

    public List<LayoutType> LayoutTypes => Enum.GetValues(typeof(LayoutType)).OfType<LayoutType>().ToList();

    public List<ActivityProperty> ActivityProperties
    {
      get
      {
        activityProperties ??= new List<ActivityProperty>(Enum.GetValues(typeof(ActivityProperty)).Cast<ActivityProperty>());
        return activityProperties;
      }
    }

    public VisibleBaseline CurrentVisibleBaseline
    {
      get => currentVisibleBaseline;
      set
      {
        if (currentVisibleBaseline != value)
        {
          currentVisibleBaseline = value;
          OnPropertyChanged(nameof(CurrentVisibleBaseline));
        }
      }
    }

    #endregion

    #region Edit Print Layout

    public ICommand EditPrintLayoutCommand { get; }

    private void EditPrintLayoutCommandExecute()
    {
      using var vm = new PrintLayoutViewModel(CurrentLayout);
      if (ViewFactory.Instance.ShowDialog(vm) == true)
      {
        vm.Apply();
      }
    }

    private bool EditPrintLayoutCommandCanExecute => true;

    #endregion

    #region Select PERT Template

    public ICommand SelectPertTemplateCommand { get; }

    private void SelectPertTemplateCommandExecute()
    {
      var vm = new PERTDefinitionsViewModel(Schedule);
      if (ViewFactory.Instance.ShowDialog(vm) == true && vm.SelectedPERTDefinition != null)
      {
        Schedule.CurrentLayout.PERTDefinition = vm.SelectedPERTDefinition;
      }
    }

    private bool SelectPertTemplateCommandCanExecute => CurrentLayout != null;

    #endregion

    #region Add Visible Baseline

    public ICommand AddVisibleBaselineCommand { get; }

    private void AddVisibleBaselineCommandExecute()
    {
      using var vm = new SelectBaselineViewModel(Schedule.Baselines);
      if (ViewFactory.Instance.ShowDialog(vm) == true)
      {
        var newBaseline = new VisibleBaseline(CurrentLayout, vm.SelectedBaseline);
        CurrentLayout.VisibleBaselines.Add(newBaseline);
        CurrentVisibleBaseline = newBaseline;
      }
    }

    private bool AddVisibleBaselineCommandCanExecute => true;

    #endregion

    #region Remove Visible Baseline

    public ICommand RemoveVisibleBaselineCommand { get; }

    private void RemoveVisibleBaselineCommandExecute()
    {
      CurrentLayout.VisibleBaselines.Remove(CurrentVisibleBaseline);
    }

    private bool RemoveVisibleBaselineCommandCanExecute => CurrentVisibleBaseline != null;

    #endregion

    #region Validation

    protected override ValidationResult OnValidating()
    {
      var result = ValidationResult.OK();

      if (string.IsNullOrWhiteSpace(CurrentLayout.Name))
      {
        result = result.Merge(ValidationResult.Error(NASResources.PleaseEnterName));
      }

      if (IsPERT && Schedule.CurrentLayout.PERTDefinition == null)
      {
        result = result.Merge(ValidationResult.Error(NASResources.PleaseSelectTemplate));
      }

      return result;
    }

    #endregion

    #region IDisposable

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);
      CurrentLayout.PropertyChanged -= Layout_PropertyChanged;
    }

    #endregion
  }
}
