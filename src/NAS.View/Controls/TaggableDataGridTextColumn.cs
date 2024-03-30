using System.Windows.Controls;

namespace NAS.View.Controls
{
  internal class TaggableDataGridTextColumn : DataGridTextColumn, ITaggable
  {
    public object Tag { get; set; }
  }
}
