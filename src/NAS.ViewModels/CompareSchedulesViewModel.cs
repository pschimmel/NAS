using NAS.Models.Entities;
using NAS.Models.Scheduler;
using NAS.Resources;
using NAS.ViewModels.Base;
using NAS.ViewModels.Helpers;

namespace NAS.ViewModels
{
  public class CompareSchedulesViewModel : DialogContentViewModel
  {
    #region Fields

    private readonly Schedule _schedule;
    private DateTime _dataDate;
    private List<CompareScheduleItem> _fragnets1;
    private List<CompareScheduleItem> _fragnets2;

    #endregion

    #region Constructor

    public CompareSchedulesViewModel(Schedule schedule)
      : base()
    {
      _schedule = schedule;
      _dataDate = schedule.DataDate;
    }

    #endregion

    #region Overwritten Members

    public override string Title => NASResources.Comparison;

    public override string Icon => "Compare";

    public override DialogSize DialogSize => DialogSize.Fixed(450, 300);

    public override HelpTopic HelpTopicKey => HelpTopic.Compare;

    #endregion

    #region Properties

    public DateTime DataDate
    {
      get => _dataDate;
      set
      {
        if (_dataDate != value)
        {
          _dataDate = value;
          OnPropertyChanged(nameof(DataDate));
        }
      }
    }

    public List<CompareScheduleItem> Fragnets1
    {
      get
      {
        if (_fragnets1 == null)
        {
          _fragnets1 = [];
          foreach (var f in _schedule.Fragnets)
          {
            _fragnets1.Add(new CompareScheduleItem(f) { IsChecked = f.IsVisible });
          }
        }
        return _fragnets1;
      }
    }

    public List<CompareScheduleItem> Fragnets2
    {
      get
      {
        if (_fragnets2 == null)
        {
          _fragnets2 = [];
          foreach (var f in _schedule.Fragnets)
          {
            _fragnets2.Add(new CompareScheduleItem(f) { IsChecked = f.IsVisible });
          }
        }
        return _fragnets2;
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
      var p1 = _schedule.Clone();
      var p2 = _schedule.Clone();
      var result = new ComparisonData(p1, p2);
      foreach (var f in p1.Fragnets)
      {
        f.IsVisible = fragnets1.Any(x => x.ID == f.ID);
      }

      foreach (var f in p2.Fragnets)
      {
        f.IsVisible = fragnets2.Any(x => x.ID == f.ID);
      }

      var d = _schedule.DataDate;
      var s1 = new Scheduler(p1);
      s1.Calculate(d);
      var s2 = new Scheduler(p2);
      s2.Calculate(d);
      // Prepare _headline
      string h1 = NASResources.Schedule1 + " (";
      foreach (var item in fragnets1)
      {
        if (h1 != NASResources.Schedule1 + " (")
        {
          h1 += ", ";
        }

        h1 += item.Name;
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

        h2 += item.Name;
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
