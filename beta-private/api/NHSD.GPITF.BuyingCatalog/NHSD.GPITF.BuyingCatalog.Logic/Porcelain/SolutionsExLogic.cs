using FluentValidation;
using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces.Porcelain;
using NHSD.GPITF.BuyingCatalog.Models.Porcelain;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Logic.Porcelain
{
  public sealed class SolutionsExLogic : LogicBase, ISolutionsExLogic
  {
    private readonly ISolutionsExDatastore _datastore;
    private readonly ISolutionsExValidator _validator;
    private readonly ISolutionsExFilter _filter;

    public SolutionsExLogic(
      ISolutionsExDatastore datastore,
      IHttpContextAccessor context,
      ISolutionsExValidator validator,
      ISolutionsExFilter filter) :
      base(context)
    {
      _datastore = datastore;
      _validator = validator;
      _filter = filter;
    }

    public SolutionEx BySolution(string solutionId)
    {
      return _filter.Filter(new[] { _datastore.BySolution(solutionId) }).SingleOrDefault();
    }

    public void Update(SolutionEx solnEx)
    {
      _validator.ValidateAndThrow(solnEx, ruleSet: nameof(ISolutionsExLogic.Update));

      _datastore.Update(solnEx);
    }
  }
}
