using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.Database.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Datastore.Database
{
  public sealed class FrameworkDatastore : DatastoreBase<Framework>, IFrameworkDatastore
  {
    public FrameworkDatastore(IDbConnectionFactory dbConnectionFactory, ILogger<FrameworkDatastore> logger, ISyncPolicyFactory policy) :
      base(dbConnectionFactory, logger, policy)
    {
    }

    public IQueryable<Framework> ByCapability(string capabilityId)
    {
      return GetInternal(() =>
      {
        var sql = $@"
select frame.* from Framework frame
join CapabilityFramework cf on cf.FrameworkId = frame.Id
join Capability cap on cap.Id = cf.CapabilityId
where cap.Id = '{capabilityId}'
";
        var retval = _dbConnection.Value.Query<Framework>(sql);
        return retval.AsQueryable();
      });
    }

    public Framework ById(string id)
    {
      return GetInternal(() =>
      {
        return _dbConnection.Value.Get<Framework>(id);
      });
    }

    public IQueryable<Framework> BySolution(string solutionId)
    {
      return GetInternal(() =>
      {
        var sql = $@"
select frame.* from Framework frame
join FrameworkSolution fs on fs.FrameworkId = frame.Id
join Solution soln on soln.Id = fs.SolutionId
where soln.Id = '{solutionId}'
";
        var retval = _dbConnection.Value.Query<Framework>(sql);
        return retval.AsQueryable();
      });
    }

    public IQueryable<Framework> ByStandard(string standardId)
    {
      return GetInternal(() =>
      {
        var sql = $@"
select frame.* from Framework frame
join FrameworkStandard fs on fs.FrameworkId = frame.Id
join Standard std on std.Id = fs.StandardId
where std.Id = '{standardId}'
";
        var retval = _dbConnection.Value.Query<Framework>(sql);
        return retval.AsQueryable();
      });
    }

    public Framework Create(Framework framework)
    {
      return GetInternal(() =>
      {
        using (var trans = _dbConnection.Value.BeginTransaction())
        {
          framework.Id = Guid.NewGuid().ToString();
          _dbConnection.Value.Insert(framework, trans);
          trans.Commit();

          return framework;
        }
      });
    }

    public IQueryable<Framework> GetAll()
    {
      return GetInternal(() =>
      {
        return _dbConnection.Value.GetAll<Framework>().AsQueryable();
      });
    }

    public void Update(Framework framework)
    {
      GetInternal(() =>
      {
        using (var trans = _dbConnection.Value.BeginTransaction())
        {
          _dbConnection.Value.Update(framework, trans);
          trans.Commit();
          return 0;
        }
      });
    }
  }
}
