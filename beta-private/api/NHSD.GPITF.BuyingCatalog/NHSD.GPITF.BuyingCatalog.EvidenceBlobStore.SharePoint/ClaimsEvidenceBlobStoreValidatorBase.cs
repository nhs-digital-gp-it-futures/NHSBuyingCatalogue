﻿using FluentValidation;
using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System;

namespace NHSD.GPITF.BuyingCatalog.EvidenceBlobStore.SharePoint
{
  public abstract class ClaimsEvidenceBlobStoreValidatorBase : EvidenceBlobStoreValidator
  {
    protected readonly IClaimsDatastore<ClaimsBase> _claimsDatastore;
    protected readonly ISolutionsDatastore _solutionsDatastore;

    public ClaimsEvidenceBlobStoreValidatorBase(
      IHttpContextAccessor context,
      ISolutionsDatastore solutionsDatastore,
      IClaimsDatastore<ClaimsBase> claimsDatastore) :
      base(context)
    {
      _solutionsDatastore = solutionsDatastore;
      _claimsDatastore = claimsDatastore;

      // claimId
      RuleSet(nameof(IEvidenceBlobStoreLogic.AddEvidenceForClaim), () =>
      {
        MustBeValidClaim();
        MustBeSameOrganisation();
      });

      // claimId
      RuleSet(nameof(IEvidenceBlobStoreLogic.EnumerateFolder), () =>
      {
        MustBeValidClaim();
        MustBeSameOrganisation();
      });
    }

    public void MustBeValidClaim()
    {
      RuleFor(x => x)
        .Must(x =>
        {
          var claim = _claimsDatastore.ById(x);
          return claim != null;
        })
        .WithMessage("Could not find claim");
    }

    public void MustBeSameOrganisation()
    {
      RuleFor(x => x)
        .Must(x =>
        {
          var orgId = _context.OrganisationId();
          var claim = _claimsDatastore.ById(x);
          var claimSoln = _solutionsDatastore.ById(claim?.SolutionId ?? Guid.NewGuid().ToString());
          return claimSoln?.OrganisationId == orgId;
        })
        .When(x => _context.HasRole(Roles.Supplier))
        .WithMessage("Cannot add/see evidence for other organisation");
    }
  }
}