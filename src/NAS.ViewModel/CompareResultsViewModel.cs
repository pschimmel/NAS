using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;
using System.Windows.Input;
using ES.Tools.Core.MVVM;
using NAS.ViewModel.Helpers;
using NAS.Model.Entities;
using NAS.Resources;
using NAS.ViewModel.Base;

namespace NAS.ViewModel
{
  public class CompareResultsViewModel : ViewModelBase
  {
    #region Fields

    private Schedule p1, p2;
    private readonly string headline;
    private readonly List<string> strings;

    #endregion

    #region Constructor

    public CompareResultsViewModel(ComparisonData data)
    {
      ComparisonDocument = new FlowDocument
      {
        IsColumnWidthFlexible = false
      };
      p1 = data.Schedule1;
      p2 = data.Schedule2;
      headline = data.Headline;
      strings = data.Text ?? new List<string>();

      ShowComparisonResults();
      PrintCommand = new ActionCommand(param => PrintCommandExecute(), param => PrintCommandCanExecute);
      ShowComparisonGraphicCommand = new ActionCommand(param => ShowComparisonGraphicCommandExecute(), param => ShowComparisonGraphicCommandCanExecute);
    }

    #endregion

    #region Public Members

    public override HelpTopic HelpTopicKey => HelpTopic.Compare;

    public FlowDocument ComparisonDocument { get; private set; }

    #endregion

    #region Print

    public ICommand PrintCommand { get; }

    private void PrintCommandExecute()
    {
      // TODO: Move to view
      //TreeHelper.GetParent<FlowDocumentReader>(ComparisonDocument);
      //var doc = ComparisonDocument.GetParent<FlowDocumentReader>();
      //if (doc != null)
      //{
      //  doc.Print();
      //}
    }

    private bool PrintCommandCanExecute => p1 != null && p2 != null;

    #endregion

    #region Show Comparison Graphic

    public ICommand ShowComparisonGraphicCommand { get; }

    private void ShowComparisonGraphicCommandExecute()
    {
      using var vm = new ShowSchedulesComparisonViewModel(p1, p2);
      ViewFactory.Instance.ShowDialog(vm);
    }

    private bool ShowComparisonGraphicCommandCanExecute => p1 != null && p2 != null;

    #endregion

    #region Private Members

