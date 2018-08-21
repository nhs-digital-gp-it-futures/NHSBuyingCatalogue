using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.Database.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Datastore.Database
{
  public sealed class StandardsApplicableDatastore : DatastoreBase<StandardsApplicable>, IStandardsApplicableDatastore
  {
    public StandardsApplicableDatastore(IDbConnectionFactory dbConnectionFactory, ILogger<StandardsApplicableDatastore> logger, ISyncPolicyFactory policy) :
      base(dbConnectionFactory, logger, policy)
    {
    }

    public IQueryable<StandardsApplicable> BySolution(string solutionId)
    {
      return GetInternal(() =>
      {
        return _dbConnection.Value.GetAll<StandardsApplicable>().Where(cs => cs.SolutionId == solutionId).AsQueryable();
      });
    }

    public StandardsApplicable Create(StandardsApplicable claimedstandard)
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

    public void Update(StandardsApplicable claimedstandard)
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

    public void Delete(StandardsApplicable claimedstandard)
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
