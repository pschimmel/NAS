using System;
using System.Collections.Generic;

namespace NAS.ViewModel.Helpers
{
  /// <summary>
  /// Handles notifications to the user. Controls that want to show notifications need to implement <see cref="IUserNotificationTarget"/>
  /// and need to be registered.
  /// </summary>
  public class UserNotificationService
  {
    private static readonly Lazy<UserNotificationService> _lazy = new(new UserNotificationService());
    private readonly ISet<IUserNotificationTarget> _targets;

    private UserNotificationService()
    {
      _targets = new HashSet<IUserNotificationTarget>();
    }

    public static UserNotificationService Instance => _lazy.Value;

    /// <summary>
    /// Registers a target that shows notifications to the user.
    /// </summary>
    public void RegisterTarget(IUserNotificationTarget target)
    {
      _ = _targets.Add(target);
    }

    /// <summary>
    /// Removes a target.
    /// </summary>
    public void UnregisterTarget(IUserNotificationTarget target)
    {
      _ = _targets.Remove(target);
    }

    /// <summary>
    /// Notifies the user about an error.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="okAction">[optional] The action that will be executed when the OK button is pressed.</param>
    /// <param name="cancelAction">[optional] The action that will be executed when the Cancel button is pressed.</param>
    public void Error(string message, Action okAction = null, Action cancelAction = null)
    {
      if (string.IsNullOrWhiteSpace(message))
      {
        throw new ArgumentException("Argument message cannot be empty.", nameof(message));
      }

      foreach (var target in _targets)
      {
        target.Error(message, okAction ?? new Action(() => { }), cancelAction);
      }
    }

    /// <summary>
    /// Notifies the user about a warning.
    /// </summary>
    /// <param name="message">The warning message.</param>
    /// <param name="okAction">[optional] The action that will be executed when the OK button is pressed.</param>
    /// <param name="cancelAction">[optional] The action that will be executed when the Cancel button is pressed.</param>
    public void Warning(string message, Action okAction = null, Action cancelAction = null)
    {
      if (string.IsNullOrWhiteSpace(message))
      {
        throw new ArgumentException("Argument message cannot be empty.", nameof(message));
      }

      foreach (var target in _targets)
      {
        target.Warning(message, okAction ?? new Action(() => { }), cancelAction);
      }
    }

    /// <summary>
    /// Notifies the user about something.
    /// </summary>
    /// <param name="message">The message.</param>
    public void Information(string message)
    {
      if (string.IsNullOrWhiteSpace(message))
      {
        throw new ArgumentException("Argument message cannot be empty.", nameof(message));
      }

      foreach (var target in _targets)
      {
        target.Information(message);
      }
    }

    /// <summary>
    /// Asks the user something.
    /// </summary>
    /// <param name="message">The question.</param>
    /// <param name="yesAction">The action that will be executed when the Yes button is pressed.</param>
    /// <param name="noAction">[optional] The action that will be executed when the No button is pressed.</param>
    /// <param name="cancelAction">[optional] The action that will be executed when the Cancel button is pressed.</param>
    public void Question(string message, Action yesAction, Action noAction = null, Action cancelAction = null)
    {
      if (string.IsNullOrWhiteSpace(message))
      {
        throw new ArgumentException("Argument message cannot be empty.", nameof(message));
      }

      foreach (var target in _targets)
      {
        target.Question(message, yesAction ?? new Action(() => { }), noAction ?? new Action(() => { }), cancelAction ?? new Action(() => { }));
      }
    }
  }
}
