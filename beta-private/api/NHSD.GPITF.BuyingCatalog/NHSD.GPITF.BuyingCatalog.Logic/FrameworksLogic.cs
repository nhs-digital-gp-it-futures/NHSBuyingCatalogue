using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Collections.Generic;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class FrameworksLogic : LogicBase, IFrameworksLogic
  {
    private readonly IFrameworksDatastore _datastore;
    private readonly IFrameworksFilter _filter;

    public FrameworksLogic(
      IFrameworksDatastore datastore,
      IHttpContextAccessor context,
      IFrameworksFilter filter) :
      base(context)
    {
      _datastore = datastore;
      _filter = filter;
    }

    public IEnumerable<Frameworks> ByCapability(string capabilityId)
    {
      return _datastore.ByCapability(capabilityId);
    }

    public IEnumerable<Frameworks> ByStandard(string standardId)
    {
      return _datastore.ByStandard(standardId);
    }

    public Frameworks ById(string id)
    {
      return _datastore.ById(id);
    }

    public IEnumerable<Frameworks> BySolution(string solutionId)
    {
      return _filter.Filter(_datastore.BySolution(solutionId));
    }

    public IEnumerable<Frameworks> GetAll()
    {
      return _datastore.GetAll();
    }
  }
}
