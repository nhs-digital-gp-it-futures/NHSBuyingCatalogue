using FluentValidation;
using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class SolutionsLogic : LogicBase, ISolutionsLogic
  {
    private readonly ISolutionsDatastore _datastore;
    private readonly ISolutionsValidator _validator;
    private readonly ISolutionsFilter _filter;

    public SolutionsLogic(
      ISolutionsDatastore datastore,
      IHttpContextAccessor context,
      ISolutionsValidator validator,
      ISolutionsFilter filter) :
      base(context)
    {
      _datastore = datastore;
      _validator = validator;
      _filter = filter;
    }

    public IQueryable<Solutions> ByFramework(string frameworkId)
    {
      return _filter.Filter(_datastore.ByFramework(frameworkId));
    }

    public Solutions ById(string id)
    {
      return _filter.Filter(new[] { _datastore.ById(id) }.AsQueryable()).SingleOrDefault();
    }

    public IQueryable<Solutions> ByOrganisation(string organisationId)
    {
      return _filter.Filter(_datastore.ByOrganisation(organisationId));
    }

    public Solutions Create(Solutions solution)
    {
      _validator.ValidateAndThrow(solution);
      return _datastore.Create(solution);
    }

    public void Update(Solutions solution)
    {
      _validator.ValidateAndThrow(solution);
      _validator.ValidateAndThrow(solution, ruleSet: nameof(ISolutionsLogic.Update));

      _datastore.Update(solution);
    }

    public void Delete(Solutions solution)
    {
      _validator.ValidateAndThrow(solution);
      _validator.ValidateAndThrow(solution, ruleSet: nameof(ISolutionsLogic.Delete));
      _datastore.Delete(solution);
    }
  }
}
