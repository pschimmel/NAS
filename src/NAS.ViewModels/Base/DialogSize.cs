namespace NAS.ViewModels.Base
{
  public class DialogSize
  {
    private DialogSize(bool auto)
    {
      IsAuto = auto;
      IsFixed = false;
    }

    private DialogSize(bool isFixed, double width, double height)
    {
      IsAuto = false;
      Width = width;
      Height = height;
      IsFixed = isFixed;
    }

    private DialogSize(bool canResize, double width, double height, double minWidth, double minHeight, double maxWidth, double maxHeight)
      : this(canResize, width, height)
    {
      MinWidth = minWidth;
      MinHeight = minHeight;
      MaxWidth = maxWidth;
      MaxHeight = maxHeight;
    }

    public bool IsAuto { get; } = true;

    public bool IsFixed { get; } = true;

    public double Width { get; } = double.NaN;

    public double Height { get; } = double.NaN;

    public double MinWidth { get; } = double.NaN;

    public double MinHeight { get; } = double.NaN;

    public double MaxWidth { get; } = double.NaN;

    public double MaxHeight { get; } = double.NaN;

    public static DialogSize Auto { get; } = new DialogSize(true);

    public static DialogSize Fixed(double width, double height)
    {
      return new DialogSize(true, width, height);
    }

    public static DialogSize Initial(double width, double height)
    {
      return new DialogSize(false, width, height);
    }

    public static DialogSize Initial(double width, double height, double minWidth, double minHeight, double maxWidth, double maxHeight)
    {
      return new DialogSize(false, width, height, minWidth, minHeight, maxWidth, maxHeight);
    }
  }
}
