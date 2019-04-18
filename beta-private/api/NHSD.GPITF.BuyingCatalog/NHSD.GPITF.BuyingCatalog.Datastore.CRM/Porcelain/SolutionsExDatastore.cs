using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces.Porcelain;
using NHSD.GPITF.BuyingCatalog.Models;
using NHSD.GPITF.BuyingCatalog.Models.Porcelain;
using System;
using System.Collections.Generic;
using System.Linq;
using GifInt = Gif.Service.Contracts;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM.Porcelain
{
  public sealed class SolutionsExDatastore : ShortTermCachedDatastore<SolutionEx>, ISolutionsExDatastore
  {
    private readonly GifInt.ISolutionsExDatastore _crmDatastore;

    public SolutionsExDatastore(
      GifInt.ISolutionsExDatastore crmDatastore,
      ILogger<SolutionsExDatastore> logger,
      ISyncPolicyFactory policy,
      IConfiguration config,
      IShortTermCache cache) :
      base(logger, policy, config, cache)
    {
      _crmDatastore = crmDatastore;
    }

    public SolutionEx BySolution(string solutionId)
    {
      return GetInternal(() =>
      {
        return GetFromCache(GetCachePathBySolution(solutionId), solutionId);
      });
    }

    public void Update(SolutionEx solnEx)
    {
      GetInternal(() =>
      {
        _crmDatastore.Update(Creator.FromApi(solnEx));
        ExpireCache(solnEx);

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

    protected override SolutionEx GetFromSource(string path, string parameter)
    {
      if (path == GetCachePathBySolution(parameter))
      {
        return GetFromSourceBySolution(parameter);
      }

      throw new ArgumentOutOfRangeException($"{nameof(path)}", path, "Unsupported cache path");
    }

    private SolutionEx GetFromSourceBySolution(string solutionId)
    {
      var val = _crmDatastore
        .BySolution(solutionId);

      return Creator.FromCrm(val);
    }

    protected override IEnumerable<SolutionEx> GetAllFromSource(string path, string parameter = null)
    {
      throw new NotImplementedException();
    }

    private void ExpireCache(SolutionEx solnEx)
    {
      ExpireCache(solnEx.Solution);
    }

    private void ExpireCache(Solutions soln)
    {
      // TODO   expire Solutions cache
      ExpireValue(GetCachePathBySolution(soln.Id));
    }

    private static string GetCachePathBySolution(string solutionId)
    {
      return $"/{nameof(SolutionEx)}/{nameof(BySolution)}/{solutionId}";
    }
  }
}
