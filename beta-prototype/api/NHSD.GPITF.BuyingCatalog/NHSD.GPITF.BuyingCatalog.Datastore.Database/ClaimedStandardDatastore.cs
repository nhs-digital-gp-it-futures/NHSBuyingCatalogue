using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.Database.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Datastore.Database
{
  public sealed class ClaimedStandardDatastore : DatastoreBase<ClaimedStandard>, IClaimedStandardDatastore
  {
    public ClaimedStandardDatastore(IDbConnectionFactory dbConnectionFactory, ILogger<ClaimedStandardDatastore> logger, ISyncPolicyFactory policy) :
      base(dbConnectionFactory, logger, policy)
    {
    }

    public IQueryable<ClaimedStandard> BySolution(string solutionId)
    {
      return GetInternal(() =>
      {
        return _dbConnection.Value.GetAll<ClaimedStandard>().Where(cs => cs.SolutionId == solutionId).AsQueryable();
      });
    }

    public ClaimedStandard Create(ClaimedStandard claimedstandard)
    {
      return GetInternal(() =>
      {
        using (var trans = _dbConnection.Value.BeginTransaction())
        {
          claimedstandard.Id = Guid.NewGuid().ToString();
          _dbConnection.Value.Insert(claimedstandard, trans);
          trans.Commit();

          return claimedstandard;
        }
      });
    }

    public void Update(ClaimedStandard claimedstandard)
    {
      GetInternal(() =>
      {
        using (var trans = _dbConnection.Value.BeginTransaction())
        {
          _dbConnection.Value.Update(claimedstandard, trans);
          trans.Commit();
          return 0;
        }
      });
    }

    public void Delete(ClaimedStandard claimedstandard)
    {
      GetInternal(() =>
      {
        using (var trans = _dbConnection.Value.BeginTransaction())
        {
          _dbConnection.Value.Delete(claimedstandard, trans);
          trans.Commit();
          return 0;
        }
      });
    }
  }
}