    private void ShowComparisonResults()
    {
      ComparisonDocument.Blocks.Add(new Paragraph(new Bold(new Run(headline))) { FontSize = 16 });
      foreach (string s in strings)
      {
        ComparisonDocument.Blocks.Add(new Paragraph(new Run(s)) { FontSize = 12 });
      }

      foreach (var a1 in p1.Activities)
      {
        string s = "";
        var a2 = p2.Activities.ToList().Find(x => x.Number == a1.Number);
        if (a2 != null)
        {
          if (a1.Fragnet != null && a2.Fragnet != null && a1.Fragnet.IsVisible && !a2.Fragnet.IsVisible)
          {
            s += string.Format(NASResources.ActivityDoesNotExist, 2) + " ";
          }

          if (a1.Fragnet != null && a2.Fragnet != null && a2.Fragnet.IsVisible && !a1.Fragnet.IsVisible)
          {
            s += string.Format(NASResources.ActivityDoesNotExist, 1) + " ";
          }

          if (a2.RetardedDuration > a1.RetardedDuration)
          {
            s += string.Format(NASResources.DurationIncreased, a2.RetardedDuration - a1.RetardedDuration) + " ";
          }

          if (a2.RetardedDuration < a1.RetardedDuration)
          {
            s += string.Format(NASResources.DurationDecreased, a1.RetardedDuration - a2.RetardedDuration) + " ";
          }

          if (a2.IsStarted && !a1.IsStarted && a2.StartDate != a1.StartDate)
          {
            if (a2.StartDate > a1.StartDate)
            {
              s += string.Format(NASResources.ActivityStartedLater, a2.Calendar.GetWorkDays(a1.StartDate, a2.StartDate, true)) + " ";
            }
            else
            {
              s += string.Format(NASResources.ActivityStartedEarlier, a2.Calendar.GetWorkDays(a2.StartDate, a1.StartDate, true)) + " ";
            }
          }
          if (a2.IsFinished && !a1.IsFinished && a1.StartDate - a1.StartDate != a2.FinishDate - a2.StartDate)
          {
            if (a1.FinishDate - a1.StartDate > a2.FinishDate - a2.StartDate)
            {
              s += string.Format(NASResources.ActivityFinishedEarlier, a1.Calendar.GetWorkDays(a1.StartDate, a1.FinishDate, true) - a2.Calendar.GetWorkDays(a2.StartDate, a2.FinishDate, true)) + " ";
            }
            else
            {
              s += string.Format(NASResources.ActivityFinishedLater, a2.Calendar.GetWorkDays(a2.StartDate, a2.FinishDate, true) - a1.Calendar.GetWorkDays(a1.StartDate, a1.FinishDate, true)) + " ";
            }
          }
          if (a2.EarlyStartDate != a1.EarlyStartDate)
          {
            s += string.Format(NASResources.EarlyStartDateChanged, a1.EarlyStartDate.ToShortDateString(), a2.EarlyStartDate.ToShortDateString()) + " ";
          }

          if (a2.TotalFloat > a1.TotalFloat)
          {
            s += string.Format(NASResources.TotalFloatIncreased, a2.TotalFloat - a1.TotalFloat) + " ";
          }

          if (a2.TotalFloat < a1.TotalFloat)
          {
            s += string.Format(NASResources.TotalFloatDecreased, a1.TotalFloat - a2.TotalFloat) + " ";
            if (a2.TotalFloat < 0)
            {
              s += string.Format(NASResources.ActivityHasDelay, -a2.TotalFloat) + " ";
            }
          }
          if (a2.TotalPlannedCosts > a1.TotalPlannedCosts)
          {
            s += string.Format(NASResources.PlannedCostsIncreased, a2.TotalPlannedCosts - a1.TotalPlannedCosts) + " ";
          }

          if (a2.TotalPlannedCosts < a1.TotalPlannedCosts)
          {
            s += string.Format(NASResources.PlannedCostsDecreased, a1.TotalPlannedCosts - a2.TotalPlannedCosts) + " ";
          }

          if (a2.TotalActualCosts > a1.TotalActualCosts)
          {
            s += string.Format(NASResources.ActualCostsIncreased, a2.TotalActualCosts - a1.TotalActualCosts) + " ";
          }

          if (a2.TotalActualCosts < a1.TotalActualCosts)
          {
            s += string.Format(NASResources.ActualCostsDecreased, a1.TotalActualCosts - a2.TotalActualCosts) + " ";
          }

          if (a2.TotalActualCosts > a1.TotalBudget)
          {
            s += string.Format(NASResources.CostsHigherAsBudget, a2.TotalActualCosts - a1.TotalBudget) + " ";
          }
        }
        if (!string.IsNullOrWhiteSpace(s))
        {
          AddTextWithLabel(a1.Number + " (" + a1.Name + "): ", s);
        }
      }
      var e1 = p1.LastDay;
      var e2 = p2.LastDay;
      if (e1 == e2)
      {
        string s = NASResources.ProjectEndUnchanged;
        if (p2.EndDate.HasValue)
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

      if (p2.TotalPlannedCosts > p1.TotalPlannedCosts)
      {
        ComparisonDocument.Blocks.Add(new Paragraph(new Run(string.Format(NASResources.PlannedCostsIncreased, p2.TotalPlannedCosts - p1.TotalPlannedCosts))) { FontSize = 12 });
      }

      if (p2.TotalPlannedCosts < p1.TotalPlannedCosts)
      {
        ComparisonDocument.Blocks.Add(new Paragraph(new Run(string.Format(NASResources.PlannedCostsDecreased, p1.TotalPlannedCosts - p2.TotalPlannedCosts))) { FontSize = 12 });
      }

      if (p2.TotalBudget > p1.TotalBudget)
      {
        ComparisonDocument.Blocks.Add(new Paragraph(new Run(string.Format(NASResources.ProjectBudgetIncreased, p2.TotalBudget - p1.TotalBudget))) { FontSize = 12 });
      }

      if (p2.TotalBudget < p1.TotalBudget)
      {
        ComparisonDocument.Blocks.Add(new Paragraph(new Run(string.Format(NASResources.ProjectBudgetDecreased, p1.TotalBudget - p2.TotalBudget))) { FontSize = 12 });
      }

      if (p2.TotalActualCosts > p1.TotalActualCosts)
      {
        ComparisonDocument.Blocks.Add(new Paragraph(new Run(string.Format(NASResources.ActualCostsIncreased, p2.TotalActualCosts - p1.TotalActualCosts))) { FontSize = 12 });
      }

      if (p2.TotalActualCosts < p1.TotalActualCosts)
      {
        ComparisonDocument.Blocks.Add(new Paragraph(new Run(string.Format(NASResources.ActualCostsDecreased, p1.TotalActualCosts - p2.TotalActualCosts))) { FontSize = 12 });
      }

      p1 = null;
      p2 = null;
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
