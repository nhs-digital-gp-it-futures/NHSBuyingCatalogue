﻿using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces.Porcelain;
using NHSD.GPITF.BuyingCatalog.Models;
using NHSD.GPITF.BuyingCatalog.Models.Porcelain;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Logic.Porcelain
{
  public sealed class SolutionsExValidator : ValidatorBase<SolutionEx>, ISolutionsExValidator
  {
    private readonly ISolutionsExDatastore _datastore;
    private readonly ISolutionsValidator _solutionsValidator;

    public SolutionsExValidator(
      IHttpContextAccessor context,
      ILogger<SolutionsExValidator> logger,
      ISolutionsExDatastore datastore,
      ISolutionsValidator solutionsValidator) :
      base(context, logger)
    {
      _datastore = datastore;
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


        // One Rule to rule them all,
        // One Rule to find them,
        // One Rule to bring them all,
        // and in the darkness bind them
        CheckUpdateAllowed();
      });
    }

    public void MustBeValidSolution()
    {
      RuleFor(x => x.Solution)
        .Must(soln =>
        {
          _solutionsValidator.ValidateAndThrowEx(soln, ruleSet: nameof(ISolutionsLogic.Update));
          return true;
        });
    }

    public void ClaimedCapabilityMustBelongToSolution()
    {
      RuleFor(x => x)
        .Must(soln =>
        {
          return soln.ClaimedCapability.All(cc => cc.SolutionId == soln.Solution.Id);
        })
        .WithMessage("ClaimedCapability must belong to solution");
    }

    public void ClaimedStandardMustBelongToSolution()
    {
      RuleFor(x => x)
        .Must(soln =>
        {
          return soln.ClaimedStandard.All(cs => cs.SolutionId == soln.Solution.Id);
        })
        .WithMessage("ClaimedStandard must belong to solution");
    }

    public void ClaimedCapabilityEvidenceMustBelongToClaim()
    {
      RuleFor(x => x)
        .Must(soln =>
        {
          var claimIds = soln.ClaimedCapability.Select(cc => cc.Id);
          return soln.ClaimedCapabilityEvidence.All(cce => claimIds.Contains(cce.ClaimId));
        })
        .WithMessage("ClaimedCapabilityEvidence must belong to claim");
    }

    public void ClaimedStandardEvidenceMustBelongToClaim()
    {
      RuleFor(x => x)
        .Must(soln =>
        {
          var claimIds = soln.ClaimedStandard.Select(cs => cs.Id);
          return soln.ClaimedStandardEvidence.All(cse => claimIds.Contains(cse.ClaimId));
        })
        .WithMessage("ClaimedStandardEvidence must belong to claim");
    }

    public void ClaimedCapabilityReviewMustBelongToEvidence()
    {
      RuleFor(x => x)
        .Must(soln =>
        {
          var evidenceIds = soln.ClaimedCapabilityEvidence.Select(cce => cce.Id);
          return soln.ClaimedCapabilityReview.All(ccr => evidenceIds.Contains(ccr.EvidenceId));
        })
        .WithMessage("ClaimedCapabilityReview must belong to evidence");
    }

    public void ClaimedStandardReviewMustBelongToEvidence()
    {
      RuleFor(x => x)
        .Must(soln =>
        {
          var evidenceIds = soln.ClaimedStandardEvidence.Select(cse => cse.Id);
          return soln.ClaimedStandardReview.All(csr => evidenceIds.Contains(csr.EvidenceId));
        })
        .WithMessage("ClaimedStandardReview must belong to evidence");
    }

    public void TechnicalContactMustBelongToSolution()
    {
      RuleFor(x => x)
        .Must(soln =>
        {
          return soln.TechnicalContact.All(tc => tc.SolutionId == soln.Solution.Id);
        })
        .WithMessage("TechnicalContact must belong to solution");
    }

    public void ClaimedCapabilityEvidencePreviousVersionMustBelongToSolution()
    {
      RuleFor(x => x)
        .Must(soln =>
        {
          var evidenceIds = soln.ClaimedCapabilityEvidence.Select(cce => cce.Id);
          var evidencePrevIds = soln.ClaimedCapabilityEvidence.Select(cce => cce.PreviousId).Where(id => id != null);
          return evidencePrevIds.All(prevId => evidenceIds.Contains(prevId));
        })
        .WithMessage("ClaimedCapabilityEvidence previous version must belong to solution");
    }

    public void ClaimedStandardEvidencePreviousVersionMustBelongToSolution()
    {
      RuleFor(x => x)
        .Must(soln =>
        {
          var evidenceIds = soln.ClaimedStandardEvidence.Select(cce => cce.Id);
          var evidencePrevIds = soln.ClaimedStandardEvidence.Select(cce => cce.PreviousId).Where(id => id != null);
          return evidencePrevIds.All(prevId => evidenceIds.Contains(prevId));
        })
        .WithMessage("ClaimedStandardEvidence previous version must belong to solution");
    }

    public void ClaimedCapabilityReviewPreviousVersionMustBelongToSolution()
    {
      RuleFor(x => x)
        .Must(soln =>
        {
          var evidenceIds = soln.ClaimedCapabilityReview.Select(cce => cce.Id);
          var evidencePrevIds = soln.ClaimedCapabilityReview.Select(cce => cce.PreviousId).Where(id => id != null);
          return evidencePrevIds.All(prevId => evidenceIds.Contains(prevId));
        })
        .WithMessage("ClaimedCapabilityReview previous version must belong to solution");
    }

    public void ClaimedStandardReviewPreviousVersionMustBelongToSolution()
    {
      RuleFor(x => x)
        .Must(soln =>
        {
          var evidenceIds = soln.ClaimedStandardReview.Select(cce => cce.Id);
          var evidencePrevIds = soln.ClaimedStandardReview.Select(cce => cce.PreviousId).Where(id => id != null);
          return evidencePrevIds.All(prevId => evidenceIds.Contains(prevId));
        })
        .WithMessage("ClaimedStandardReview previous version must belong to solution");
    }

    public void CheckUpdateAllowed()
    {
      SolutionEx oldSolnEx = null;

      RuleFor(x => x)
        .Must(newSolnEx =>
        {
          oldSolnEx = oldSolnEx ?? _datastore.BySolution(newSolnEx.Solution.Id);
          return MustBePendingToChangeClaimedCapability(oldSolnEx, newSolnEx);
        })
        .WithMessage("Must Be Pending To Change Claimed Capability");

      RuleFor(x => x)
        .Must(newSolnEx =>
        {
          oldSolnEx = oldSolnEx ?? _datastore.BySolution(newSolnEx.Solution.Id);
          return MustBePendingToChangeClaimedStandard(oldSolnEx, newSolnEx);
        })
        .WithMessage("Must Be Pending To Change Claimed Standard");

      RuleFor(x => x)
        .Must(newSolnEx =>
        {
          oldSolnEx = oldSolnEx ?? _datastore.BySolution(newSolnEx.Solution.Id);
          return MustBePendingToChangeClaimedCapabilityEvidence(oldSolnEx, newSolnEx);
        })
        .WithMessage("Must Be Pending To Change Claimed Capability Evidence");

      RuleFor(x => x)
        .Must(newSolnEx =>
        {
          oldSolnEx = oldSolnEx ?? _datastore.BySolution(newSolnEx.Solution.Id);
          return MustBePendingToChangeClaimedStandardEvidence(oldSolnEx, newSolnEx);
        })
        .WithMessage("Must Be Pending To Change Claimed Standard Evidence");
    }

    // can only add/remove ClaimedCapability while pending
    public bool MustBePendingToChangeClaimedCapability(SolutionEx oldSolnEx, SolutionEx newSolnEx)
    {
      var comparer = new CapabilitiesImplementedComparer();
      var newNotOld = newSolnEx.ClaimedCapability.Except(oldSolnEx.ClaimedCapability, comparer).ToList();
      var oldNotNew = oldSolnEx.ClaimedCapability.Except(newSolnEx.ClaimedCapability, comparer).ToList();
      var same = !newNotOld.Any() && !oldNotNew.Any();

      if (same)
      {
        // no add/remove
        return true;
      }

      if ((oldNotNew.Any() || newNotOld.Any()) &&
        !IsPendingForClaims(newSolnEx.Solution.Status))
      {
        // Can only add/remove ClaimedCapability while pending
        var msg = new { ErrorMessage = nameof(MustBePendingToChangeClaimedCapability), ExistingValue = oldSolnEx };
        _logger.LogError(JsonConvert.SerializeObject(msg));
        return false;
      }

      return true;
    }

    // can only add/remove ClaimedStandard while pending
    public bool MustBePendingToChangeClaimedStandard(SolutionEx oldSolnEx, SolutionEx newSolnEx)
    {
      var comparer = new StandardsApplicableComparer();
      var newNotOld = newSolnEx.ClaimedStandard.Except(oldSolnEx.ClaimedStandard, comparer).ToList();
      var oldNotNew = oldSolnEx.ClaimedStandard.Except(newSolnEx.ClaimedStandard, comparer).ToList();
      var same = !newNotOld.Any() && !oldNotNew.Any();

      if (same)
      {
        // no add/remove
        return true;
      }

      if ((oldNotNew.Any() || newNotOld.Any()) &&
        !IsPendingForClaims(newSolnEx.Solution.Status))
      {
        // Can only add/remove ClaimedStandard while pending
        var msg = new { ErrorMessage = nameof(MustBePendingToChangeClaimedStandard), ExistingValue = oldSolnEx };
        _logger.LogError(JsonConvert.SerializeObject(msg));
        return false;
      }

      return true;
    }

    // cannot change/remove ClaimedCapabilityEvidence but can add while pending
    public bool MustBePendingToChangeClaimedCapabilityEvidence(SolutionEx oldSolnEx, SolutionEx newSolnEx)
    {
      var comparer = new CapabilitiesImplementedEvidenceComparer();
      var newNotOld = newSolnEx.ClaimedCapabilityEvidence.Except(oldSolnEx.ClaimedCapabilityEvidence, comparer).ToList();
      var oldNotNew = oldSolnEx.ClaimedCapabilityEvidence.Except(newSolnEx.ClaimedCapabilityEvidence, comparer).ToList();

      if (newNotOld.Any() &&
        newSolnEx.ClaimedCapabilityEvidence.Count() > oldSolnEx.ClaimedCapabilityEvidence.Count() &&
        IsPendingForEvidence(newSolnEx.Solution.Status))
      {
        // added
        return true;
      }

      var same = !newNotOld.Any() && !oldNotNew.Any();
      if (!same)
      {
        var msg = new { ErrorMessage = nameof(MustBePendingToChangeClaimedCapabilityEvidence), ExistingValue = oldSolnEx };
        _logger.LogError(JsonConvert.SerializeObject(msg));
      }

      return same;
    }

    // cannot change/remove ClaimedStandardEvidence but can add while pending
    public bool MustBePendingToChangeClaimedStandardEvidence(SolutionEx oldSolnEx, SolutionEx newSolnEx)
    {
      var comparer = new StandardsApplicableEvidenceComparer();
      var newNotOld = newSolnEx.ClaimedStandardEvidence.Except(oldSolnEx.ClaimedStandardEvidence, comparer).ToList();
      var oldNotNew = oldSolnEx.ClaimedStandardEvidence.Except(newSolnEx.ClaimedStandardEvidence, comparer).ToList();

      if (newNotOld.Any() &&
        newSolnEx.ClaimedStandardEvidence.Count() > oldSolnEx.ClaimedStandardEvidence.Count() &&
        IsPendingForEvidence(newSolnEx.Solution.Status))
      {
        // added
        return true;
      }

      var same = !newNotOld.Any() && !oldNotNew.Any();
      if (!same)
      {
        var msg = new { ErrorMessage = nameof(MustBePendingToChangeClaimedStandardEvidence), ExistingValue = oldSolnEx };
        _logger.LogError(JsonConvert.SerializeObject(msg));
      }

      return same;
    }


    // cannot change/remove ClaimedCapabilityReview but can add while pending
    public bool MustBePendingToChangeClaimedCapabilityReview(SolutionEx oldSolnEx, SolutionEx newSolnEx)
    {
      var comparer = new CapabilitiesImplementedReviewsComparer();
      var newNotOld = newSolnEx.ClaimedCapabilityReview.Except(oldSolnEx.ClaimedCapabilityReview, comparer).ToList();
      var oldNotNew = oldSolnEx.ClaimedCapabilityReview.Except(newSolnEx.ClaimedCapabilityReview, comparer).ToList();

      if (newNotOld.Any() &&
        newSolnEx.ClaimedCapabilityReview.Count() > oldSolnEx.ClaimedCapabilityReview.Count() &&
        IsPendingForReview(newSolnEx.Solution.Status))
      {
        // added
        return true;
      }

      var same = !newNotOld.Any() && !oldNotNew.Any();
      if (!same)
      {
        var msg = new { ErrorMessage = nameof(MustBePendingToChangeClaimedCapabilityReview), ExistingValue = oldSolnEx };
        _logger.LogError(JsonConvert.SerializeObject(msg));
      }

      return same;
    }

    // cannot change/remove ClaimedStandardReview but can add while pending
    public bool MustBePendingToChangeClaimedStandardReview(SolutionEx oldSolnEx, SolutionEx newSolnEx)
    {
      var comparer = new StandardsApplicableReviewsComparer();
      var newNotOld = newSolnEx.ClaimedStandardReview.Except(oldSolnEx.ClaimedStandardReview, comparer).ToList();
      var oldNotNew = oldSolnEx.ClaimedStandardReview.Except(newSolnEx.ClaimedStandardReview, comparer).ToList();

      if (newNotOld.Any() &&
        newSolnEx.ClaimedStandardReview.Count() > oldSolnEx.ClaimedStandardReview.Count() &&
        IsPendingForReview(newSolnEx.Solution.Status))
      {
        // added
        return true;
      }

      var same = !newNotOld.Any() && !oldNotNew.Any();
      if (!same)
      {
        var msg = new { ErrorMessage = nameof(MustBePendingToChangeClaimedStandardReview), ExistingValue = oldSolnEx };
        _logger.LogError(JsonConvert.SerializeObject(msg));
      }

      return same;
    }

    // check every ClaimedCapability
    // check every ClaimedStandard
    // check every ClaimedCapabilityEvidence
    // check every ClaimedStandardEvidence
    // check every ClaimedCapabilityReview
    // check every ClaimedStandardReview

    private bool IsPendingForClaims(SolutionStatus status)
    {
      return status == SolutionStatus.Draft ||
        status == SolutionStatus.Registered;
    }

    private bool IsPendingForEvidence(SolutionStatus status)
    {
      return status == SolutionStatus.CapabilitiesAssessment ||
        status == SolutionStatus.StandardsCompliance;
    }

    private bool IsPendingForReview(SolutionStatus status)
    {
      return status == SolutionStatus.CapabilitiesAssessment ||
        status == SolutionStatus.StandardsCompliance;
    }
  }
}
