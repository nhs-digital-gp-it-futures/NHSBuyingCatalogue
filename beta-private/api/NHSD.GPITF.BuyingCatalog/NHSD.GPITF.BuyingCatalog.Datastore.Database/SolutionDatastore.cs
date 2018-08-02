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
  public sealed class SolutionDatastore : DatastoreBase<Solution>, ISolutionDatastore
  {
    public SolutionDatastore(IDbConnectionFactory dbConnectionFactory, ILogger<SolutionDatastore> logger, ISyncPolicyFactory policy) :
      base(dbConnectionFactory, logger, policy)
    {
    }

    public IQueryable<Solution> ByFramework(string frameworkId)
    {
      return GetInternal(() =>
      {
        var sql = $@"
select soln.* from Solution soln
join FrameworkSolution fs on fs.SolutionId = soln.Id
join Framework frame on frame.Id = fs.FrameworkId
where frame.Id = '{frameworkId}'
";
        var retval = _dbConnection.Value.Query<Solution>(sql);
        return retval.AsQueryable();
      });
    }

    public Solution ById(string id)
    {
      return GetInternal(() =>
      {
        return _dbConnection.Value.Get<Solution>(id);
      });
    }

    public IQueryable<Solution> ByOrganisation(string organisationId)
    {
      return GetInternal(() =>
      {
        return _dbConnection.Value.GetAll<Solution>().Where(soln => soln.OrganisationId == organisationId).AsQueryable();
      });
    }

    public Solution Create(Solution solution)
    {
      return GetInternal(() =>
      {
        using (var trans = _dbConnection.Value.BeginTransaction())
        {
          solution.Id = Guid.NewGuid().ToString();
          _dbConnection.Value.Insert(solution, trans);
          trans.Commit();

          return solution;
        }
      });
    }

    public void Update(Solution solution)
    {
      GetInternal(() =>
      {
        using (var trans = _dbConnection.Value.BeginTransaction())
        {
          _dbConnection.Value.Update(solution, trans);
          trans.Commit();
          return 0;
        }
      });
    }

    public void Delete(Solution solution)
    {
      GetInternal(() =>
      {
        using (var trans = _dbConnection.Value.BeginTransaction())
        {
          _dbConnection.Value.Delete(solution, trans);
          trans.Commit();
          return 0;
        }
      });
    }
  }
}
