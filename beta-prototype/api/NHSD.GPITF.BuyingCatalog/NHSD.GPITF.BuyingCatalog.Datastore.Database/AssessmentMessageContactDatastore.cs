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
  public sealed class AssessmentMessageContactDatastore : DatastoreBase<AssessmentMessageContact>, IAssessmentMessageContactDatastore
  {
    public AssessmentMessageContactDatastore(IDbConnectionFactory dbConnectionFactory, ILogger<AssessmentMessageContactDatastore> logger, ISyncPolicyFactory policy) :
      base(dbConnectionFactory, logger, policy)
    {
    }

    public IQueryable<AssessmentMessageContact> ByAssessmentMessage(string assMessId)
    {
      return GetInternal(() =>
      {
        var sql = $@"
select * from AssessmentMessageContact
where AssessmentMessageId = '{assMessId}'
";
        var retval = _dbConnection.Value.Query<AssessmentMessageContact>(sql);
        return retval.AsQueryable();
      });
    }

    public AssessmentMessageContact Create(AssessmentMessageContact assMessCont)
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

    public void Delete(AssessmentMessageContact assMessCont)
    {
      throw new NotImplementedException();
    }

    public void Update(AssessmentMessageContact assMessCont)
    {
      throw new NotImplementedException();
    }
  }
}
