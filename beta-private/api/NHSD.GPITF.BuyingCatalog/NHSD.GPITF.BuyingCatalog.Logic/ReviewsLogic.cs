using FluentValidation;
using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class ReviewsLogic : LogicBase, IReviewsLogic
  {
    private readonly IReviewsDatastore _datastore;
    private readonly IReviewsValidator _validator;
    private readonly IReviewsFilter _filter;

    public ReviewsLogic(
      IReviewsDatastore datastore,
      IHttpContextAccessor context,
      IReviewsValidator validator,
      IReviewsFilter filter) :
      base(context)
    {
      _datastore = datastore;
      _validator = validator;
      _filter = filter;
    }

    public IQueryable<Reviews> BySolution(string solutionId)
    {
      var assMesses = _datastore.BySolution(solutionId);
      assMesses.ToList().ForEach(assMess => _validator.ValidateAndThrow(assMess, ruleSet: nameof(IReviewsLogic.BySolution)));
      return _filter.Filter(assMesses);
    }

    public Reviews Create(Reviews assMess)
    {
      _validator.ValidateAndThrow(assMess, ruleSet: nameof(IReviewsLogic.Create));
      return _filter.Filter(new[] { _datastore.Create(assMess) }.AsQueryable()).SingleOrDefault();
    }

    public void Delete(Reviews assMess)
    {
      _validator.ValidateAndThrow( assMess, ruleSet: nameof(IReviewsLogic.Delete));
      throw new NotImplementedException("Cannot change history");
    }

    public void Update(Reviews assMess)
    {
      _validator.ValidateAndThrow(assMess, ruleSet: nameof(IReviewsLogic.Update));
      throw new NotImplementedException("Cannot change history");
    }
  }
}
