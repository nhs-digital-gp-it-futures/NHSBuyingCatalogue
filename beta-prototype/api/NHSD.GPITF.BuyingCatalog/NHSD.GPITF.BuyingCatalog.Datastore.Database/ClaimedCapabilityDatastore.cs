using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.Database.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Datastore.Database
{
  public sealed class ClaimedCapabilityDatastore : DatastoreBase<ClaimedCapability>, IClaimedCapabilityDatastore
  {
    public ClaimedCapabilityDatastore(IDbConnectionFactory dbConnectionFactory, ILogger<ClaimedCapabilityDatastore> logger, ISyncPolicyFactory policy) :
      base(dbConnectionFactory, logger, policy)
    {
    }

    public IQueryable<ClaimedCapability> BySolution(string solutionId)
    {
      return GetInternal(() =>
      {
        return _dbConnection.Value.GetAll<ClaimedCapability>().Where(cc => cc.SolutionId == solutionId).AsQueryable();
      });
    }

    public ClaimedCapability Create(ClaimedCapability claimedcapability)
    {
      return GetInternal(() =>
      {
        using (var trans = _dbConnection.Value.BeginTransaction())
        {
          claimedcapability.Id = Guid.NewGuid().ToString();
          _dbConnection.Value.Insert(claimedcapability, trans);
          trans.Commit();

          return claimedcapability;
        }
      });
    }

    public void Update(ClaimedCapability claimedcapability)
    {
      GetInternal(() =>
      {
        using (var trans = _dbConnection.Value.BeginTransaction())
        {
          _dbConnection.Value.Update(claimedcapability, trans);
          trans.Commit();
          return 0;
        }
      });
    }

    public void Delete(ClaimedCapability claimedcapability)
    {
      GetInternal(() =>
      {
        using (var trans = _dbConnection.Value.BeginTransaction())
        {
          _dbConnection.Value.Delete(claimedcapability, trans);
          trans.Commit();
          return 0;
        }
      });
    }
  }
}
