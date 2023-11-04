using System;

namespace NAS.Model.Base
{
  /// <summary>
  /// Event arguments that contain and error message and severity.
  /// </summary>
  public class ErrorEventArgs : MessageEventArgs
  {
    public ErrorEventArgs(string message, ErrorSeverity severity = ErrorSeverity.Error)
      : base(message)
    {
      Severity = severity;
    }

    public ErrorSeverity Severity { get; }

    public Exception ErrorException { get; private set; }

    /// <summary>
    /// Error message on information level.
    /// </summary>
    public static ErrorEventArgs Information(string message)
    {
      return new ErrorEventArgs(message, ErrorSeverity.Information);
    }

    /// <summary>
    /// A warning message.
    /// </summary>
    public static ErrorEventArgs Warning(string message)
    {
      return new ErrorEventArgs(message, ErrorSeverity.Warning);
    }

    /// <summary>
    /// An error message.
    /// </summary>
    public static ErrorEventArgs Error(string message)
    {
      return new ErrorEventArgs(message);
    }

    /// <summary>
    /// Creates an error message from an exception.
    /// </summary>
    public static ErrorEventArgs Exception(Exception ex)
    {
      string text = ex.Message;
      for (var innerException = ex.InnerException; innerException != null; innerException = innerException.InnerException)
      {
        text = text + Environment.NewLine + innerException.Message;
      }

      return new ErrorEventArgs(text)
      {
        ErrorException = ex
      };
    }

    public enum ErrorSeverity
    {
      Information,
      Warning,
      Error
    }
  }
}
