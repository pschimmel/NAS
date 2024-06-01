using System.Collections.Generic;
using NAS.Models.Entities;

namespace NAS.ViewModels
{
  public class ComparisonData
  {
    public ComparisonData(Schedule p1, Schedule p2) {
      Schedule1 = p1;
      Schedule2 = p2;
    }

    public Schedule Schedule1 { get; }

    public Schedule Schedule2 { get; }

    public string Headline { get; set; }

    public List<string> Text { get; set; }
  }
}
