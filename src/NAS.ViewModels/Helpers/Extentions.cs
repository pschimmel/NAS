using NAS.Models.Entities;

namespace NAS.ViewModels.Helpers
{
  internal static class Extentions
  {
    public static void Replace<T>(this ICollection<T> collection, IEnumerable<T> items)
{
      collection.Clear();
      collection.AddRange(items);
    }

    public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
    {
      foreach (T item in items)
      {
        collection.Add(item);
      }
    }
  }
}
