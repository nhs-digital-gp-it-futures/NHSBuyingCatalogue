using NHSD.GPITF.BuyingCatalog.Models;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Interfaces
{
#pragma warning disable CS1591
  public interface IStandardsApplicableReviewsLogic
  {
    IQueryable<StandardsApplicableReviews> ByEvidence(string evidenceId);
    StandardsApplicableReviews Create(StandardsApplicableReviews review);
  }
#pragma warning restore CS1591
}
