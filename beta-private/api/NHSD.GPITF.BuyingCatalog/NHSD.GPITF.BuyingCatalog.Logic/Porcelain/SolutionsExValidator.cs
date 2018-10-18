using FluentValidation;
using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces.Porcelain;
using NHSD.GPITF.BuyingCatalog.Models.Porcelain;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Logic.Porcelain
{
  public sealed class SolutionsExValidator : ValidatorBase<SolutionEx>, ISolutionsExValidator
  {
    private readonly ISolutionsValidator _solutionsValidator;

    public SolutionsExValidator(
      IHttpContextAccessor context,
      ISolutionsValidator solutionsValidator) :
      base(context)
    {
      _solutionsValidator = solutionsValidator;

      RuleSet(nameof(ISolutionsExLogic.Update), () =>
      {
        // use Solution validator
        MustBeValidSolution();

        // internal consistency checks
        ClaimedCapabilityMustBelongToSolution();
        ClaimedCapabilityEvidenceMustBelongToClaim();
        ClaimedCapabilityReviewMustBelongToEvidence();

        ClaimedStandardMustBelongToSolution();
        ClaimedStandardEvidenceMustBelongToClaim();
        ClaimedStandardReviewMustBelongToEvidence();

        TechnicalContactMustBelongToSolution();

        // all previous versions in solution
        ClaimedCapabilityEvidencePreviousVersionMustBelongToSolution();
        ClaimedStandardEvidencePreviousVersionMustBelongToSolution();

        ClaimedCapabilityReviewPreviousVersionMustBelongToSolution();
        ClaimedStandardReviewPreviousVersionMustBelongToSolution();
      });
    }

    public void MustBeValidSolution()
    {
      RuleFor(x => x.Solution)
        .Must(soln =>
        {
          _solutionsValidator.ValidateAndThrow(soln, ruleSet: nameof(ISolutionsLogic.Update));
          return true;
        });
    }

    public void ClaimedCapabilityMustBelongToSolution()
    {
      RuleFor(x => x)
        .Must(soln =>
        {
          return soln.ClaimedCapability.All(cc => cc.SolutionId == soln.Solution.Id);
        });
    }

    public void ClaimedStandardMustBelongToSolution()
    {
      RuleFor(x => x)
        .Must(soln =>
        {
          return soln.ClaimedStandard.All(cs => cs.SolutionId == soln.Solution.Id);
        });
    }

    public void ClaimedCapabilityEvidenceMustBelongToClaim()
    {
      RuleFor(x => x)
        .Must(soln =>
        {
          var claimIds = soln.ClaimedCapability.Select(cc => cc.Id);
          return soln.ClaimedCapabilityEvidence.All(cce => claimIds.Contains(cce.ClaimId));
        });
    }

    public void ClaimedStandardEvidenceMustBelongToClaim()
    {
      RuleFor(x => x)
        .Must(soln =>
        {
          var claimIds = soln.ClaimedStandard.Select(cs => cs.Id);
          return soln.ClaimedStandardEvidence.All(cse => claimIds.Contains(cse.ClaimId));
        });
    }

    public void ClaimedCapabilityReviewMustBelongToEvidence()
    {
      RuleFor(x => x)
        .Must(soln =>
        {
          var evidenceIds = soln.ClaimedCapabilityEvidence.Select(cce => cce.Id);
          return soln.ClaimedCapabilityReview.All(ccr => evidenceIds.Contains(ccr.EvidenceId));
        });
    }

    public void ClaimedStandardReviewMustBelongToEvidence()
    {
      RuleFor(x => x)
        .Must(soln =>
        {
          var evidenceIds = soln.ClaimedStandardEvidence.Select(cse => cse.Id);
          return soln.ClaimedStandardReview.All(csr => evidenceIds.Contains(csr.EvidenceId));
        });
    }

    public void TechnicalContactMustBelongToSolution()
    {
      RuleFor(x => x)
        .Must(soln =>
        {
          return soln.TechnicalContact.All(tc => tc.SolutionId == soln.Solution.Id);
        });
    }

    public void ClaimedCapabilityEvidencePreviousVersionMustBelongToSolution()
    {
      RuleFor(x => x)
        .Must(soln =>
        {
          var evidenceIds = soln.ClaimedCapabilityEvidence.Select(cce => cce.Id);
          var evidencePrevIds = soln.ClaimedCapabilityEvidence.Select(cce => cce.PreviousId).Where(id => id != null);
          return evidencePrevIds.All(prevId => evidenceIds.Contains(prevId));
        });

    }

    public void ClaimedStandardEvidencePreviousVersionMustBelongToSolution()
    {
      RuleFor(x => x)
        .Must(soln =>
        {
          var evidenceIds = soln.ClaimedStandardEvidence.Select(cce => cce.Id);
          var evidencePrevIds = soln.ClaimedStandardEvidence.Select(cce => cce.PreviousId).Where(id => id != null);
          return evidencePrevIds.All(prevId => evidenceIds.Contains(prevId));
        });

    }

    public void ClaimedCapabilityReviewPreviousVersionMustBelongToSolution()
    {
      RuleFor(x => x)
        .Must(soln =>
        {
          var evidenceIds = soln.ClaimedCapabilityReview.Select(cce => cce.Id);
          var evidencePrevIds = soln.ClaimedCapabilityReview.Select(cce => cce.PreviousId).Where(id => id != null);
          return evidencePrevIds.All(prevId => evidenceIds.Contains(prevId));
        });

    }

    public void ClaimedStandardReviewPreviousVersionMustBelongToSolution()
    {
      RuleFor(x => x)
        .Must(soln =>
        {
          var evidenceIds = soln.ClaimedStandardReview.Select(cce => cce.Id);
          var evidencePrevIds = soln.ClaimedStandardReview.Select(cce => cce.PreviousId).Where(id => id != null);
          return evidencePrevIds.All(prevId => evidenceIds.Contains(prevId));
        });

    }
  }
}
