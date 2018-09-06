using FluentValidation;
using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Collections.Generic;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public abstract class ClaimsLogicBase<T> : LogicBase, IClaimsLogic<T> where T : ClaimsBase
  {
    private readonly IClaimsDatastore<T> _datastore;
    private readonly IClaimsValidator<T> _validator;
    private readonly IClaimsFilter<T> _filter;

    public ClaimsLogicBase(
      IClaimsDatastore<T> datastore,
      IClaimsValidator<T> validator,
      IClaimsFilter<T> filter,
      IHttpContextAccessor context) :
      base(context)
    {
      _datastore = datastore;
      _validator = validator;
      _filter = filter;
    }

    public T ById(string id)
    {
      return _filter.Filter(new[] { _datastore.ById(id) }).SingleOrDefault();
    }

    public IEnumerable<T> BySolution(string solutionId)
    {
      return _filter.Filter(_datastore.BySolution(solutionId));
    }

    public T Create(T claim)
    {
      _validator.ValidateAndThrow(claim);
      _validator.ValidateAndThrow(claim, ruleSet: nameof(IClaimsLogic<T>.Create));

      return _datastore.Create(claim);
    }

    public void Update(T claim)
    {
      _validator.ValidateAndThrow(claim);
      _validator.ValidateAndThrow(claim, ruleSet: nameof(IClaimsLogic<T>.Update));

      _datastore.Update(claim);
    }

    public void Delete(T claim)
    {
      _validator.ValidateAndThrow(claim);
      _validator.ValidateAndThrow(claim, ruleSet: nameof(IClaimsLogic<T>.Delete));

      _datastore.Delete(claim);
    }
  }
}
