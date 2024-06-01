namespace NAS.Views.Controls
{
  public class CheckPositionEventArgs : EventArgs
  {
    public CheckPositionEventArgs() { }

    public bool Result { get; set; } = true;
  }
}
