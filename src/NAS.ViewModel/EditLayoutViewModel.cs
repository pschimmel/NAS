using System.ComponentModel;
using System.Windows.Input;
using ES.Tools.Core.MVVM;
using NAS.Model.Entities;
using NAS.Model.Enums;
using NAS.Resources;
using NAS.ViewModel.Base;
using NAS.ViewModel.Helpers;

namespace NAS.ViewModel
{
  public class EditLayoutViewModel : ViewModelBase, IValidatable
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

    #region Public Properties

    public Layout CurrentLayout { get; private set; }

    public Schedule Schedule { get; private set; }

    public bool IsPERT => CurrentLayout.LayoutType == LayoutType.PERT;

    public List<LayoutType> LayoutTypes => Enum.GetValues(typeof(LayoutType)).OfType<LayoutType>().ToList();

    public List<ActivityProperty> ActivityProperties
    {
      get
      {
        if (activityProperties == null)
        {
          activityProperties = new List<ActivityProperty>();
          foreach (ActivityProperty item in Enum.GetValues(typeof(ActivityProperty)))
          {
            activityProperties.Add(item);
          }
        }
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
      _ = CurrentLayout.VisibleBaselines.Remove(CurrentVisibleBaseline);
    }

    private bool RemoveVisibleBaselineCommandCanExecute => CurrentVisibleBaseline != null;

    #endregion

    #region Validation

    public string ErrorMessage { get; set; } = null;

    public bool HasErrors => !string.IsNullOrWhiteSpace(ErrorMessage);

    private void AddError(string message)
    {
      if (!string.IsNullOrWhiteSpace(message))
      {
        if (!string.IsNullOrWhiteSpace(ErrorMessage))
        {
          ErrorMessage += Environment.NewLine;
        }

        ErrorMessage += message;
        OnPropertyChanged(nameof(ErrorMessage));
        OnPropertyChanged(nameof(HasErrors));
      }
    }

    public bool Validate()
    {
      ErrorMessage = null;
      if (string.IsNullOrWhiteSpace(CurrentLayout.Name))
      {
        AddError(NASResources.PleaseEnterName);
      }

      if (IsPERT && Schedule.CurrentLayout.PERTDefinition == null)
      {
        AddError(NASResources.PleaseSelectTemplate);
      }

      return string.IsNullOrWhiteSpace(ErrorMessage);
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
