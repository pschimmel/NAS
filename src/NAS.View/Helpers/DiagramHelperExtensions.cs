using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace NAS.View.Helpers
{
  public static class DiagramHelperExtensions
  {
    public static double GetRowY(this CollectionView view, object o, double rowHeight, double groupHeaderHeight)
    {
      if (view == null)
      {
        return 0;
      }

      if (view is IEditableCollectionView)
      {
        var MyEditableCollectionView = view as IEditableCollectionView;
        if (MyEditableCollectionView.IsAddingNew || MyEditableCollectionView.IsEditingItem)
        {
          MyEditableCollectionView.CommitEdit();
        }
      }
      view.Refresh();
      double result = 0;
      if (view.Groups == null)
      {
        return rowHeight * view.IndexOf(o);
      }

      foreach (CollectionViewGroup gd in view.Groups.Cast<CollectionViewGroup>())
      {
        result += groupHeaderHeight + GetItemY(gd, o, rowHeight, groupHeaderHeight, out bool found);
        if (found)
        {
          return result;
        }
      }
      return result;
    }

    private static double GetItemY(CollectionViewGroup gd, object o, double rowHeight, double groupHeaderHeight, out bool found)
    {
      found = false;
      if (gd.IsBottomLevel)
      {
        if (gd.Items.Contains(o))
        {
          found = true;
          return rowHeight * gd.Items.IndexOf(o);
        }
        else
        {
          return rowHeight * gd.Items.Count;
        }
      }
      else
      {
        double result = 0;
        foreach (CollectionViewGroup g in gd.Items.Cast<CollectionViewGroup>())
        {
          if (found == false)
          {
            result += groupHeaderHeight + GetItemY(g, o, rowHeight, groupHeaderHeight, out found);
          }
        }
        return result;
      }
    }

    private static int GetItemIndex(CollectionViewGroup gd, object o, out bool found)
    {
      found = false;
      if (gd.IsBottomLevel)
      {
        if (gd.Items.Contains(o))
        {
          found = true;
          return gd.Items.IndexOf(o);
        }
        else
        {
          return gd.Items.Count;
        }
      }
      else
      {
        int result = 0;
        foreach (CollectionViewGroup g in gd.Items.Cast<CollectionViewGroup>())
        {
          if (found == false)
          {
            result += 1 + GetItemIndex(g, o, out found);
          }
        }
        return result;
      }
    }

    private static int GetLevel(CollectionViewGroup gd, object o, int lastLevel)
    {
      if (gd.IsBottomLevel)
      {
        if (gd.Items.Contains(o))
        {
          return lastLevel + 1;
        }
      }
      else
      {
        foreach (CollectionViewGroup g in gd.Items.Cast<CollectionViewGroup>())
        {
          int i = GetLevel(g, o, lastLevel + 1);
          if (i > -1)
          {
            return i;
          }
        }
      }
      return -1;
    }

    /// <summary>
    /// Appends a point to a path data string.
    /// </summary>
    /// <param name="sb">The StringBuilder.</param>
    /// <param name="x">The x-value of the point.</param>
    /// <param name="y">The y value of the point.</param>
    /// <example>
    /// Result will be formated like this: M 0 0 L 5 0 L 35 40 L 40 40 L 36 37 L 40 40 L 36 43
    /// </example>
    public static void AppendPoint(this StringBuilder sb, double x, double y)
    {
      ArgumentNullException.ThrowIfNull(sb);

      if (string.IsNullOrWhiteSpace(sb.ToString()))
      {
        _ = sb.Append("M ");
      }
      else
      {
        if (sb.ToString() != null && !sb.ToString().EndsWith(" "))
        {
          _ = sb.Append(' ');
        }

        _ = sb.Append("L ");
      }
      _ = sb.AppendFormat(CultureInfo.GetCultureInfo("en"), "{0} {1}", x, y);
    }

    public static Color TryParseColor(this string s, Color defaultColor)
    {
      if (!string.IsNullOrWhiteSpace(s))
      {
        try
        {
          return (Color)ColorConverter.ConvertFromString(s);
        }
        catch
        {
        }
      }
      return defaultColor;
    }
  }
}
