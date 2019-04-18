using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces.Porcelain;
using NHSD.GPITF.BuyingCatalog.Models;
using NHSD.GPITF.BuyingCatalog.Models.Porcelain;
using System;
using System.Collections.Generic;
using System.Linq;
using GifInt = Gif.Service.Contracts;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM
{
  public sealed class SolutionsDatastore : ShortTermCachedDatastore<Solutions>, ISolutionsDatastore
  {
    private readonly GifInt.ISolutionsDatastore _crmDatastore;

    public SolutionsDatastore(
      GifInt.ISolutionsDatastore crmDatastore,
      ILogger<SolutionsDatastore> logger,
      ISyncPolicyFactory policy,
      IConfiguration config,
      IShortTermCache cache,
      IServiceProvider serviceProvider) :
      base(logger, policy, config, cache, serviceProvider)
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
        return GetFromCache(GetCachePathById(id), id);
      });
    }

    public IEnumerable<Solutions> ByOrganisation(string organisationId)
    {
      return GetInternal(() =>
      {
        return GetAllFromCache(GetCachePathByOrganisation(organisationId), organisationId);
      });
    }

    public Solutions Create(Solutions solution)
    {
      return GetInternal(() =>
      {
        solution.Id = UpdateId(solution.Id);

        var val = _crmDatastore
          .Create(Creator.FromApi(solution));
        ExpireCache(solution);

        return Creator.FromCrm(val);
      });
    }

    public void Update(Solutions solution)
    {
      GetInternal(() =>
      {
        _crmDatastore.Update(Creator.FromApi(solution));
        ExpireCache(solution);

        return 0;
      });
    }

    public void Delete(Solutions solution)
    {
      GetInternal(() =>
      {
        _crmDatastore.Delete(Creator.FromApi(solution));
        ExpireCache(solution);

        return 0;
      });
    }

    protected override Solutions GetFromSource(string path, string parameter)
    {
      if (path == GetCachePathById(parameter))
      {
        return GetFromSourceById(parameter);
      }

      throw new ArgumentOutOfRangeException($"{nameof(path)}", path, "Unsupported cache path");
    }

    private Solutions GetFromSourceById(string id)
    {
      var val = _crmDatastore
        .ById(id);

      return Creator.FromCrm(val);
    }

    private IEnumerable<Solutions> GetFromSourceByOrganisation(string organisationId)
    {
      var vals = _crmDatastore
        .ByOrganisation(organisationId)
        .Select(val => Creator.FromCrm(val));

      return vals;
    }

    protected override IEnumerable<Solutions> GetAllFromSource(string path, string parameter)
    {
      if (path == GetCachePathByOrganisation(parameter))
      {
        return GetFromSourceByOrganisation(parameter);
      }

      throw new ArgumentOutOfRangeException($"{nameof(path)}", path, "Unsupported cache path");
    }

    private void ExpireCache(Solutions solution)
    {
      // expire our cache
      ExpireValue(GetCachePathById(solution.Id));
      ExpireValue(GetCachePathByOrganisation(solution.OrganisationId));

      // expire SolutionsEx cache
      var other = _serviceProvider.GetService<ISolutionsExDatastore>() as IOtherCache;
      other?.ExpireOtherValue(solution);
    }

    private static string GetCachePathById(string id)
    {
      return $"/{nameof(Solutions)}/{nameof(ById)}/{id}";
    }

    private static string GetCachePathByOrganisation(string organisationId)
    {
      return $"/{nameof(Solutions)}/{nameof(ByOrganisation)}/{organisationId}";
    }

    public override void ExpireOtherValue(object item)
    {
      if (item as SolutionEx != null)
      {
        ExpireCache(((SolutionEx)item).Solution);
      }

      throw new ArgumentOutOfRangeException($"{nameof(item)}", item.GetType(), "Unsupported cache expiry type");
    }
  }
}
