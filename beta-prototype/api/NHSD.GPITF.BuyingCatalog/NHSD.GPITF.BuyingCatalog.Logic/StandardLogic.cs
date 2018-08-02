using FluentValidation;
using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Collections.Generic;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class StandardLogic : LogicBase, IStandardLogic
  {
    private readonly IStandardDatastore _datastore;
    private readonly IStandardValidator _validator;

    public StandardLogic(
      IStandardDatastore datastore, 
      IHttpContextAccessor context,
      IStandardValidator validator
      ) :
      base(context)
    {
      _datastore = datastore;
      _validator = validator;
    }

    public IQueryable<Standard> ByCapability(string capabilityId, bool isOptional)
    {
      return _datastore.ByCapability(capabilityId, isOptional);
    }

    public IQueryable<Standard> ByFramework(string frameworkId)
    {
      return _datastore.ByFramework(frameworkId);
    }

    public Standard ById(string id)
    {
      return _datastore.ById(id);
    }

    public IQueryable<Standard> ByIds(IEnumerable<string> ids)
    {
      return _datastore.ByIds(ids);
    }

    public Standard Create(Standard standard)
    {
      _validator.ValidateAndThrow(standard, ruleSet: nameof(IStandardLogic.Create));
      return _datastore.Create(standard);
    }

    public IQueryable<Standard> GetAll()
    {
      return _datastore.GetAll();
    }

    public void Update(Standard standard)
    {
      _validator.ValidateAndThrow(standard, ruleSet: nameof(IStandardLogic.Create));
      _datastore.Update(standard);
    }
  }
}
