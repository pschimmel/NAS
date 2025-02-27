using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using ES.Tools.Core.MVVM;
using NAS.Models.Entities;
using NAS.ViewModels;

namespace NAS
{
  /// <summary>
  /// Interaction logic for WindowDistortions.xaml
  /// </summary>
  public partial class WindowDistortions : IView
  {
    public WindowDistortions()
    {
      InitializeComponent();
    }

    public IViewModel ViewModel
    {
      get => DataContext as IViewModel;
      set => DataContext = value;
    }

    private void ButtonOK_Click(object sender, RoutedEventArgs e)
    {
      DialogResult = true;
    }

    private void Update()
    {
      canvas.Children.Clear();
      var l = new GanttLayout(); // Create layout to get standard colors
      var rect = new Rectangle
      {
        Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(l.ActivityStandardColor)),
        Stroke = Brushes.Black,
        RadiusX = 2,
        RadiusY = 2
      };
      canvas.Children.Add(rect);
      rect.Height = canvas.ActualHeight;
      rect.Width = canvas.ActualWidth;
      foreach (Distortion distortion in listBox.Items)
      {
        var r = new Rectangle
        {
          Fill = new SolidColorBrush(Color.FromArgb(100, 255, 0, 0))
        };
        canvas.Children.Add(r);
        if (distortion is Delay && (distortion as Delay).Days.HasValue)
        {
          int days = (distortion as Delay).Days.Value;
          r.Width = GetX(distortion, days);
          r.Height = rect.Height;
        }
        else if (distortion is Interruption && (distortion as Interruption).Days.HasValue)
        {
          int days = (distortion as Interruption).Days.Value;
          r.Width = GetX(distortion, days);
          r.Height = rect.Height;
          double d = ((distortion as Interruption).Start.Value - (DataContext as ScheduleViewModel).CurrentActivity.Activity.EarlyStartDate).TotalDays;
          Canvas.SetLeft(r, GetX(distortion, d));
        }
        else if (distortion is Inhibition && (distortion as Inhibition).Percent.HasValue)
        {
          double percent = (distortion as Inhibition).Percent.Value;
          if (percent > 100)
          {
            percent = 100;
          }

          r.Width = rect.Width;
          r.Height = rect.Height * percent / 100;
        }
        else if (distortion is Extension && (distortion as Extension).Days.HasValue)
        {
          int days = (distortion as Extension).Days.Value;
          r.Width = GetX(distortion, days);
          r.Height = rect.Height;
          Canvas.SetLeft(r, rect.Width - r.Width);
        }
        else if (distortion is Reduction && (distortion as Reduction).Days.HasValue)
        {
          int days = (distortion as Reduction).Days.Value;
          r.Width = GetX(distortion, days);
          r.Height = rect.Height;
          Canvas.SetLeft(r, rect.Width - r.Width);
          r.Fill = new SolidColorBrush(Color.FromArgb(100, 0, 0, 255));
        }
        r.Clip = new RectangleGeometry(new Rect(0, 0, rect.Width, rect.Height), rect.RadiusX, rect.RadiusY);
      }
    }

    private double GetX(Distortion distortion, double day)
    {
      //TODO: Implement this method
      return 0;

      //double w = canvas.ActualWidth;
      //double days = distortion.Activity.OriginalDuration;
      //foreach (var d in distortion.Activity.Distortions)
      //{
      //  if (d is Delay && (d as Delay).Days.HasValue)
      //  {
      //    days += (d as Delay).Days.Value;
      //  }
      //  else if (d is Interruption && (d as Interruption).Days.HasValue)
      //  {
      //    days += (d as Interruption).Days.Value;
      //  }
      //  else if (d is Inhibition && (d as Inhibition).Percent.HasValue)
      //  {
      //    days *= (d as Inhibition).Percent.Value / 100 + 1;
      //  }
      //  else if (d is Extension && (d as Extension).Days.HasValue)
      //  {
      //    days += (d as Extension).Days.Value;
      //  }
      //}

      //return days > 0 ? w / days * day : 0;
    }

    private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      Update();
    }

    private void Canvas_Loaded(object sender, RoutedEventArgs e)
    {
      Update();
    }
  }
}
