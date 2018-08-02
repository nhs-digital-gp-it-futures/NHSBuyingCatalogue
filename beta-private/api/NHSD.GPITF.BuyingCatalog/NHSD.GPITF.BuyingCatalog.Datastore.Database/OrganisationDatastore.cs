using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.Database.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Datastore.Database
{
  public sealed class OrganisationDatastore : DatastoreBase<Organisation>, IOrganisationDatastore
  {
    public OrganisationDatastore(IDbConnectionFactory dbConnectionFactory, ILogger<OrganisationDatastore> logger, ISyncPolicyFactory policy) :
      base(dbConnectionFactory, logger, policy)
    {
    }

    public Organisation ByODS(string odsCode)
    {
      return GetInternal(() =>
      {
        return _dbConnection.Value.GetAll<Organisation>().SingleOrDefault(org => org.OdsCode == odsCode);
      });
    }

    public Organisation ById(string id)
    {
      return GetInternal(() =>
      {
        return _dbConnection.Value.Get<Organisation>(id);
      });
    }

    public IQueryable<Organisation> GetAll()
    {
      return GetInternal(() =>
      {
        return _dbConnection.Value.GetAll<Organisation>().AsQueryable();
      });
    }

    public Organisation Create(Organisation org)
    {
      return GetInternal(() =>
      {
        using (var trans = _dbConnection.Value.BeginTransaction())
        {
          org.Id = Guid.NewGuid().ToString();
          _dbConnection.Value.Insert(org, trans);
          trans.Commit();

          return org;
        }
      });
    }

    public void Update(Organisation org)
    {
      GetInternal(() =>
      {
        using (var trans = _dbConnection.Value.BeginTransaction())
        {
          _dbConnection.Value.Update(org, trans);
          trans.Commit();

          return 0;
        }
      });
    }

    public void Delete(Organisation org)
    {
      GetInternal(() =>
      {
        using (var trans = _dbConnection.Value.BeginTransaction())
        {
          _dbConnection.Value.Delete(org, trans);
          trans.Commit();

          return 0;
        }
      });
    }
  }
}
