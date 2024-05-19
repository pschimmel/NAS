using NAS.ViewModel.Base;

namespace NAS.ViewModel
{
  public class GetDateViewModel : DialogContentViewModel
  {
    public GetDateViewModel(string title, DateTime? date = null, DateTime? displayDateStart = null, DateTime? displayDateEnd = null)
    {
      Title = title;
      StartDate = displayDateStart;
      EndDate = displayDateEnd;
      Date = date ?? DateTime.Now;
    }

    public override string Title { get; }

    public DateTime? StartDate { get; }

    public DateTime? EndDate { get; }

    public DateTime Date { get; set; }
  }
}
