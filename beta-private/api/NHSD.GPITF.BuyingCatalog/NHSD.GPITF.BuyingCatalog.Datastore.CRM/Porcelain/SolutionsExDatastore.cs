using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces.Porcelain;
using NHSD.GPITF.BuyingCatalog.Models.Porcelain;
using System.Collections.Generic;
using System.Linq;
using GifInt = Gif.Service.Contracts;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM.Porcelain
{
  public sealed class SolutionsExDatastore : CrmDatastoreBase<SolutionEx>, ISolutionsExDatastore
  {
    private readonly GifInt.ISolutionsExDatastore _crmDatastore;

    public SolutionsExDatastore(
      GifInt.ISolutionsExDatastore crmDatastore,
      ILogger<SolutionsExDatastore> logger,
      ISyncPolicyFactory policy) :
      base(logger, policy)
    {
      _crmDatastore = crmDatastore;
    }

    public SolutionEx BySolution(string solutionId)
    {
      return GetInternal(() =>
      {
        var val = _crmDatastore
          .BySolution(solutionId);

        return Creator.FromCrm(val);
      });
    }

    public void Update(SolutionEx solnEx)
    {
      GetInternal(() =>
      {
        _crmDatastore.Update(Creator.FromApi(solnEx));

        return 0;
      });
    }

    public IEnumerable<SolutionEx> ByOrganisation(string organisationId)
    {
      return GetInternal(() =>
      {
        var vals = _crmDatastore
          .ByOrganisation(organisationId)
          .Select(val => Creator.FromCrm(val));

        return vals;
      });
    }
  }
}
