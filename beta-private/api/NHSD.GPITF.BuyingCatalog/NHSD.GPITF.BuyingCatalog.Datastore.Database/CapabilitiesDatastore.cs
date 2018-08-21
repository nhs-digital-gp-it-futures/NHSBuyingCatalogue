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
  public sealed class CapabilitiesDatastore : DatastoreBase<Capabilities>, ICapabilitiesDatastore
  {
    public CapabilitiesDatastore(IDbConnectionFactory dbConnectionFactory, ILogger<CapabilitiesDatastore> logger, ISyncPolicyFactory policy) :
      base(dbConnectionFactory, logger, policy)
    {
    }

    public IQueryable<Capabilities> ByFramework(string frameworkId)
    {
      return GetInternal(() =>
      {
        var sql = $@"
select cap.* from Capabilities cap
join CapabilityFramework cf on cf.CapabilityId = cap.Id
join Frameworks frame on frame.Id = cf.FrameworkId
where frame.Id = '{frameworkId}'
";
        var retval = _dbConnection.Value.Query<Capabilities>(sql);
        return retval.AsQueryable();
      });
    }

    public Capabilities ById(string id)
    {
      return GetInternal(() =>
      {
        return _dbConnection.Value.Get<Capabilities>(id);
      });
    }

    public IQueryable<Capabilities> ByIds(IEnumerable<string> ids)
    {
      return GetInternal(() =>
      {
        var sqlIdsQuoted = ids.Select(id => $"'{id}'");
        var sqlIds = string.Join(',', sqlIdsQuoted);
        var sql = $@"
select * from Capabilities
where Id in ({sqlIds})";
        var retval = _dbConnection.Value.Query<Capabilities>(sql);
        return retval.AsQueryable();
      });
    }

    public IQueryable<Capabilities> ByStandard(string standardId, bool isOptional)
    {
      return GetInternal(() =>
      {
        var sql = $@"
select cap.* from Capabilities cap
join CapabilityStandard cs on cs.CapabilityId = cap.Id
join Standards std on std.Id = cs.StandardId
where std.Id = '{standardId}' and cs.IsOptional = {(isOptional ? 1 : 0).ToString()}
";
        var retval = _dbConnection.Value.Query<Capabilities>(sql);
        return retval.AsQueryable();
      });
    }

    public Capabilities Create(Capabilities capability)
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

    public IQueryable<Capabilities> GetAll()
    {
      return GetInternal(() =>
      {
        return _dbConnection.Value.GetAll<Capabilities>().AsQueryable();
      });
    }

    public void Update(Capabilities capability)
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
