using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.Database.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Datastore.Database
{
  public sealed class OrganisationsDatastore : DatastoreBase<Organisations>, IOrganisationsDatastore
  {
    public OrganisationsDatastore(IDbConnectionFactory dbConnectionFactory, ILogger<OrganisationsDatastore> logger, ISyncPolicyFactory policy) :
      base(dbConnectionFactory, logger, policy)
    {
    }

    public Organisations ByODS(string odsCode)
    {
      return GetInternal(() =>
      {
        return _dbConnection.Value.GetAll<Organisations>().SingleOrDefault(org => org.OdsCode == odsCode);
      });
    }

    public Organisations ById(string id)
    {
      return GetInternal(() =>
      {
        return _dbConnection.Value.Get<Organisations>(id);
      });
    }

    public IQueryable<Organisations> GetAll()
    {
      return GetInternal(() =>
      {
        return _dbConnection.Value.GetAll<Organisations>().AsQueryable();
      });
    }

    public Organisations Create(Organisations org)
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

    public void Update(Organisations org)
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

    public void Delete(Organisations org)
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
