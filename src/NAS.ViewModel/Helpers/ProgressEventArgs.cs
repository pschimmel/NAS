using System;

namespace NAS.ViewModel.Helpers
{
  public class ProgressEventArgs : EventArgs
  {
    private ProgressEventArgs(double progress)
    {
      CurrentProgress = progress;
    }

    public double CurrentProgress { get; }

    public static readonly ProgressEventArgs Starting = new ProgressEventArgs(0);

    public static readonly ProgressEventArgs Finished = new ProgressEventArgs(100);

    public static ProgressEventArgs Progress(double progress)
    {
      return new ProgressEventArgs(progress);
    }
  }
}
