using System.Windows.Documents;
using System.Windows.Input;
using ES.Tools.Core.MVVM;
using NAS.Models.Entities;
using NAS.Resources;
using NAS.ViewModels.Base;
using NAS.ViewModels.Helpers;

namespace NAS.ViewModels
{
  public class CompareResultsViewModel : DialogContentViewModel
  {
    #region Events

    public event EventHandler OnPrintRequested;

    #endregion

    #region Fields

    private readonly Schedule _schedule1;
    private readonly Schedule _schedule2;
    private readonly string _headline;
    private readonly List<string> _strings;
    private ActionCommand _printCommand;
    private ActionCommand _showGraphCommand;

    #endregion

    #region Constructor

    public CompareResultsViewModel(ComparisonData data)
    {
      ComparisonDocument = new FlowDocument
      {
        IsColumnWidthFlexible = false
      };
      _schedule1 = data.Schedule1;
      _schedule2 = data.Schedule2;
      _headline = data.Headline;
      _strings = data.Text ?? [];

      ShowComparisonResults();
    }

    #endregion

    #region Overwritten Members

    public override string Title => NASResources.Comparison;

    public override string Icon => "Compare";

    public override DialogSize DialogSize => DialogSize.Fixed(600, 350);

    public override HelpTopic HelpTopicKey => HelpTopic.Compare;

    public override IEnumerable<IButtonViewModel> Buttons
    {
      get
      {
        return new List<IButtonViewModel>
        {
          ButtonViewModel.CreateButton(NASResources.Print, Print, CanPrint),
          ButtonViewModel.CreateButton(NASResources.ShowChart, ShowGraph, CanShowGraph),
          ButtonViewModel.CreateOKButton()
        };
      }
    }

    #endregion

    #region Properties

    public FlowDocument ComparisonDocument { get; private set; }

    #endregion

    #region Print

    public ICommand PrintCommand => _printCommand ??= new ActionCommand(Print, CanPrint);

    private void Print()
    {
      OnPrintRequested?.Invoke(this, EventArgs.Empty);
    }

    private bool CanPrint()
    {
      return _schedule1 != null && _schedule2 != null;
    }

    #endregion

    #region Show Graph

    public ICommand ShowGraphCommand => _showGraphCommand ??= new ActionCommand(ShowGraph, CanShowGraph);

    private void ShowGraph()
    {
      using var vm = new ShowSchedulesComparisonViewModel(_schedule1, _schedule2);
      ViewFactory.Instance.ShowDialog(vm);
    }

    private bool CanShowGraph()
    {
      return _schedule1 != null && _schedule2 != null;
    }

    #endregion

    #region Private Members

