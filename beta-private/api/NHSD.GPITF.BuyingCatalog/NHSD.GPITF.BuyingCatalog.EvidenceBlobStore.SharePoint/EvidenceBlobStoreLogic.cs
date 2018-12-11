using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Collections.Generic;
using System.IO;

namespace NHSD.GPITF.BuyingCatalog.EvidenceBlobStore.SharePoint
{
  public abstract class EvidenceBlobStoreLogic : IClaimsInfoProvider, IEvidenceBlobStoreLogic
  {
    protected const string CapabilityFolderName = "Capability Evidence";
    protected const string StandardsFolderName = "Standards Evidence";

    private readonly IEvidenceBlobStoreDatastore _evidenceBlobStoreDatastore;
    protected readonly ICapabilitiesImplementedDatastore _capabilitiesImplementedDatastore;
    protected readonly IStandardsApplicableDatastore _standardsApplicableDatastore;
    protected readonly ICapabilitiesDatastore _capabilitiesDatastore;
    protected readonly IStandardsDatastore _standardsDatastore;
    protected readonly IEvidenceBlobStoreValidator _validator;
    protected readonly IEvidenceBlobStoreValidator _claimValidator;

    public EvidenceBlobStoreLogic(
      IEvidenceBlobStoreDatastore evidenceBlobStoreDatastore,
      ICapabilitiesImplementedDatastore capabilitiesImplementedDatastore,
      IStandardsApplicableDatastore standardsApplicableDatastore,
      ICapabilitiesDatastore capabilitiesDatastore,
      IStandardsDatastore standardsDatastore,
      IEvidenceBlobStoreValidator validator,
      IEvidenceBlobStoreValidator claimValidator)
    {
      _evidenceBlobStoreDatastore = evidenceBlobStoreDatastore;
      _capabilitiesImplementedDatastore = capabilitiesImplementedDatastore;
      _standardsApplicableDatastore = standardsApplicableDatastore;
      _capabilitiesDatastore = capabilitiesDatastore;
      _standardsDatastore = standardsDatastore;
      _validator = validator;
      _claimValidator = claimValidator;
    }

    public string AddEvidenceForClaim(string claimId, Stream file, string filename, string subFolder = null)
    {
      _claimValidator.ValidateAndThrow(claimId, ruleSet: nameof(IEvidenceBlobStoreLogic.AddEvidenceForClaim));

      var claim = ClaimsDatastore.ById(claimId);
      if (claim == null)
      {
        throw new KeyNotFoundException($"Could not find claim: {claimId}");
      }

      return _evidenceBlobStoreDatastore.AddEvidenceForClaim(this, claimId, file, filename, subFolder);
    }

    public FileStreamResult GetFileStream(string claimId, string uniqueId)
    {
      _claimValidator.ValidateAndThrow(claimId, ruleSet: nameof(IEvidenceBlobStoreLogic.GetFileStream));

      var claim = GetClaimById(claimId);
      if (claim == null)
      {
        throw new KeyNotFoundException($"Could not find claim: {claimId}");
      }

      return _evidenceBlobStoreDatastore.GetFileStream(this, claimId, uniqueId);
    }

    public IEnumerable<BlobInfo> EnumerateFolder(string claimId, string subFolder = null)
    {
      _claimValidator.ValidateAndThrow(claimId, ruleSet: nameof(IEvidenceBlobStoreLogic.EnumerateFolder));

      var claim = GetClaimById(claimId);
      if (claim == null)
      {
        throw new KeyNotFoundException($"Could not find claim: {claimId}");
      }

      return _evidenceBlobStoreDatastore.EnumerateFolder(this, claimId, subFolder);
    }

    public void PrepareForSolution(string solutionId)
    {
      _validator.ValidateAndThrow(solutionId, ruleSet: nameof(IEvidenceBlobStoreLogic.PrepareForSolution));

      _evidenceBlobStoreDatastore.PrepareForSolution(this, solutionId);
    }

    public abstract string GetFolderName();

    public abstract string GetFolderClaimName(ClaimsBase claim);

    public ClaimsBase GetClaimById(string claimId)
    {
      return ClaimsDatastore.ById(claimId);
    }

    public string GetCapabilityFolderName()
    {
      return CapabilityFolderName;
    }

    public string GetStandardsFolderName()
    {
      return StandardsFolderName;
    }

    protected abstract IClaimsDatastore<ClaimsBase> ClaimsDatastore { get; }
  }
}
