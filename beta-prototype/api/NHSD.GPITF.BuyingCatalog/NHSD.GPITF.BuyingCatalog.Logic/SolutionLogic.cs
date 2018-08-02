using FluentValidation;
using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class SolutionLogic : LogicBase, ISolutionLogic
  {
    private readonly ISolutionDatastore _datastore;
    private readonly ISolutionValidator _validator;
    private readonly ISolutionFilter _filter;

    public SolutionLogic(
      ISolutionDatastore datastore,
      IHttpContextAccessor context,
      ISolutionValidator validator,
      ISolutionFilter filter) :
      base(context)
    {
      _datastore = datastore;
      _validator = validator;
      _filter = filter;
    }

    public IQueryable<Solution> ByFramework(string frameworkId)
    {
      return _filter.Filter(_datastore.ByFramework(frameworkId));
    }

    public Solution ById(string id)
    {
      return _filter.Filter(new[] { _datastore.ById(id) }.AsQueryable()).SingleOrDefault();
    }

    public IQueryable<Solution> ByOrganisation(string organisationId)
    {
      return _filter.Filter(_datastore.ByOrganisation(organisationId));
    }

    public Solution Create(Solution solution)
    {
      _validator.ValidateAndThrow(solution);
      return _datastore.Create(solution);
    }

    public void Update(Solution solution)
    {
      _validator.ValidateAndThrow(solution);
      _validator.ValidateAndThrow(solution, ruleSet: nameof(ISolutionLogic.Update));

      _datastore.Update(solution);
    }

    public void Delete(Solution solution)
    {
      _validator.ValidateAndThrow(solution);
      _validator.ValidateAndThrow(solution, ruleSet: nameof(ISolutionLogic.Delete));
      _datastore.Delete(solution);
    }
  }
}
