using FluentValidation;
using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System;
using System.Linq;
using System.Collections.Generic;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class SolutionsValidator : ValidatorBase<Solutions>, ISolutionsValidator
  {
    private readonly ISolutionsDatastore _solutionDatastore;
    private readonly IOrganisationsDatastore _organisationDatastore;

    public SolutionsValidator(
      IHttpContextAccessor context,
      ISolutionsDatastore solutionDatastore,
      IOrganisationsDatastore organisationDatastore) :
      base(context)
    {
      _solutionDatastore = solutionDatastore;
      _organisationDatastore = organisationDatastore;

      RuleSet(nameof(ISolutionsLogic.Update), () =>
      {
        RuleForUpdate();
      });

      RuleFor(x => x.Id).NotNull().Must(id => Guid.TryParse(id, out _)).WithMessage("Invalid Id");
      RuleFor(x => x.OrganisationId).NotNull().Must(orgId => Guid.TryParse(orgId, out _)).WithMessage("Invalid OrganisationId");
    }

    private void RuleForUpdate()
    {
      RuleFor(x => x)
        .Must(x =>
        {
          /// NOTE:  null solution check is not quite correct
          /// as this would result in a FK exception if we let it through
          /// but it is good enough for the moment
          var soln = _solutionDatastore.ById(x.Id);
          return soln != null && x.OrganisationId == soln.OrganisationId;
        })
        .WithMessage("Cannot transfer solutions between organisations");
      RuleFor(x => x)
        .Must(x =>
        {
          /// NOTE:  null solution check is not quite correct
          /// as this would result in a FK exception if we let it through
          /// but it is good enough for the moment
          var soln = _solutionDatastore.ById(x.Id);
          if (soln == null)
          {
            return false;
          }
          var oldStatus = soln.Status;
          var newStatus = x.Status;
          return ValidStatusTransitions.Any(
            trans => trans.OldStatus == oldStatus && trans.NewStatus == newStatus);
        })
        .WithMessage($"Invalid Status transition");
    }

    private static IEnumerable<(SolutionStatus OldStatus, SolutionStatus NewStatus)> ValidStatusTransitions
    {
      get
      {
        yield return (SolutionStatus.Draft, SolutionStatus.Draft);
        yield return (SolutionStatus.Draft, SolutionStatus.Registered);
        yield return (SolutionStatus.Registered, SolutionStatus.CapabilitiesAssessment);
        yield return (SolutionStatus.CapabilitiesAssessment, SolutionStatus.Failed);
        yield return (SolutionStatus.CapabilitiesAssessment, SolutionStatus.StandardsCompliance);
        yield return (SolutionStatus.StandardsCompliance, SolutionStatus.Failed);
        yield return (SolutionStatus.StandardsCompliance, SolutionStatus.FinalApproval);
        yield return (SolutionStatus.FinalApproval, SolutionStatus.SolutionPage);
        yield return (SolutionStatus.SolutionPage, SolutionStatus.Approved);
      }
    }
  }
}
