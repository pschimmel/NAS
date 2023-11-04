using System;

namespace NAS.Model.Base
{
  /// <summary>
  /// Event arguments for transferring a message.
  /// </summary>
  public class MessageEventArgs : EventArgs
  {
    public MessageEventArgs(string message)
    {
      Message = message;
    }

    public string Message { get; }
  }
}
