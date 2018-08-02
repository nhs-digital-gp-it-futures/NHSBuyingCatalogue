using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.Database.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.Datastore.Database
{
  public sealed class LinkManagerDatastore : DatastoreBase<object>, ILinkManagerDatastore
  {
    public LinkManagerDatastore(IDbConnectionFactory dbConnectionFactory, ILogger<LinkManagerDatastore> logger, ISyncPolicyFactory policy) :
      base(dbConnectionFactory, logger, policy)
    {
    }

    public void CapabilityFrameworkCreate(string frameworkId, string capabilityId)
    {
      GetInternal(() =>
      {
        using (var trans = _dbConnection.Value.BeginTransaction())
        {
          var entity = new CapabilityFramework { FrameworkId = frameworkId, CapabilityId = capabilityId };
          _dbConnection.Value.Insert(entity, trans);
          trans.Commit();
          return 0;
        }
      });
    }

    public void CapabilityStandardCreate(string capabilityId, string standardId, bool isOptional)
    {
      GetInternal(() =>
      {
        using (var trans = _dbConnection.Value.BeginTransaction())
        {
          var entity = new CapabilityStandard { CapabilityId = capabilityId, StandardId = standardId, IsOptional= isOptional };
          _dbConnection.Value.Insert(entity, trans);
          trans.Commit();
          return 0;
        }
      });
    }

    public void FrameworkSolutionCreate(string frameworkId, string solutionId)
    {
      GetInternal(() =>
      {
        using (var trans = _dbConnection.Value.BeginTransaction())
        {
          var entity = new FrameworkSolution { FrameworkId = frameworkId, SolutionId = solutionId };
          _dbConnection.Value.Insert(entity, trans);
          trans.Commit();
          return 0;
        }
      });
    }

    public void FrameworkStandardCreate(string frameworkId, string standardId)
    {
      GetInternal(() =>
      {
        using (var trans = _dbConnection.Value.BeginTransaction())
        {
          var entity = new FrameworkStandard { FrameworkId = frameworkId, StandardId = standardId };
          _dbConnection.Value.Insert(entity, trans);
          trans.Commit();
          return 0;
        }
      });
    }
  }
}
