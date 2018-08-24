using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.Database.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Datastore.Database
{
  public abstract class ClaimsDatastoreBase<T> : DatastoreBase<T> where T : ClaimsBase
  {
    public ClaimsDatastoreBase(
      IDbConnectionFactory dbConnectionFactory,
      ILogger<ClaimsDatastoreBase<T>> logger,
      ISyncPolicyFactory policy) :
      base(dbConnectionFactory, logger, policy)
    {
    }

    public IQueryable<T> BySolution(string solutionId)
    {
      return GetInternal(() =>
      {
        return _dbConnection.Value.GetAll<T>().Where(cc => cc.SolutionId == solutionId).AsQueryable();
      });
    }

    public T Create(T claimedcapability)
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

    public void Update(T claimedcapability)
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

    public void Delete(T claimedcapability)
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
