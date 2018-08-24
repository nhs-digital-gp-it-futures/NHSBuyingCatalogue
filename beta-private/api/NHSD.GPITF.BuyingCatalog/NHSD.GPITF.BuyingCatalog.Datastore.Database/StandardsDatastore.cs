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
  public sealed class StandardsDatastore : DatastoreBase<Standards>, IStandardsDatastore
  {
    public StandardsDatastore(IDbConnectionFactory dbConnectionFactory, ILogger<StandardsDatastore> logger, ISyncPolicyFactory policy) :
      base(dbConnectionFactory, logger, policy)
    {
    }

    public IQueryable<Standards> ByCapability(string capabilityId, bool isOptional)
    {
      return GetInternal(() =>
      {
        var sql = $@"
select std.* from Standards std
join CapabilityStandard cs on cs.StandardId = std.Id
join Capabilities cap on cap.Id = cs.CapabilityId
where cap.Id = '{capabilityId}' and cs.IsOptional = {(isOptional ? 1 : 0).ToString()}
";
        var retval = _dbConnection.Value.Query<Standards>(sql);
        return retval.AsQueryable();
      });
    }

    public IQueryable<Standards> ByFramework(string frameworkId)
    {
      return GetInternal(() =>
      {
        var sql = $@"
select std.* from Standards std
join FrameworkStandard fs on fs.StandardId = std.Id
join Frameworks frame on frame.Id = fs.FrameworkId
where frame.Id = '{frameworkId}'
";
        var retval = _dbConnection.Value.Query<Standards>(sql);
        return retval.AsQueryable();
      });
    }

    public Standards ById(string id)
    {
      return GetInternal(() =>
      {
        return _dbConnection.Value.Get<Standards>(id);
      });
    }

    public IQueryable<Standards> ByIds(IEnumerable<string> ids)
    {
      return GetInternal(() =>
      {
        var sqlIdsQuoted = ids.Select(id => $"'{id}'");
        var sqlIds = string.Join(',', sqlIdsQuoted);
        var sql = $@"
select * from Standards
where Id in ({sqlIds})";
        var retval = _dbConnection.Value.Query<Standards>(sql);
        return retval.AsQueryable();
      });
    }

    public Standards Create(Standards standard)
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

    public IQueryable<Standards> GetAll()
    {
      return GetInternal(() =>
      {
        var sql = @"
-- select all current versions
select * from Standards where Id not in 
(
  select PreviousId from Standards where PreviousId is not null
)
";
        return _dbConnection.Value.Query<Standards>(sql).AsQueryable();
      });
    }

    public void Update(Standards standard)
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
