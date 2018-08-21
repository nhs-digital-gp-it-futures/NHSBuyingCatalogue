using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.Database.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Datastore.Database
{
  public sealed class CapabilitiesImplementedDatastore : DatastoreBase<CapabilitiesImplemented>, ICapabilitiesImplementedDatastore
  {
    public CapabilitiesImplementedDatastore(IDbConnectionFactory dbConnectionFactory, ILogger<CapabilitiesImplementedDatastore> logger, ISyncPolicyFactory policy) :
      base(dbConnectionFactory, logger, policy)
    {
    }

    public IQueryable<CapabilitiesImplemented> BySolution(string solutionId)
    {
      return GetInternal(() =>
      {
        return _dbConnection.Value.GetAll<CapabilitiesImplemented>().Where(cc => cc.SolutionId == solutionId).AsQueryable();
      });
    }

    public CapabilitiesImplemented Create(CapabilitiesImplemented claimedcapability)
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

    public void Update(CapabilitiesImplemented claimedcapability)
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

    public void Delete(CapabilitiesImplemented claimedcapability)
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
