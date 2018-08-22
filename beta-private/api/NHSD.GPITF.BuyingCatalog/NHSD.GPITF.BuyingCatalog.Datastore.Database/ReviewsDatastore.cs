using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.Database.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Datastore.Database
{
  public sealed class ReviewsDatastore : DatastoreBase<Reviews>, IReviewsDatastore
  {
    public ReviewsDatastore(IDbConnectionFactory dbConnectionFactory, ILogger<ReviewsDatastore> logger, ISyncPolicyFactory policy) :
      base(dbConnectionFactory, logger, policy)
    {
    }

    public IQueryable<Reviews> BySolution(string solutionId)
    {
      return GetInternal(() =>
      {
        return _dbConnection.Value.GetAll<Reviews>().Where(am => am.SolutionId == solutionId).AsQueryable();
      });
    }

    public Reviews Create(Reviews assMess)
    {
      return GetInternal(() =>
      {
        using (var trans = _dbConnection.Value.BeginTransaction())
        {
          assMess.Id = Guid.NewGuid().ToString();
          assMess.Timestamp = DateTime.UtcNow;
          _dbConnection.Value.Insert(assMess, trans);
          trans.Commit();

          return assMess;
        }
      });
    }

    public void Delete(Reviews assMess)
    {
      throw new NotImplementedException();
    }

    public void Update(Reviews assMess)
    {
      throw new NotImplementedException();
    }
  }
}
