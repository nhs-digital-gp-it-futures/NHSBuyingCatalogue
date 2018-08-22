using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.Database.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Datastore.Database
{
  public sealed class CapabilitiesImplementedReviewsDatastore : DatastoreBase<CapabilitiesImplementedReviews>, ICapabilitiesImplementedReviewsDatastore
  {
    public CapabilitiesImplementedReviewsDatastore(
      IDbConnectionFactory dbConnectionFactory, 
      ILogger<CapabilitiesImplementedReviewsDatastore> logger, 
      ISyncPolicyFactory policy) :
      base(dbConnectionFactory, logger, policy)
    {
    }

    public IQueryable<CapabilitiesImplementedReviews> ByEvidence(string evidenceId)
    {
      return GetInternal(() =>
      {
        var retval = _dbConnection.Value.GetAll<CapabilitiesImplementedReviews>().Where(cir => cir.CapabilitiesImplementedEvidenceId == evidenceId);
        return retval.AsQueryable();
      });
    }
  }
}
