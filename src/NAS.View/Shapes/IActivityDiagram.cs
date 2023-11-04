namespace NAS.View.Shapes
{
  public interface IActivityDiagram<T> where T : class
  {
    T Item { get; }
  }
}