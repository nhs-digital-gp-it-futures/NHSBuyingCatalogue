using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.Database.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Datastore.Database
{
  public sealed class CapabilitiesImplementedEvidenceDatastore : DatastoreBase<CapabilitiesImplementedEvidence>, ICapabilitiesImplementedEvidenceDatastore
  {
    public CapabilitiesImplementedEvidenceDatastore(
      IDbConnectionFactory dbConnectionFactory,
      ILogger<CapabilitiesImplementedEvidenceDatastore> logger,
      ISyncPolicyFactory policy) :
      base(dbConnectionFactory, logger, policy)
    {
    }

    public IQueryable<CapabilitiesImplementedEvidence> ByCapabilitiesImplemented(string capabilitiesImplementedId)
    {
      return GetInternal(() =>
      {
        var retval = _dbConnection.Value.GetAll<CapabilitiesImplementedEvidence>().Where(cie => cie.CapabilitiesImplementedId == capabilitiesImplementedId);
        return retval.AsQueryable();
      });
    }

    public CapabilitiesImplementedEvidence Create(CapabilitiesImplementedEvidence evidence)
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
