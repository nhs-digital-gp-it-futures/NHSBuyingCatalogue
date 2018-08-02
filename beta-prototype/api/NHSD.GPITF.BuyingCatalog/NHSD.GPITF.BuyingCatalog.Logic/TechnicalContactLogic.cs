using FluentValidation;
using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class TechnicalContactLogic : LogicBase, ITechnicalContactLogic
  {
    private readonly ITechnicalContactDatastore _datastore;
    private readonly ITechnicalContactValidator _validator;
    private readonly ITechnicalContactFilter _filter;

    public TechnicalContactLogic(
      ITechnicalContactDatastore datastore,
      IHttpContextAccessor context,
      ITechnicalContactValidator validator,
      ITechnicalContactFilter filter) :
      base(context)
    {
      _datastore = datastore;
      _validator = validator;
      _filter = filter;
    }

    public IQueryable<TechnicalContact> BySolution(string solutionId)
    {
      return _filter.Filter(_datastore.BySolution(solutionId));
    }

    public TechnicalContact Create(TechnicalContact techCont)
    {
      _validator.ValidateAndThrow(techCont, ruleSet: nameof(ITechnicalContactLogic.Create));
      return _datastore.Create(techCont);
    }

    public void Delete(TechnicalContact techCont)
    {
      _validator.ValidateAndThrow(techCont, ruleSet: nameof(ITechnicalContactLogic.Delete));
      _datastore.Delete(techCont);
    }

    public void Update(TechnicalContact techCont)
    {
      _validator.ValidateAndThrow(techCont, ruleSet: nameof(ITechnicalContactLogic.Update));
      _datastore.Update(techCont);
    }
  }
}
