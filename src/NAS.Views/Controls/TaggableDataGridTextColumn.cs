using System.Windows.Controls;

namespace NAS.Views.Controls
{
  internal class TaggableDataGridTextColumn : DataGridTextColumn, ITaggable
  {
    public object Tag { get; set; }
  }
}
