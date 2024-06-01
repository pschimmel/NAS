namespace NAS.ViewModels.Helpers
{
  public static class TextExtensions
  {
    /// <summary>
    /// Strips the BOM (Byte order mark) U+FEFF and ZERO WIDTH SPACE U+200B.
    /// </summary>
    /// <returns>Clean string</returns>
    public static string StripBOM(this string s)
    {
      return s?.Trim(new char[] { '\uFEFF', '\u200B' }) ?? null;
    }
  }
}
