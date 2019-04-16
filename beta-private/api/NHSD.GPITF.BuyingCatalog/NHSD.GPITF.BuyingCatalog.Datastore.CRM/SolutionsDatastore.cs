using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Collections.Generic;
using System.Linq;
using GifInt = Gif.Service.Contracts;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM
{
  public sealed class SolutionsDatastore : CrmDatastoreBase<Solutions>, ISolutionsDatastore
  {
    private readonly GifInt.ISolutionsDatastore _crmDatastore;

    public SolutionsDatastore(
      GifInt.ISolutionsDatastore crmDatastore,
      ILogger<SolutionsDatastore> logger,
      ISyncPolicyFactory policy) :
      base(logger, policy)
    {
      _crmDatastore = crmDatastore;
    }

    public IEnumerable<Solutions> ByFramework(string frameworkId)
    {
      return GetInternal(() =>
      {
        var vals = _crmDatastore
          .ByFramework(frameworkId)
          .Select(val => Creator.FromCrm(val));

        return vals;
      });
    }

    public Solutions ById(string id)
    {
      return GetInternal(() =>
      {
        var val = _crmDatastore
          .ById(id);

        return Creator.FromCrm(val);
      });
    }

    public IEnumerable<Solutions> ByOrganisation(string organisationId)
    {
      return GetInternal(() =>
      {
        var vals = _crmDatastore
          .ByOrganisation(organisationId)
          .Select(val => Creator.FromCrm(val));

        return vals;
      });
    }

    public Solutions Create(Solutions solution)
    {
      return GetInternal(() =>
      {
        solution.Id = UpdateId(solution.Id);

        var val = _crmDatastore
          .Create(Creator.FromApi(solution));

        return Creator.FromCrm(val);
      });
    }

    public void Update(Solutions solution)
    {
      GetInternal(() =>
      {
        _crmDatastore.Update(Creator.FromApi(solution));

        return 0;
      });
    }

    public void Delete(Solutions solution)
    {
      GetInternal(() =>
      {
        _crmDatastore.Delete(Creator.FromApi(solution));

        return 0;
      });
    }
  }
}
