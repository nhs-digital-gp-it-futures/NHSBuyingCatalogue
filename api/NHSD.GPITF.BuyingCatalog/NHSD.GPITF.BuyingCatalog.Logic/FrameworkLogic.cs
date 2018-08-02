using FluentValidation;
using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class FrameworkLogic : LogicBase, IFrameworkLogic
  {
    private readonly IFrameworkDatastore _datastore;
    private readonly IFrameworkValidator _validator;
    private readonly IFrameworkFilter _filter;

    public FrameworkLogic(
      IFrameworkDatastore datastore,
      IHttpContextAccessor context,
      IFrameworkValidator validator,
      IFrameworkFilter filter) :
      base(context)
    {
      _datastore = datastore;
      _validator = validator;
      _filter = filter;
    }

    public IQueryable<Framework> ByCapability(string capabilityId)
    {
      return _datastore.ByCapability(capabilityId);
    }

    public IQueryable<Framework> ByStandard(string standardId)
    {
      return _datastore.ByStandard(standardId);
    }

    public Framework ById(string id)
    {
      return _datastore.ById(id);
    }

    public IQueryable<Framework> BySolution(string solutionId)
    {
      return _filter.Filter(_datastore.BySolution(solutionId));
    }

    public Framework Create(Framework framework)
    {
      _validator.ValidateAndThrow(framework, ruleSet: nameof(IFrameworkLogic.Create));
      return _datastore.Create(framework);
    }

    public IQueryable<Framework> GetAll()
    {
      return _datastore.GetAll();
    }

    public void Update(Framework framework)
    {
      _validator.ValidateAndThrow(framework, ruleSet: nameof(IFrameworkLogic.Create));
      _datastore.Update(framework);
    }
  }
}
