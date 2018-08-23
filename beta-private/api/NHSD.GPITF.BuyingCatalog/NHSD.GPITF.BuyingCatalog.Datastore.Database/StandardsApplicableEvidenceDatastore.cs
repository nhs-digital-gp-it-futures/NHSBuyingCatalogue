using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.Database.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Datastore.Database
{
  public sealed class StandardsApplicableEvidenceDatastore : DatastoreBase<StandardsApplicableEvidence>, IStandardsApplicableEvidenceDatastore
  {
    public StandardsApplicableEvidenceDatastore(
      IDbConnectionFactory dbConnectionFactory,
      ILogger<StandardsApplicableEvidenceDatastore> logger,
      ISyncPolicyFactory policy) :
      base(dbConnectionFactory, logger, policy)
    {
    }

    public IQueryable<StandardsApplicableEvidence> ByStandardsApplicable(string standardsApplicableId)
    {
      return GetInternal(() =>
      {
        var retval = _dbConnection.Value.GetAll<StandardsApplicableEvidence>().Where(sae => sae.StandardsApplicableId == standardsApplicableId);
        return retval.AsQueryable();
      });
    }

    public StandardsApplicableEvidence Create(StandardsApplicableEvidence evidence)
    {
      return GetInternal(() =>
      {
        using (var trans = _dbConnection.Value.BeginTransaction())
        {
          evidence.Id = Guid.NewGuid().ToString();
          evidence.CreatedOn = DateTime.UtcNow;
          _dbConnection.Value.Insert(evidence, trans);
          trans.Commit();

          return evidence;
        }
      });
    }
  }
}
