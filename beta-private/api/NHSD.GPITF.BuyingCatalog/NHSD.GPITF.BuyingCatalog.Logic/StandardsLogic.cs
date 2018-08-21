using FluentValidation;
using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Collections.Generic;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class StandardsLogic : LogicBase, IStandardsLogic
  {
    private readonly IStandardsDatastore _datastore;
    private readonly IStandardsValidator _validator;

    public StandardsLogic(
      IStandardsDatastore datastore, 
      IHttpContextAccessor context,
      IStandardsValidator validator
      ) :
      base(context)
    {
      _datastore = datastore;
      _validator = validator;
    }

    public IQueryable<Standards> ByCapability(string capabilityId, bool isOptional)
    {
      return _datastore.ByCapability(capabilityId, isOptional);
    }

    public IQueryable<Standards> ByFramework(string frameworkId)
    {
      return _datastore.ByFramework(frameworkId);
    }

    public Standards ById(string id)
    {
      return _datastore.ById(id);
    }

    public IQueryable<Standards> ByIds(IEnumerable<string> ids)
    {
      return _datastore.ByIds(ids);
    }

    public Standards Create(Standards standard)
    {
      _validator.ValidateAndThrow(standard, ruleSet: nameof(IStandardsLogic.Create));
      return _datastore.Create(standard);
    }

    public IQueryable<Standards> GetAll()
    {
      return _datastore.GetAll();
    }

    public void Update(Standards standard)
    {
      _validator.ValidateAndThrow(standard, ruleSet: nameof(IStandardsLogic.Create));
      _datastore.Update(standard);
    }
  }
}
