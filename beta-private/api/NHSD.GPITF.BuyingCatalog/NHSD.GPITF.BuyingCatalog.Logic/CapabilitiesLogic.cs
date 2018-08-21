using FluentValidation;
using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Collections.Generic;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class CapabilitiesLogic : LogicBase, ICapabilitiesLogic
  {
    private readonly ICapabilitiesDatastore _datastore;
    private readonly ICapabilitiesValidator _validator;

    public CapabilitiesLogic(
      ICapabilitiesDatastore datastore, 
      IHttpContextAccessor context,
      ICapabilitiesValidator validator) :
      base(context)
    {
      _datastore = datastore;
      _validator = validator;
    }

    public IQueryable<Capabilities> ByFramework(string frameworkId)
    {
      return _datastore.ByFramework(frameworkId);
    }

    public Capabilities ById(string id)
    {
      return _datastore.ById(id);
    }

    public IQueryable<Capabilities> ByIds(IEnumerable<string> ids)
    {
      return _datastore.ByIds(ids);
    }

    public IQueryable<Capabilities> ByStandard(string standardId, bool isOptional)
    {
      return _datastore.ByStandard(standardId, isOptional);
    }

    public Capabilities Create(Capabilities capability)
    {
      _validator.ValidateAndThrow(capability, ruleSet: nameof(ICapabilitiesLogic.Create));
      return _datastore.Create(capability);
    }

    public IQueryable<Capabilities> GetAll()
    {
      return _datastore.GetAll();
    }

    public void Update(Capabilities capability)
    {
      _validator.ValidateAndThrow(capability, ruleSet: nameof(ICapabilitiesLogic.Update));
      _datastore.Update(capability);
    }
  }
}
