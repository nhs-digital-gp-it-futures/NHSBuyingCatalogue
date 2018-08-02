using FluentValidation;
using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class AssessmentMessageLogic : LogicBase, IAssessmentMessageLogic
  {
    private readonly IAssessmentMessageDatastore _datastore;
    private readonly IAssessmentMessageValidator _validator;
    private readonly IAssessmentMessageFilter _filter;

    public AssessmentMessageLogic(
      IAssessmentMessageDatastore datastore,
      IHttpContextAccessor context,
      IAssessmentMessageValidator validator,
      IAssessmentMessageFilter filter) :
      base(context)
    {
      _datastore = datastore;
      _validator = validator;
      _filter = filter;
    }

    public IQueryable<AssessmentMessage> BySolution(string solutionId)
    {
      var assMesses = _datastore.BySolution(solutionId);
      assMesses.ToList().ForEach(assMess => _validator.ValidateAndThrow(assMess, ruleSet: nameof(IAssessmentMessageLogic.BySolution)));
      return _filter.Filter(assMesses);
    }

    public AssessmentMessage Create(AssessmentMessage assMess)
    {
      _validator.ValidateAndThrow(assMess, ruleSet: nameof(IAssessmentMessageLogic.Create));
      return _filter.Filter(new[] { _datastore.Create(assMess) }.AsQueryable()).SingleOrDefault();
    }

    public void Delete(AssessmentMessage assMess)
    {
      _validator.ValidateAndThrow( assMess, ruleSet: nameof(IAssessmentMessageLogic.Delete));
      throw new NotImplementedException("Cannot change history");
    }

    public void Update(AssessmentMessage assMess)
    {
      _validator.ValidateAndThrow(assMess, ruleSet: nameof(IAssessmentMessageLogic.Update));
      throw new NotImplementedException("Cannot change history");
    }
  }
}
