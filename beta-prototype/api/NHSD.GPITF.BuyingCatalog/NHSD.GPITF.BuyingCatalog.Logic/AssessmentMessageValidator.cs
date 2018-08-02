using FluentValidation;
using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class AssessmentMessageValidator : ValidatorBase<AssessmentMessage>, IAssessmentMessageValidator
  {
    public AssessmentMessageValidator(IHttpContextAccessor context) :
      base(context)
    {
      RuleSet(nameof(IAssessmentMessageLogic.BySolution), () =>
      {
        MustBeAdminOrSupplier();
      });
      RuleSet(nameof(IAssessmentMessageLogic.Create), () =>
      {
        MustBeAdminOrSupplier();
      });
      RuleSet(nameof(IAssessmentMessageLogic.Update), () =>
      {
        MustBeAdmin();
      });
      RuleSet(nameof(IAssessmentMessageLogic.Delete), () =>
      {
        MustBeAdmin();
      });
    }

  }
}
