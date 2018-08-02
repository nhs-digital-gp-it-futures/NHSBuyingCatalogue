using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.CRM.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM
{
  public sealed class AssessmentMessageContactDatastore : DatastoreBase<AssessmentMessageContact>, IAssessmentMessageContactDatastore
  {
    public AssessmentMessageContactDatastore(
      IRestClientFactory crmConnectionFactory,
      ILogger<AssessmentMessageContactDatastore> logger,
      ISyncPolicyFactory policy) :
      base(crmConnectionFactory, logger, policy)
    {
    }

    public IQueryable<AssessmentMessageContact> ByAssessmentMessage(string assMessId)
    {
      throw new NotImplementedException();
    }

    public AssessmentMessageContact Create(AssessmentMessageContact assMessCont)
    {
      throw new NotImplementedException();
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
