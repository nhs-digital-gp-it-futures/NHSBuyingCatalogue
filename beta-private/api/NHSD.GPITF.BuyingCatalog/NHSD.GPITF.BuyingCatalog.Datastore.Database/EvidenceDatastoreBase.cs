using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.Database.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Datastore.Database
{
  public abstract class EvidenceDatastoreBase<T> : DatastoreBase<T> where T : EvidenceBase
  {
    public EvidenceDatastoreBase(
      IDbConnectionFactory dbConnectionFactory,
      ILogger<EvidenceDatastoreBase<T>> logger,
      ISyncPolicyFactory policy) :
      base(dbConnectionFactory, logger, policy)
    {
    }

    public IQueryable<T> ByClaim(string claimId)
    {
      return GetInternal(() =>
      {
        var retval = _dbConnection.Value.GetAll<T>().Where(sae => sae.ClaimId == claimId);
        return retval.AsQueryable();
      });
    }

    public T Create(T evidence)
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