    private void ShowComparisonResults()
    {
      ComparisonDocument.Blocks.Add(new Paragraph(new Bold(new Run(_headline))) { FontSize = 16 });
      foreach (string s in _strings)
      {
        ComparisonDocument.Blocks.Add(new Paragraph(new Run(s)) { FontSize = 12 });
      }

      foreach (var activity1 in _schedule1.Activities)
      {
        string s = "";
        var activity2 = _schedule2.Activities.ToList().Find(x => x.Number == activity1.Number);
        if (activity2 != null)
        {
          if (activity1.Fragnet != null && activity2.Fragnet != null && activity1.Fragnet.IsVisible && !activity2.Fragnet.IsVisible)
          {
            s += string.Format(NASResources.ActivityDoesNotExist, 2) + " ";
          }

          if (activity1.Fragnet != null && activity2.Fragnet != null && activity2.Fragnet.IsVisible && !activity1.Fragnet.IsVisible)
          {
            s += string.Format(NASResources.ActivityDoesNotExist, 1) + " ";
          }

          if (activity2.RetardedDuration > activity1.RetardedDuration)
          {
            s += string.Format(NASResources.DurationIncreased, activity2.RetardedDuration - activity1.RetardedDuration) + " ";
          }

          if (activity2.RetardedDuration < activity1.RetardedDuration)
          {
            s += string.Format(NASResources.DurationDecreased, activity1.RetardedDuration - activity2.RetardedDuration) + " ";
          }

          if (activity2.IsStarted && !activity1.IsStarted && activity2.StartDate != activity1.StartDate)
          {
            if (activity2.StartDate > activity1.StartDate)
            {
              s += string.Format(NASResources.ActivityStartedLater, activity2.Calendar.GetWorkDays(activity1.StartDate, activity2.StartDate, true)) + " ";
            }
            else
            {
              s += string.Format(NASResources.ActivityStartedEarlier, activity2.Calendar.GetWorkDays(activity2.StartDate, activity1.StartDate, true)) + " ";
            }
          }
          if (activity2.IsFinished && !activity1.IsFinished && activity1.StartDate - activity1.StartDate != activity2.FinishDate - activity2.StartDate)
          {
            if (activity1.FinishDate - activity1.StartDate > activity2.FinishDate - activity2.StartDate)
            {
              s += string.Format(NASResources.ActivityFinishedEarlier, activity1.Calendar.GetWorkDays(activity1.StartDate, activity1.FinishDate, true) - activity2.Calendar.GetWorkDays(activity2.StartDate, activity2.FinishDate, true)) + " ";
            }
            else
            {
              s += string.Format(NASResources.ActivityFinishedLater, activity2.Calendar.GetWorkDays(activity2.StartDate, activity2.FinishDate, true) - activity1.Calendar.GetWorkDays(activity1.StartDate, activity1.FinishDate, true)) + " ";
            }
          }
          if (activity2.EarlyStartDate != activity1.EarlyStartDate)
          {
            s += string.Format(NASResources.EarlyStartDateChanged, activity1.EarlyStartDate.ToShortDateString(), activity2.EarlyStartDate.ToShortDateString()) + " ";
          }

          if (activity2.TotalFloat > activity1.TotalFloat)
          {
            s += string.Format(NASResources.TotalFloatIncreased, activity2.TotalFloat - activity1.TotalFloat) + " ";
          }

          if (activity2.TotalFloat < activity1.TotalFloat)
          {
            s += string.Format(NASResources.TotalFloatDecreased, activity1.TotalFloat - activity2.TotalFloat) + " ";
            if (activity2.TotalFloat < 0)
            {
              s += string.Format(NASResources.ActivityHasDelay, -activity2.TotalFloat) + " ";
            }
          }
          if (activity2.TotalPlannedCosts > activity1.TotalPlannedCosts)
          {
            s += string.Format(NASResources.PlannedCostsIncreased, activity2.TotalPlannedCosts - activity1.TotalPlannedCosts) + " ";
          }

          if (activity2.TotalPlannedCosts < activity1.TotalPlannedCosts)
          {
            s += string.Format(NASResources.PlannedCostsDecreased, activity1.TotalPlannedCosts - activity2.TotalPlannedCosts) + " ";
          }

          if (activity2.TotalActualCosts > activity1.TotalActualCosts)
          {
            s += string.Format(NASResources.ActualCostsIncreased, activity2.TotalActualCosts - activity1.TotalActualCosts) + " ";
          }

          if (activity2.TotalActualCosts < activity1.TotalActualCosts)
          {
            s += string.Format(NASResources.ActualCostsDecreased, activity1.TotalActualCosts - activity2.TotalActualCosts) + " ";
          }

          if (activity2.TotalActualCosts > activity1.TotalBudget)
          {
            s += string.Format(NASResources.CostsHigherAsBudget, activity2.TotalActualCosts - activity1.TotalBudget) + " ";
          }
        }
        if (!string.IsNullOrWhiteSpace(s))
        {
          AddTextWithLabel(activity1.Number + " (" + activity1.Name + "): ", s);
        }
      }
      var e1 = _schedule1.LastDay;
      var e2 = _schedule2.LastDay;
      if (e1 == e2)
      {
        string s = NASResources.ProjectEndUnchanged;
        if (_schedule2.EndDate.HasValue)
        {
          s += " (" + NASResources.FixedEndDate + ").";
        }
        else
        {
          s += ".";
        }

        ComparisonDocument.Blocks.Add(new Paragraph(new Bold(new Run(s))) { FontSize = 12 });
      }
      else
      {
        ComparisonDocument.Blocks.Add(new Paragraph(new Run(string.Format(NASResources.ProjectEndChanged, e1.ToShortDateString(), e2.ToShortDateString()))) { FontSize = 12 });
      }

      if (_schedule2.TotalPlannedCosts > _schedule1.TotalPlannedCosts)
      {
        ComparisonDocument.Blocks.Add(new Paragraph(new Run(string.Format(NASResources.PlannedCostsIncreased, _schedule2.TotalPlannedCosts - _schedule1.TotalPlannedCosts))) { FontSize = 12 });
      }

      if (_schedule2.TotalPlannedCosts < _schedule1.TotalPlannedCosts)
      {
        ComparisonDocument.Blocks.Add(new Paragraph(new Run(string.Format(NASResources.PlannedCostsDecreased, _schedule1.TotalPlannedCosts - _schedule2.TotalPlannedCosts))) { FontSize = 12 });
      }

      if (_schedule2.TotalBudget > _schedule1.TotalBudget)
      {
        ComparisonDocument.Blocks.Add(new Paragraph(new Run(string.Format(NASResources.ProjectBudgetIncreased, _schedule2.TotalBudget - _schedule1.TotalBudget))) { FontSize = 12 });
      }

      if (_schedule2.TotalBudget < _schedule1.TotalBudget)
      {
        ComparisonDocument.Blocks.Add(new Paragraph(new Run(string.Format(NASResources.ProjectBudgetDecreased, _schedule1.TotalBudget - _schedule2.TotalBudget))) { FontSize = 12 });
      }

      if (_schedule2.TotalActualCosts > _schedule1.TotalActualCosts)
      {
        ComparisonDocument.Blocks.Add(new Paragraph(new Run(string.Format(NASResources.ActualCostsIncreased, _schedule2.TotalActualCosts - _schedule1.TotalActualCosts))) { FontSize = 12 });
      }

      if (_schedule2.TotalActualCosts < _schedule1.TotalActualCosts)
      {
        ComparisonDocument.Blocks.Add(new Paragraph(new Run(string.Format(NASResources.ActualCostsDecreased, _schedule1.TotalActualCosts - _schedule2.TotalActualCosts))) { FontSize = 12 });
      }
    }

    private void AddTextWithLabel(string label, string text)
    {
      Inline i1 = new Bold(new Run(label));
      Inline i2 = new Run(text);
      var p = new Paragraph() { FontSize = 12 };
      p.Inlines.Add(i1);
      p.Inlines.Add(i2);
      ComparisonDocument.Blocks.Add(p);
    }

    #endregion
  }
}
