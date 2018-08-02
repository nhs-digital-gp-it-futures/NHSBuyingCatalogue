using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.CRM.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM
{
  public sealed class AssessmentMessageDatastore : DatastoreBase<AssessmentMessage>, IAssessmentMessageDatastore
  {
    public AssessmentMessageDatastore(
      IRestClientFactory crmConnectionFactory,
      ILogger<AssessmentMessageDatastore> logger,
      ISyncPolicyFactory policy) :
      base(crmConnectionFactory, logger, policy)
    {
    }

    public IQueryable<AssessmentMessage> BySolution(string solutionId)
    {
      throw new NotImplementedException();
    }

    public AssessmentMessage Create(AssessmentMessage assMess)
    {
      throw new NotImplementedException();
    }

    public void Delete(AssessmentMessage assMess)
    {
      throw new NotImplementedException();
    }

    public void Update(AssessmentMessage assMess)
    {
      throw new NotImplementedException();
    }
  }
}
