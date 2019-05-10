using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using System;
using GifInt = Gif.Service.Contracts;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM
{
  public sealed class LinkManagerDatastore : CrmDatastoreBase<object>, ILinkManagerDatastore
  {
    private readonly GifInt.ILinkManagerDatastore _crmDatastore;

    public LinkManagerDatastore(
      GifInt.ILinkManagerDatastore crmDatastore,
      ILogger<LinkManagerDatastore> logger,
      ISyncPolicyFactory policy) :
      base(logger, policy)
    {
      _crmDatastore = crmDatastore;
    }

    public void FrameworkSolutionCreate(string frameworkId, string solutionId)
    {
      GetInternal(() =>
      {
        _crmDatastore.FrameworkSolutionAssociate(Guid.Parse(frameworkId), Guid.Parse(solutionId));

        return 0;
      });
    }
  }
}
