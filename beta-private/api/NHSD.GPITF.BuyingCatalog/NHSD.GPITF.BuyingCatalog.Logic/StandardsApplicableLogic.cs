using FluentValidation;
using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class StandardsApplicableLogic : LogicBase, IStandardsApplicableLogic
  {
    private readonly IStandardsApplicableDatastore _datastore;
    private readonly IStandardsApplicableValidator _validator;
    private readonly IStandardsApplicableFilter _filter;

    public StandardsApplicableLogic(
      IStandardsApplicableDatastore datastore,
      IHttpContextAccessor context,
      IStandardsApplicableValidator validator,
      IStandardsApplicableFilter filter) :
      base(context)
    {
      _datastore = datastore;
      _validator = validator;
      _filter = filter;
    }

    public IQueryable<StandardsApplicable> BySolution(string solutionId)
    {
      var claimedStds = _datastore.BySolution(solutionId);
      claimedStds.ToList().ForEach(cs => _validator.ValidateAndThrow(cs));
      return _filter.Filter(claimedStds);
    }

    public StandardsApplicable Create(StandardsApplicable claimedstandard)
    {
      _validator.ValidateAndThrow(claimedstandard);
      return _filter.Filter(new[] { _datastore.Create(claimedstandard) }.AsQueryable()).SingleOrDefault();
    }

    public void Update(StandardsApplicable claimedstandard)
    {
      _validator.ValidateAndThrow(claimedstandard);
      _datastore.Update(claimedstandard);
    }

    public void Delete(StandardsApplicable claimedstandard)
    {
      _validator.ValidateAndThrow(claimedstandard);
      _datastore.Delete(claimedstandard);
    }
  }
}
