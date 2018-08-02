using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.Database.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Datastore.Database
{
  public sealed class CapabilityDatastore : DatastoreBase<Capability>, ICapabilityDatastore
  {
    public CapabilityDatastore(IDbConnectionFactory dbConnectionFactory, ILogger<CapabilityDatastore> logger, ISyncPolicyFactory policy) :
      base(dbConnectionFactory, logger, policy)
    {
    }

    public IQueryable<Capability> ByFramework(string frameworkId)
    {
      return GetInternal(() =>
      {
        var sql = $@"
select cap.* from Capability cap
join CapabilityFramework cf on cf.CapabilityId = cap.Id
join Framework frame on frame.Id = cf.FrameworkId
where frame.Id = '{frameworkId}'
";
        var retval = _dbConnection.Value.Query<Capability>(sql);
        return retval.AsQueryable();
      });
    }

    public Capability ById(string id)
    {
      return GetInternal(() =>
      {
        return _dbConnection.Value.Get<Capability>(id);
      });
    }

    public IQueryable<Capability> ByIds(IEnumerable<string> ids)
    {
      return GetInternal(() =>
      {
        var sqlIdsQuoted = ids.Select(id => $"'{id}'");
        var sqlIds = string.Join(',', sqlIdsQuoted);
        var sql = $@"
select * from Capability
where Id in ({sqlIds})";
        var retval = _dbConnection.Value.Query<Capability>(sql);
        return retval.AsQueryable();
      });
    }

    public IQueryable<Capability> ByStandard(string standardId, bool isOptional)
    {
      return GetInternal(() =>
      {
        var sql = $@"
select cap.* from Capability cap
join CapabilityStandard cs on cs.CapabilityId = cap.Id
join Standard std on std.Id = cs.StandardId
where std.Id = '{standardId}' and cs.IsOptional = {(isOptional ? 1 : 0).ToString()}
";
        var retval = _dbConnection.Value.Query<Capability>(sql);
        return retval.AsQueryable();
      });
    }

    public Capability Create(Capability capability)
    {
      return GetInternal(() =>
      {
        using (var trans = _dbConnection.Value.BeginTransaction())
        {
          capability.Id = Guid.NewGuid().ToString();
          _dbConnection.Value.Insert(capability, trans);
          trans.Commit();

          return capability;
        }
      });
    }

    public IQueryable<Capability> GetAll()
    {
      return GetInternal(() =>
      {
        return _dbConnection.Value.GetAll<Capability>().AsQueryable();
      });
    }

    public void Update(Capability capability)
    {
      GetInternal(() =>
      {
        using (var trans = _dbConnection.Value.BeginTransaction())
        {
          _dbConnection.Value.Update(capability, trans);
          trans.Commit();
          return 0;
        }
      });
    }
  }
}
