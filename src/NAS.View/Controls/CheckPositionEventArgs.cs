namespace NAS.View.Controls
{
  public class CheckPositionEventArgs : EventArgs
  {
    public CheckPositionEventArgs() { }

    public bool Result { get; set; } = true;
  }
}
