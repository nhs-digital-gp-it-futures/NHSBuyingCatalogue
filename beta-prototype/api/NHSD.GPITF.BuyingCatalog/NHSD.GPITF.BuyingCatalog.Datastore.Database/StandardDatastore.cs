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
  public sealed class StandardDatastore : DatastoreBase<Standard>, IStandardDatastore
  {
    public StandardDatastore(IDbConnectionFactory dbConnectionFactory, ILogger<StandardDatastore> logger, ISyncPolicyFactory policy) :
      base(dbConnectionFactory, logger, policy)
    {
    }

    public IQueryable<Standard> ByCapability(string capabilityId, bool isOptional)
    {
      return GetInternal(() =>
      {
        var sql = $@"
select std.* from Standard std
join CapabilityStandard cs on cs.StandardId = std.Id
join Capability cap on cap.Id = cs.CapabilityId
where cap.Id = '{capabilityId}' and cs.IsOptional = {(isOptional ? 1 : 0).ToString()}
";
        var retval = _dbConnection.Value.Query<Standard>(sql);
        return retval.AsQueryable();
      });
    }

    public IQueryable<Standard> ByFramework(string frameworkId)
    {
      return GetInternal(() =>
      {
        var sql = $@"
select std.* from Standard std
join FrameworkStandard fs on fs.StandardId = std.Id
join Framework frame on frame.Id = fs.FrameworkId
where frame.Id = '{frameworkId}'
";
        var retval = _dbConnection.Value.Query<Standard>(sql);
        return retval.AsQueryable();
      });
    }

    public Standard ById(string id)
    {
      return GetInternal(() =>
      {
        return _dbConnection.Value.Get<Standard>(id);
      });
    }

    public IQueryable<Standard> ByIds(IEnumerable<string> ids)
    {
      return GetInternal(() =>
      {
        var sqlIdsQuoted = ids.Select(id => $"'{id}'");
        var sqlIds = string.Join(',', sqlIdsQuoted);
        var sql = $@"
select * from Standard
where Id in ({sqlIds})";
        var retval = _dbConnection.Value.Query<Standard>(sql);
        return retval.AsQueryable();
      });
    }

    public Standard Create(Standard standard)
    {
      return GetInternal(() =>
      {
        using (var trans = _dbConnection.Value.BeginTransaction())
        {
          standard.Id = Guid.NewGuid().ToString();
          _dbConnection.Value.Insert(standard, trans);
          trans.Commit();

          return standard;
        }
      });
    }

    public IQueryable<Standard> GetAll()
    {
      return GetInternal(() =>
      {
        return _dbConnection.Value.GetAll<Standard>().AsQueryable();
      });
    }

    public void Update(Standard standard)
    {
      GetInternal(() =>
      {
        using (var trans = _dbConnection.Value.BeginTransaction())
        {
          _dbConnection.Value.Update(standard, trans);
          trans.Commit();
          return 0;
        }
      });
    }
  }
}
