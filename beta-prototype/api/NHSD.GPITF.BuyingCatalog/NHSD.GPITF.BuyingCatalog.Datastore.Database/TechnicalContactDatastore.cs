using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.Database.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Datastore.Database
{
  public sealed class TechnicalContactDatastore : DatastoreBase<TechnicalContact>, ITechnicalContactDatastore
  {
    public TechnicalContactDatastore(IDbConnectionFactory dbConnectionFactory, ILogger<TechnicalContactDatastore> logger, ISyncPolicyFactory policy) :
      base(dbConnectionFactory, logger, policy)
    {
    }

    public IQueryable<TechnicalContact> BySolution(string solutionId)
    {
      return GetInternal(() =>
      {
        return _dbConnection.Value.GetAll<TechnicalContact>().Where(tc => tc.SolutionId == solutionId).AsQueryable();
      });
    }

    public TechnicalContact Create(TechnicalContact techCont)
    {
      return GetInternal(() =>
      {
        using (var trans = _dbConnection.Value.BeginTransaction())
        {
          techCont.Id = Guid.NewGuid().ToString();
          _dbConnection.Value.Insert(techCont, trans);
          trans.Commit();

          return techCont;
        }
      });
    }

    public void Update(TechnicalContact techCont)
    {
      GetInternal(() =>
      {
        using (var trans = _dbConnection.Value.BeginTransaction())
        {
          _dbConnection.Value.Update(techCont, trans);
          trans.Commit();
          return 0;
        }
      });
    }

    public void Delete(TechnicalContact techCont)
    {
      GetInternal(() =>
      {
        using (var trans = _dbConnection.Value.BeginTransaction())
        {
          _dbConnection.Value.Delete(techCont, trans);
          trans.Commit();
          return 0;
        }
      });
    }
  }
}
