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
  public class EditLayoutViewModel : DialogContentViewModel
  {
    #region Fields

    private readonly Schedule _schedule;
    private List<ActivityProperty> _activityProperties = null;
    private ActionCommand _editPrintLayoutCommand;
    private ActionCommand _selectPertTemplateCommand;
    private ActionCommand _addVisibleBaselineCommand;
    private ActionCommand _removeVisibleBaselineCommand;

    #endregion

    #region Constructor

    public EditLayoutViewModel(Schedule schedule, Layout layout)
    {
      _schedule = schedule;
      Layout = layout;

      Name = layout.Name;
      ShowRelationships = layout.ShowRelationships;
      ShowFloat = layout.ShowFloat;

      ActivityStandardColor = layout.ActivityStandardColor;
      ActivityCriticalColor = layout.ActivityCriticalColor;
      ActivityDoneColor = layout.ActivityDoneColor;
      MilestoneStandardColor = layout.MilestoneStandardColor;
      MilestoneCriticalColor = layout.MilestoneCriticalColor;
      MilestoneDoneColor = layout.MilestoneDoneColor;
      DataDateColor = layout.DataDateColor;

      if (layout is PERTLayout pertLayout)
      {
        PERTTemplate = pertLayout.PERTDefinition?.Clone();
      }
      else if (layout is GanttLayout ganttLayout)
      {
        LeftText = ganttLayout.LeftText;
        CenterText = ganttLayout.CenterText;
        RightText = ganttLayout.RightText;
      }
      else
      {
        throw new NotImplementedException("Unknown layout type.");
      }

      VisibleBaselines = new ObservableCollection<VisibleBaseline>();
      foreach (var baseline in layout.VisibleBaselines)
      {
        VisibleBaselines.Add(baseline.Clone());
      }
    }

    #endregion

    #region Overwritten Members

    public override string Title => NASResources.Layout;

    public override string Icon => "Layout";

    public override DialogSize DialogSize => DialogSize.Fixed(400, 450);

    public override HelpTopic HelpTopicKey => HelpTopic.Relationship;

    #endregion

    #region Properties

    internal Layout Layout { get; }

    public LayoutType LayoutType => Layout.LayoutType;

    public bool IsPERT => LayoutType == LayoutType.PERT;

    public string Name { get; set; }

    public bool ShowRelationships { get; set; }

    public bool ShowFloat { get; set; }

    public PERTDefinition PERTTemplate { get; private set; }

    public string ActivityStandardColor { get; set; }

    public string ActivityCriticalColor { get; set; }

    public string ActivityDoneColor { get; set; }

    public string MilestoneStandardColor { get; set; }

    public string MilestoneCriticalColor { get; set; }

    public string MilestoneDoneColor { get; set; }

    public string DataDateColor { get; set; }

    public ActivityProperty LeftText { get; set; }

    public ActivityProperty CenterText { get; set; }

    public ActivityProperty RightText { get; set; }

    public List<ActivityProperty> ActivityProperties => _activityProperties ??= new List<ActivityProperty>(Enum.GetValues(typeof(ActivityProperty)).Cast<ActivityProperty>());

    public ObservableCollection<VisibleBaseline> VisibleBaselines { get; }

    public VisibleBaseline SelectedVisibleBaseline { get; set; }

    #endregion

    #region Edit Print Layout

    public ICommand EditPrintLayoutCommand => _editPrintLayoutCommand ??= new ActionCommand(EditPrintLayout);

    private void EditPrintLayout()
    {
      using var vm = new PrintLayoutViewModel(Layout);
      if (ViewFactory.Instance.ShowDialog(vm) == true)
      {
        vm.Apply();
      }
    }

    #endregion

    #region Select PERT Template

    public ICommand SelectPertTemplateCommand => _selectPertTemplateCommand ??= new ActionCommand(SelectPertTemplate, CanSelectPertTemplate);

    private void SelectPertTemplate()
    {
      var vm = new PERTDefinitionsViewModel(_schedule);
      if (ViewFactory.Instance.ShowDialog(vm) == true && vm.SelectedPERTDefinition != null)
      {
        PERTTemplate = vm.SelectedPERTDefinition;
      }
    }

    private bool CanSelectPertTemplate()
    {
      return IsPERT;
    }

    #endregion

    #region Add Visible Baseline

    public ICommand AddVisibleBaselineCommand => _addVisibleBaselineCommand ??= new ActionCommand(AddVisibleBaseline);

    private void AddVisibleBaseline()
    {
      using var vm = new SelectBaselineViewModel(_schedule.Baselines);
      if (ViewFactory.Instance.ShowDialog(vm) == true && vm.SelectedBaseline != null)
      {
        var newBaseline = new VisibleBaseline(vm.SelectedBaseline);
        VisibleBaselines.Add(newBaseline);
        SelectedVisibleBaseline = newBaseline;
      }
    }

    #endregion

    #region Remove Visible Baseline

    public ICommand RemoveVisibleBaselineCommand => _removeVisibleBaselineCommand ??= new ActionCommand(RemoveVisibleBaseline, CanRemoveVisibleBaseline);

    private void RemoveVisibleBaseline()
    {
      VisibleBaselines.Remove(SelectedVisibleBaseline);
    }

    private bool CanRemoveVisibleBaseline()
    {
      return SelectedVisibleBaseline != null;
    }

    #endregion

    #region Validation

    protected override ValidationResult OnValidating()
    {
      var result = ValidationResult.OK();

      if (string.IsNullOrWhiteSpace(Name))
      {
        result = result.Merge(ValidationResult.Error(NASResources.PleaseEnterName));
      }

      if (IsPERT && PERTTemplate == null)
      {
        result = result.Merge(ValidationResult.Error(NASResources.PleaseSelectTemplate));
      }

      return result;
    }

    #endregion

    #region Apply

    protected override void OnApply()
    {
      base.OnApply();
      Layout.Name = Name;
      Layout.ShowRelationships = ShowRelationships;
      Layout.ShowFloat = ShowFloat;

      Layout.ActivityStandardColor = ActivityStandardColor;
      Layout.ActivityCriticalColor = ActivityCriticalColor;
      Layout.ActivityDoneColor = ActivityDoneColor;
      Layout.MilestoneStandardColor = MilestoneStandardColor;
      Layout.MilestoneCriticalColor = MilestoneCriticalColor;
      Layout.MilestoneDoneColor = MilestoneDoneColor;
      Layout.DataDateColor = DataDateColor;

      if (Layout is PERTLayout pertLayout)
      {
        pertLayout.PERTDefinition = PERTTemplate;
      }
      else if (Layout is GanttLayout ganttLayout)
      {
        ganttLayout.LeftText = LeftText;
        ganttLayout.CenterText = CenterText;
        ganttLayout.RightText = RightText;
      }
      else
      {
        throw new NotImplementedException("Unknown layout type.");
      }

      Layout.VisibleBaselines.Clear();

      foreach (var baseline in VisibleBaselines)
      {
        Layout.VisibleBaselines.Add(baseline);
      }
    }

    #endregion
  }
}
