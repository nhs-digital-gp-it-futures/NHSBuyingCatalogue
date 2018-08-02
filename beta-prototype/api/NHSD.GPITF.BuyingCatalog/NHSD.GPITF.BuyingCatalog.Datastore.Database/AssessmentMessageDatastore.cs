using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.Database.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Datastore.Database
{
  public sealed class AssessmentMessageDatastore : DatastoreBase<AssessmentMessage>, IAssessmentMessageDatastore
  {
    public AssessmentMessageDatastore(IDbConnectionFactory dbConnectionFactory, ILogger<AssessmentMessageDatastore> logger, ISyncPolicyFactory policy) :
      base(dbConnectionFactory, logger, policy)
    {
    }

    public IQueryable<AssessmentMessage> BySolution(string solutionId)
    {
      return GetInternal(() =>
      {
        return _dbConnection.Value.GetAll<AssessmentMessage>().Where(am => am.SolutionId == solutionId).AsQueryable();
      });
    }

    public AssessmentMessage Create(AssessmentMessage assMess)
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

    public void Delete(AssessmentMessage assMess)
    {
      // TODO   manually handle delete for MS SQL Server due to cyclic foreign key cascading delete
      throw new NotImplementedException();
    }

    public void Update(AssessmentMessage assMess)
    {
      throw new NotImplementedException();
    }
  }
}
