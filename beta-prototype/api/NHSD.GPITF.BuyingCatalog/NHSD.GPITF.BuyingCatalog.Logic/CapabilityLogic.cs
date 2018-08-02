using FluentValidation;
using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Collections.Generic;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class CapabilityLogic : LogicBase, ICapabilityLogic
  {
    private readonly ICapabilityDatastore _datastore;
    private readonly ICapabilityValidator _validator;

    public CapabilityLogic(
      ICapabilityDatastore datastore, 
      IHttpContextAccessor context,
      ICapabilityValidator validator) :
      base(context)
    {
      _datastore = datastore;
      _validator = validator;
    }

    public IQueryable<Capability> ByFramework(string frameworkId)
    {
      return _datastore.ByFramework(frameworkId);
    }

    public Capability ById(string id)
    {
      return _datastore.ById(id);
    }

    public IQueryable<Capability> ByIds(IEnumerable<string> ids)
    {
      return _datastore.ByIds(ids);
    }

    public IQueryable<Capability> ByStandard(string standardId, bool isOptional)
    {
      return _datastore.ByStandard(standardId, isOptional);
    }

    public Capability Create(Capability capability)
    {
      _validator.ValidateAndThrow(capability, ruleSet: nameof(ICapabilityLogic.Create));
      return _datastore.Create(capability);
    }

    public IQueryable<Capability> GetAll()
    {
      return _datastore.GetAll();
    }

    public void Update(Capability capability)
    {
      _validator.ValidateAndThrow(capability, ruleSet: nameof(ICapabilityLogic.Update));
      _datastore.Update(capability);
    }
  }
}
