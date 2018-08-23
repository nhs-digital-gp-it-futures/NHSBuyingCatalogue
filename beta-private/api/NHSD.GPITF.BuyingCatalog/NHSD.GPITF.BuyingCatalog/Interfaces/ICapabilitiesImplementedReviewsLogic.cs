using NHSD.GPITF.BuyingCatalog.Models;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Interfaces
{
#pragma warning disable CS1591
  public interface ICapabilitiesImplementedReviewsLogic
  {
    IQueryable<CapabilitiesImplementedReviews> ByEvidence(string evidenceId);
    CapabilitiesImplementedReviews Create(CapabilitiesImplementedReviews review);
  }
#pragma warning restore CS1591
}
