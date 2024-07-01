using NAS.Models.Entities;
using NAS.Models.Scheduler;
using NAS.Resources;
using NAS.ViewModels.Base;
using NAS.ViewModels.Helpers;

namespace NAS.ViewModels
{
  public class CompareSchedulesViewModel : ViewModelBase
  {
    #region Fields

    private DateTime dataDate;
    private List<CompareScheduleItem> fragnets1;
    private List<CompareScheduleItem> fragnets2;

    #endregion

    #region Constructor

    public CompareSchedulesViewModel(Schedule schedule)
      : base()
    {
      Schedule = schedule;
      dataDate = schedule.DataDate;
    }

    #endregion

    #region Public Members

    public override HelpTopic HelpTopicKey => HelpTopic.Compare;

    public Schedule Schedule { get; private set; }

    public DateTime DataDate
    {
      get => dataDate;
      set
      {
        if (dataDate != value)
        {
          dataDate = value;
          OnPropertyChanged(nameof(DataDate));
        }
      }
    }

    public List<CompareScheduleItem> Fragnets1
    {
      get
      {
        if (fragnets1 == null)
        {
          fragnets1 = [];
          foreach (var f in Schedule.Fragnets)
          {
            fragnets1.Add(new CompareScheduleItem(f) { IsChecked = f.IsVisible });
          }
        }
        return fragnets1;
      }
    }

    public List<CompareScheduleItem> Fragnets2
    {
      get
      {
        if (fragnets2 == null)
        {
          fragnets2 = [];
          foreach (var f in Schedule.Fragnets)
          {
            fragnets2.Add(new CompareScheduleItem(f) { IsChecked = f.IsVisible });
          }
        }
        return fragnets2;
      }
    }

    public ComparisonData Compare()
    {
      var fragnets1 = new List<Fragnet>();
      foreach (var f in Fragnets1)
      {
        if (f.IsChecked)
        {
          fragnets1.Add(f.Fragnet);
        }
      }
      var fragnets2 = new List<Fragnet>();
      foreach (var f in Fragnets2)
      {
        if (f.IsChecked)
        {
          fragnets2.Add(f.Fragnet);
        }
      }
      var p1 = new Schedule(Schedule);
      var p2 = new Schedule(Schedule);
      var result = new ComparisonData(p1, p2);
      foreach (var f in p1.Fragnets)
      {
        f.IsVisible = fragnets1.Any(x => x.ID == f.ID);
      }

      foreach (var f in p2.Fragnets)
      {
        f.IsVisible = fragnets2.Any(x => x.ID == f.ID);
      }

      var d = Schedule.DataDate;
      var s1 = new Scheduler(p1);
      s1.Calculate(d);
      var s2 = new Scheduler(p2);
      s2.Calculate(d);
      // Prepare headline
      string h1 = NASResources.Schedule1 + " (";
      foreach (var item in fragnets1)
      {
        if (h1 != NASResources.Schedule1 + " (")
        {
          h1 += ", ";
        }

        h1 += item;
      }
      if (h1 == NASResources.Schedule1 + " (")
      {
        h1 += NASResources.NoActiveFragnets;
      }

      h1 += ")";
      string h2 = NASResources.Schedule2 + " (";
      foreach (var item in fragnets2)
      {
        if (h2 != NASResources.Schedule2 + " (")
        {
          h2 += ", ";
        }

        h2 += item;
      }
      if (h2 == NASResources.Schedule2 + " (")
      {
        h2 += NASResources.NoActiveFragnets;
      }

      h2 += ")";
      result.Headline = string.Format(NASResources.SchedulesCompared, h1, h2);
      result.Text = [];
      foreach (var item in fragnets2)
      {
        if (!string.IsNullOrWhiteSpace(item.Description) && !string.IsNullOrWhiteSpace(item.Name))
        {
          result.Text.Add(item.Name + ": " + item.Description);
        }
      }
      return result;
    }

    #endregion
  }
}
