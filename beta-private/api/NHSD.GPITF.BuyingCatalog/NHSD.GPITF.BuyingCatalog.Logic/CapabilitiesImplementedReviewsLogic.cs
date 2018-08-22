using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class CapabilitiesImplementedReviewsLogic : LogicBase, ICapabilitiesImplementedReviewsLogic
  {
    private readonly ICapabilitiesImplementedReviewsDatastore _datastore;
    private readonly IContactsDatastore _contacts;

    public CapabilitiesImplementedReviewsLogic(
      ICapabilitiesImplementedReviewsDatastore datastore,
      IContactsDatastore contacts,
      IHttpContextAccessor context) :
      base(context)
    {
      _datastore = datastore;
      _contacts = contacts;
    }

    public IQueryable<CapabilitiesImplementedReviews> ByEvidence(string evidenceId)
    {
      return _datastore.ByEvidence(evidenceId);
    }
  }
}
