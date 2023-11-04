using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NAS.Model.Entities
{
  public class Fragnet : NASObject
  {
    private string number;
    private string name;
    private string description;
    private bool isDisputable;
    private DateTime identified;
    private DateTime? approved;
    private DateTime? submitted;
    private bool isVisible;

    public Fragnet()
    {
      identified = DateTime.Today;
      isVisible = true;
      Activities = new ObservableCollection<Activity>();
      Distortion = new ObservableCollection<Distortion>();
    }

    public string Number
    {
      get => number;
      set
      {
        if (number != value)
        {
          number = value;
          OnPropertyChanged(nameof(Number));
        }
      }
    }

    public string Name
    {
      get => name;
      set
      {
        if (name != value)
        {
          name = value;
          OnPropertyChanged(nameof(Name));
        }
      }
    }

    public string Description
    {
      get => description;
      set
      {
        if (description != value)
        {
          description = value;
          OnPropertyChanged(nameof(Description));
        }
      }
    }

    public bool IsDisputable
    {
      get => isDisputable;
      set
      {
        if (isDisputable != value)
        {
          isDisputable = value;
          OnPropertyChanged(nameof(IsDisputable));
        }
      }
    }

    public DateTime Identified
    {
      get => identified;
      set
      {
        if (identified != value)
        {
          identified = value;
          OnPropertyChanged(nameof(Identified));
        }
      }
    }

    public DateTime? Approved
    {
      get => approved;
      set
      {
        if (approved != value)
        {
          approved = value;
          OnPropertyChanged(nameof(Approved));
        }
      }
    }

    public DateTime? Submitted
    {
      get => submitted;
      set
      {
        if (submitted != value)
        {
          submitted = value;
          OnPropertyChanged(nameof(Submitted));
        }
      }
    }

    public bool IsVisible
    {
      get => isVisible;
      set
      {
        if (isVisible != value)
        {
          isVisible = value;
          OnPropertyChanged(nameof(IsVisible));
        }
      }
    }

    public virtual ICollection<Activity> Activities { get; set; }
    public virtual Schedule Schedule { get; set; }
    public virtual ICollection<Distortion> Distortion { get; set; }

    #region Activity

    public void RefreshActibities(IEnumerable<Activity> activities)
    {
      if (activities == null)
      {
        throw new ArgumentNullException(nameof(activities), "Argument can't be null");
      }

      Activities.Clear();

      foreach (var activity in activities)
      {
        Activities.Add(activity);
      }
    }

    #endregion


    public override string ToString()
    {
      return Name;
    }
  }
}
