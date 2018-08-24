using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.Database.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Datastore.Database
{
  public sealed class FrameworksDatastore : DatastoreBase<Frameworks>, IFrameworksDatastore
  {
    public FrameworksDatastore(IDbConnectionFactory dbConnectionFactory, ILogger<FrameworksDatastore> logger, ISyncPolicyFactory policy) :
      base(dbConnectionFactory, logger, policy)
    {
    }

    public IQueryable<Frameworks> ByCapability(string capabilityId)
    {
      return GetInternal(() =>
      {
        var sql = $@"
select frame.* from Frameworks frame
join CapabilityFramework cf on cf.FrameworkId = frame.Id
join Capabilities cap on cap.Id = cf.CapabilityId
where cap.Id = '{capabilityId}'
";
        var retval = _dbConnection.Value.Query<Frameworks>(sql);
        return retval.AsQueryable();
      });
    }

    public Frameworks ById(string id)
    {
      return GetInternal(() =>
      {
        return _dbConnection.Value.Get<Frameworks>(id);
      });
    }

    public IQueryable<Frameworks> BySolution(string solutionId)
    {
      return GetInternal(() =>
      {
        var sql = $@"
select frame.* from Frameworks frame
join FrameworkSolution fs on fs.FrameworkId = frame.Id
join Solutions soln on soln.Id = fs.SolutionId
where soln.Id = '{solutionId}'
";
        var retval = _dbConnection.Value.Query<Frameworks>(sql);
        return retval.AsQueryable();
      });
    }

    public IQueryable<Frameworks> ByStandard(string standardId)
    {
      return GetInternal(() =>
      {
        var sql = $@"
select frame.* from Frameworks frame
join FrameworkStandard fs on fs.FrameworkId = frame.Id
join Standards std on std.Id = fs.StandardId
where std.Id = '{standardId}'
";
        var retval = _dbConnection.Value.Query<Frameworks>(sql);
        return retval.AsQueryable();
      });
    }

    public IQueryable<Frameworks> GetAll()
    {
      return GetInternal(() =>
      {
        return _dbConnection.Value.GetAll<Frameworks>().AsQueryable();
      });
    }
  }
}
