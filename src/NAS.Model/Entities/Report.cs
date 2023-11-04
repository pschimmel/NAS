using System.ComponentModel.DataAnnotations;
using NAS.Model.Enums;

namespace NAS.Model.Entities
{
  public class Report
  {
    public event EventHandler NameChanged;
    public event EventHandler DataChanged;

    private string _name;
    private string _data;

    public Report()
    { }

    public Report(Report other)
    {
      ReportType = other.ReportType;
      ReportLevel = ReportLevel.User;
      Name = other.Name;
      Description = other.Description;
      FileName = other.FileName;
      IsReadOnly = false;
    }

    public ReportType ReportType { get; set; }

    public ReportLevel ReportLevel { get; set; }

    public string Name
    {
      get => _name;
      set
      {
        if (_name != value)
        {
          _name = value;
          NameChanged?.Invoke(this, EventArgs.Empty);
        }
      }
    }

    public string Description { get; set; }

    public string FileName { get; set; }

    public bool IsReadOnly { get; set; }

    [MaxLength]
    public string Data
    {
      get => _data;
      set
      {
        if (_data != value)
        {
          _data = value;
          DataChanged?.Invoke(this, EventArgs.Empty);
        }
      }
    }
  }
}
