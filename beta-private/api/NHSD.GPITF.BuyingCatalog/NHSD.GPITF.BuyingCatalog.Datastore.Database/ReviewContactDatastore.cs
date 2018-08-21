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
  public sealed class ReviewContactDatastore : DatastoreBase<ReviewContact>, IReviewContactDatastore
  {
    public ReviewContactDatastore(IDbConnectionFactory dbConnectionFactory, ILogger<ReviewContactDatastore> logger, ISyncPolicyFactory policy) :
      base(dbConnectionFactory, logger, policy)
    {
    }

    public IQueryable<ReviewContact> ByAssessmentMessage(string assMessId)
    {
      return GetInternal(() =>
      {
        var sql = $@"
select * from ReviewContact
where AssessmentMessageId = '{assMessId}'
";
        var retval = _dbConnection.Value.Query<ReviewContact>(sql);
        return retval.AsQueryable();
      });
    }

    public ReviewContact Create(ReviewContact assMessCont)
    {
      return GetInternal(() =>
      {
        using (var trans = _dbConnection.Value.BeginTransaction())
        {
          _dbConnection.Value.Insert(assMessCont, trans);
          trans.Commit();

          return assMessCont;
        }
      });
    }

    public void Delete(ReviewContact assMessCont)
    {
      throw new NotImplementedException();
    }

    public void Update(ReviewContact assMessCont)
    {
      throw new NotImplementedException();
    }
  }
}
